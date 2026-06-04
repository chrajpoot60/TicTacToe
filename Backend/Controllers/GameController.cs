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

        // Validate it's the correct player's turn
        if (game.CurrentPlayer != request.Player)
            return BadRequest(new { error = $"It's {game.CurrentPlayer}'s turn" });

        var (success, error, updatedGame) = await _gameService.MakeMoveAsync(id, request.Player, request.Row, request.Col);

        if (!success)
            return BadRequest(new { error });

        // Check if game was completed after this move
        if (updatedGame!.Status != GameStatus.InProgress)
        {
            await _scoreboardService.UpdateScoreboardAsync(updatedGame.Winner);
        }

        var scoreboard = await _scoreboardService.GetScoreboardAsync();
        var response = await BuildGameStateResponse(updatedGame, scoreboard);
        return Ok(response);
    }

    /// <summary>
    /// Make a computer move (AI opponent)
    /// </summary>
    [HttpPost("{id}/computer-move")]
    public async Task<ActionResult<GameStateResponse>> MakeComputerMove(string id)
    {
        var game = await _gameService.GetGameAsync(id);
        if (game == null)
            return NotFound();

        // Verify it's computer mode and computer's turn
        if (game.Mode != GameMode.Computer)
            return BadRequest(new { error = "Not in computer mode" });

        if (game.CurrentPlayer != "O")
            return BadRequest(new { error = "Not computer's turn" });

        if (game.Status != GameStatus.InProgress)
            return BadRequest(new { error = "Game is already completed" });

        // Get computer's move
        var computerMove = _gameService.GetComputerMove(game);
        if (!computerMove.HasValue)
            return BadRequest(new { error = "No valid moves available" });

        // Make the computer's move
        var (success, error, updatedGame) = await _gameService.MakeMoveAsync(id, "O", computerMove.Value.row, computerMove.Value.col);

        if (!success)
            return BadRequest(new { error });

        // Check if game was completed after computer's move
        if (updatedGame!.Status != GameStatus.InProgress)
        {
            await _scoreboardService.UpdateScoreboardAsync(updatedGame.Winner);
        }

        var scoreboard = await _scoreboardService.GetScoreboardAsync();
        var response = await BuildGameStateResponse(updatedGame, scoreboard);
        return Ok(response);
    }

    /// <summary>
    /// Undo the last move
    /// </summary>
    [HttpPost("{id}/undo")]
    public async Task<ActionResult<GameStateResponse>> UndoMove(string id)
    {
        var game = await _gameService.GetGameAsync(id);
        if (game == null)
            return NotFound();

        // Don't allow undo if game is completed (Option A)
        if (game.Status != GameStatus.InProgress)
            return BadRequest(new { error = "Cannot undo a completed game" });

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

    private async Task<GameStateResponse> BuildGameStateResponse(Game game, Scoreboard scoreboard)
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
