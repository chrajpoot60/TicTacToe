using TicTacToe.API.Enums;
using TicTacToe.API.Models;

namespace TicTacToe.API.Services;

public interface IGameService
{
    Task<Game> CreateGameAsync(GameMode mode);
    Task<Game?> GetGameAsync(string gameId);
    Task<(bool success, string error, Game? game)> MakeMoveAsync(string gameId, string player, int row, int col);
    Task<(bool success, Game? game)> UndoMoveAsync(string gameId);
    Task<Game> ResetGameAsync(string gameId);
    (bool hasWinner, string? winner, List<int[]> winningCells) CheckWinner(string[][] board);
    bool CheckDraw(string[][] board);
    (int row, int col)? GetComputerMove(Game game);
}
