using Microsoft.AspNetCore.Mvc;
using TicTacToe.API.Models;
using TicTacToe.API.Services;

namespace TicTacToe.API.Controllers;

[ApiController]
[Route("api/scoreboard")]
public class ScoreboardController : ControllerBase
{
    private readonly IScoreboardService _scoreboardService;

    public ScoreboardController(IScoreboardService scoreboardService)
    {
        _scoreboardService = scoreboardService;
    }

    /// <summary>
    /// Get all player statistics from the scoreboard
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetScoreboard()
    {
        var scoreboard = await _scoreboardService.GetScoreboardAsync();
        return Ok(scoreboard);
    }

    /// <summary>
    /// Reset the entire scoreboard (clear all player statistics)
    /// </summary>
    [HttpPost("reset")]
    public async Task<IActionResult> ResetScoreboard()
    {
        await _scoreboardService.ResetScoreboardAsync();
        return Ok();
    }
}
