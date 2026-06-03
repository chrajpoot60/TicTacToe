using Microsoft.AspNetCore.Mvc;
using TicTacToe.API.Enums;
using TicTacToe.API.Models;
using TicTacToe.API.Services;

namespace TicTacToe.API.Controllers;

[ApiController]
[Route("api/games")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly IScoreboardService _scoreboardService;

    public GameController(IGameService gameService, IScoreboardService scoreboardService)
    {
        _gameService = gameService;
        _scoreboardService = scoreboardService;
    }

    /// <summary>
    /// Create a new game session
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<GameStateResponse>> CreateGame([FromQuery] GameMode mode = GameMode.TwoPlayer)
    {
        var game = await _gameService.CreateGameAsync(mode);
        var scoreboard = await _scoreboardService.GetScoreboardAsync();
        var response = await BuildGameStateResponse(game, scoreboard);
        return Ok(response);
    }

    /// <summary>
    /// Get current game state by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<GameStateResponse>> GetGame(string id)
    {
        var game = await _gameService.GetGameAsync(id);
        if (game == null)
            return NotFound();

        var scoreboard = await _scoreboardService.GetScoreboardAsync();
        var response = await BuildGameStateResponse(game, scoreboard);
        return Ok(response);
    }

    /// <summary>
    /// Submit a player move
    /// </summary>
    [HttpPost("{id}/moves")]
    public async Task<ActionResult<GameStateResponse>> MakeMove(string id, [FromBody] MoveRequest request)
    {
        var game = await _gameService.GetGameAsync(id);
        if (game == null)
            return NotFound();

        var (success, error, updatedGame) = await _gameService.MakeMoveAsync(id, request.Player, request.Row, request.Col);

        if (!success)
            return BadRequest(new { error });

        // Check if game was completed
        if (updatedGame!.Status != GameStatus.InProgress)
        {
            await _scoreboardService.UpdateScoreboardAsync(updatedGame.Winner);
        }

        // Handle computer move if in computer mode and game still in progress
        if (updatedGame.Mode == GameMode.Computer && updatedGame.Status == GameStatus.InProgress && updatedGame.CurrentPlayer == "O")
        {
            var computerMove = _gameService.GetComputerMove(updatedGame);
            if (computerMove.HasValue)
            {
                var (computerSuccess, _, computerUpdatedGame) = await _gameService.MakeMoveAsync(
                    id, "O", computerMove.Value.row, computerMove.Value.col);

                if (computerSuccess && computerUpdatedGame!.Status != GameStatus.InProgress)
                {
                    await _scoreboardService.UpdateScoreboardAsync(computerUpdatedGame.Winner);
                }
                updatedGame = computerUpdatedGame;
            }
        }

        var scoreboard = await _scoreboardService.GetScoreboardAsync();
        var response = await BuildGameStateResponse(updatedGame!, scoreboard);
        return Ok(response);
    }

    /// <summary>
    /// Make a computer move (AI opponent)
    /// </summary>
    //[HttpPost("{id}/computer-move")]
    //public ActionResult<GameBoard> MakeComputerMove(string id, [FromBody] ComputerMoveRequest request)
    //{
    //    try
    //    {
    //        var game = _gameService.MakeComputerMove(id);
            
    //        // Record game result when game ends
    //        if (game.Status == GameStatus.Won)
    //        {
    //            _scoreboardService.RecordWin(request.PlayerId, game.Winner ?? ' ');
    //        }
    //        else if (game.Status == GameStatus.Draw)
    //        {
    //            _scoreboardService.RecordDraw(request.PlayerId);
    //        }
            
    //        return Ok(game);
    //    }
    //    catch (KeyNotFoundException)
    //    {
    //        return NotFound(new { message = $"Game {id} not found" });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { message = ex.Message });
    //    }
    //}

    /// <summary>
    /// Undo the last move
    /// </summary>
    [HttpPost("{id}/undo")]
    public async Task<ActionResult<GameStateResponse>> UndoMove(string id)
    {
        var game = await _gameService.GetGameAsync(id);
        if (game == null)
            return NotFound();

        var (success, updatedGame) = await _gameService.UndoMoveAsync(id);

        if (!success)
            return BadRequest(new { error = "Cannot undo" });

        var scoreboard = await _scoreboardService.GetScoreboardAsync();
        var response = await BuildGameStateResponse(updatedGame!, scoreboard);
        return Ok(response);
    }

    /// <summary>
    /// Reset the current game board
    /// </summary>
    [HttpPost("{id}/reset")]
    public async Task<ActionResult<GameStateResponse>> ResetGame(string id)
    {
        var game = await _gameService.GetGameAsync(id);
        if (game == null)
            return NotFound();

        var resetGame = await _gameService.ResetGameAsync(id);
        var scoreboard = await _scoreboardService.GetScoreboardAsync();
        var response = await BuildGameStateResponse(resetGame, scoreboard);
        return Ok(response);
    }

    private async Task<GameStateResponse> BuildGameStateResponse(GameBoard game, Scoreboard scoreboard)
    {
        return new GameStateResponse
        {
            GameId = game.Id,
            Board = game.Board,
            CurrentPlayer = game.CurrentPlayer,
            Mode = game.Mode,
            Status = game.Status,
            Winner = game.Winner,
            WinningCells = game.WinningCells,
            MoveHistory = game.MoveHistory,
            Scoreboard = scoreboard,
            IsUndoAvailable = game.MoveHistory.Count > 0 && game.Status == GameStatus.InProgress
        };
    }
}

/// <summary>
/// Request model for making a player move
/// </summary>
public class MoveRequest
{
    public string GameId { get; set; } = null!;
    public string Player { get; set; } = null!;
    public int Row { get; set; }
    public int Col { get; set; }
}

/// <summary>
/// Request model for computer move
/// </summary>
public class ComputerMoveRequest
{
    /// <summary>Unique player identifier</summary>
    public string PlayerId { get; set; } = string.Empty;
}

/// <summary>
/// Gane state response model returned by API endpoints to provide current game status and details
/// </summary>
public class GameStateResponse
{
    public string GameId { get; set; } = null!;
    public string[][] Board { get; set; } = null!;
    public string CurrentPlayer { get; set; } = null!;
    public GameMode Mode { get; set; }
    public GameStatus Status { get; set; }
    public string? Winner { get; set; }
    public List<int[]> WinningCells { get; set; } = new();
    public List<Move> MoveHistory { get; set; } = new();
    public Scoreboard Scoreboard { get; set; } = null!;
    public bool IsUndoAvailable { get; set; }

}
