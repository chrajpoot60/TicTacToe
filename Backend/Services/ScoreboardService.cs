using TicTacToe.API.Models;

namespace TicTacToe.API.Services;

public class ScoreboardService : IScoreboardService
{
    private Scoreboard _scoreboard = new();
    private readonly HashSet<string> _recordedGames = new();

    public Task<Scoreboard> GetScoreboardAsync()
    {
        return Task.FromResult(_scoreboard);
    }

    public Task UpdateScoreboardAsync(string? winner)
    {
        if (winner == "X")
            _scoreboard.XWins++;
        else if (winner == "O")
            _scoreboard.OWins++;
        else if (winner == null)
            _scoreboard.Draws++;

        return Task.CompletedTask;
    }

    public Task ResetScoreboardAsync()
    {
        _scoreboard = new Scoreboard();
        return Task.CompletedTask;
    }
}
