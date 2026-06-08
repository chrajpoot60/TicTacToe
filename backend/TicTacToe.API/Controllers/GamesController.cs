using Microsoft.AspNetCore.Mvc;
using TicTacToe.API.Models;
using TicTacToe.API.Services;

namespace TicTacToe.API.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController(IGameService gameService) : ControllerBase
{
    private GameResponse ToResponse(GameState g) => new()
    {
        Id = g.Id,
        Board = g.Board,
        CurrentPlayer = g.CurrentPlayer.ToString(),
        Mode = g.Mode.ToString(),
        Status = g.Status.ToString(),
        Winner = g.Winner?.ToString(),
        WinningCells = g.WinningCells,
        MoveHistory = g.MoveHistory,
        Scoreboard = gameService.GetScoreboard()
    };

    [HttpPost]
    public IActionResult CreateGame([FromBody] CreateGameRequest req)
    {
        var game = gameService.CreateGame(req.Mode);
        return Ok(ToResponse(game));
    }

    [HttpGet("{id}")]
    public IActionResult GetGame(string id)
    {
        var game = gameService.GetGame(id);
        return game == null ? NotFound() : Ok(ToResponse(game));
    }

    [HttpPost("{id}/moves")]
    public IActionResult MakeMove(string id, [FromBody] MoveRequest req)
    {
        var (game, error) = gameService.MakeMove(id, req.Row, req.Column);
        if (game == null) return BadRequest(new { error });
        return Ok(ToResponse(game));
    }

    [HttpPost("{id}/undo")]
    public IActionResult UndoMove(string id)
    {
        var (game, error) = gameService.UndoMove(id);
        if (game == null) return BadRequest(new { error });
        return Ok(ToResponse(game));
    }

    [HttpPost("{id}/reset")]
    public IActionResult ResetGame(string id)
    {
        var game = gameService.ResetGame(id);
        return game == null ? NotFound() : Ok(ToResponse(game));
    }
}
