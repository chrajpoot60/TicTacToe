using TicTacToe.API.Models;

namespace TicTacToe.API.Services;

public interface IScoreboardService
{
    Task<Scoreboard> GetScoreboardAsync();
    Task UpdateScoreboardAsync(string? winner);
    Task ResetScoreboardAsync();
}
