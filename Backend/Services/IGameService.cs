using TicTacToe.API.Enums;
using TicTacToe.API.Models;

namespace TicTacToe.API.Services;

public interface IGameService
{
    Task<GameBoard> CreateGameAsync(GameMode mode);
    Task<GameBoard?> GetGameAsync(string gameId);
    Task<(bool success, string error, GameBoard? game)> MakeMoveAsync(string gameId, string player, int row, int col);
    Task<(bool success, GameBoard? game)> UndoMoveAsync(string gameId);
    Task<GameBoard> ResetGameAsync(string gameId);
    (bool hasWinner, string? winner, List<int[]> winningCells) CheckWinner(string[][] board);
    bool CheckDraw(string[][] board);
    (int row, int col)? GetComputerMove(GameBoard game);
}
