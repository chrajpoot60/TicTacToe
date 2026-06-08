using Microsoft.AspNetCore.Mvc;
using TicTacToe.API.Services;

namespace TicTacToe.API.Controllers;

[ApiController]
[Route("api/scoreboard")]
public class ScoreboardController(IGameService gameService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetScoreboard() => Ok(gameService.GetScoreboard());

    [HttpPost("reset")]
    public IActionResult ResetScoreboard() => Ok(gameService.ResetScoreboard());
}
