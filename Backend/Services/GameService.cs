using TicTacToe.API.Enums;
using TicTacToe.API.Models;

namespace TicTacToe.API.Services;

public class GameService : IGameService
{
    private readonly Dictionary<string, Game> _games = new();
    private readonly object _lockObject = new();

    public Task<Game> CreateGameAsync(GameMode mode)
    {
        var game = new Game { Mode = mode };
        _games[game.Id] = game;
        return Task.FromResult(game);
    }

    public Task<Game?> GetGameAsync(string gameId)
    {
        _games.TryGetValue(gameId, out var game);
        return Task.FromResult(game);
    }

    public Task<(bool success, string error, Game? game)> MakeMoveAsync(string gameId, string player, int row, int col)
    {
        lock (_lockObject)
        {
            if (!_games.TryGetValue(gameId, out var game))
                return Task.FromResult((false, "Game not found", game));

            if (game.Status != GameStatus.InProgress)
                return Task.FromResult((false, "Game is already completed", game));

            if (game.CurrentPlayer != player)
                return Task.FromResult((false, $"Not {player}'s turn", game));

            if (row < 0 || row > 2 || col < 0 || col > 2)
                return Task.FromResult((false, "Invalid position", game));

            if (!string.IsNullOrEmpty(game.Board[row][col]))
                return Task.FromResult((false, "Cell already occupied", game));

            // Make the move
            game.Board[row][col] = player;
            game.MoveHistory.Add(new Move
            {
                MoveNumber = game.MoveHistory.Count + 1,
                Player = player,
                Row = row,
                Col = col
            });

            // Check for winner
            var (hasWinner, winner, winningCells) = CheckWinner(game.Board);
            if (hasWinner)
            {
                game.Status = GameStatus.Won;
                game.Winner = winner;
                game.WinningCells = winningCells;
            }
            else if (CheckDraw(game.Board))
            {
                game.Status = GameStatus.Draw;
            }
            else
            {
                // Switch player
                game.CurrentPlayer = player == "X" ? "O" : "X";
            }

            return Task.FromResult((true, string.Empty, game));
        }
    }

    public Task<(bool success, Game? game)> UndoMoveAsync(string gameId)
    {
        lock (_lockObject)
        {
            if (!_games.TryGetValue(gameId, out var game))
                return Task.FromResult((false, game));

            if (game.MoveHistory.Count == 0)
                return Task.FromResult((false, game));

            // In computer mode, remove computer's move and human's move
            if (game.Mode == GameMode.Computer)
            {
                // Remove computer's move (O) if exists
                if (game.MoveHistory.Count > 0 && game.MoveHistory.Last().Player == "O")
                {
                    var lastMove = game.MoveHistory.Last();
                    game.Board[lastMove.Row][lastMove.Col] = "";
                    game.MoveHistory.RemoveAt(game.MoveHistory.Count - 1);
                }

                // Remove human's move (X) if exists
                if (game.MoveHistory.Count > 0 && game.MoveHistory.Last().Player == "X")
                {
                    var lastMove = game.MoveHistory.Last();
                    game.Board[lastMove.Row][lastMove.Col] = "";
                    game.MoveHistory.RemoveAt(game.MoveHistory.Count - 1);
                }
            }
            else
            {
                // Two player mode - remove only the last move
                var lastMove = game.MoveHistory.Last();
                game.Board[lastMove.Row][lastMove.Col] = "";
                game.MoveHistory.RemoveAt(game.MoveHistory.Count - 1);
            }

            // Reset game status
            game.Status = GameStatus.InProgress;
            game.Winner = null;
            game.WinningCells.Clear();

            // Set current player based on move history
            if (game.MoveHistory.Count == 0)
            {
                game.CurrentPlayer = "X";
            }
            else
            {
                game.CurrentPlayer = game.MoveHistory.Last().Player == "X" ? "O" : "X";
            }

            return Task.FromResult((true, game));
        }
    }

    public Task<Game> ResetGameAsync(string gameId)
    {
        lock (_lockObject)
        {
            if (!_games.TryGetValue(gameId, out var game))
                throw new ArgumentException("Game not found");

            // Reset board
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    game.Board[i][j] = "";
                }
            }

            game.CurrentPlayer = "X";
            game.Status = GameStatus.InProgress;
            game.Winner = null;
            game.WinningCells.Clear();
            game.MoveHistory.Clear();

            return Task.FromResult(game);
        }
    }

    public (bool hasWinner, string? winner, List<int[]> winningCells) CheckWinner(string[][] board)
    {
        // Check rows
        for (int i = 0; i < 3; i++)
        {
            if (!string.IsNullOrEmpty(board[i][0]) &&
                board[i][0] == board[i][1] &&
                board[i][1] == board[i][2])
            {
                return (true, board[i][0], new List<int[]> { new[] { i, 0 }, new[] { i, 1 }, new[] { i, 2 } });
            }
        }

        // Check columns
        for (int j = 0; j < 3; j++)
        {
            if (!string.IsNullOrEmpty(board[0][j]) &&
                board[0][j] == board[1][j] &&
                board[1][j] == board[2][j])
            {
                return (true, board[0][j], new List<int[]> { new[] { 0, j }, new[] { 1, j }, new[] { 2, j } });
            }
        }

        // Check diagonals
        if (!string.IsNullOrEmpty(board[0][0]) &&
            board[0][0] == board[1][1] &&
            board[1][1] == board[2][2])
        {
            return (true, board[0][0], new List<int[]> { new[] { 0, 0 }, new[] { 1, 1 }, new[] { 2, 2 } });
        }

        if (!string.IsNullOrEmpty(board[0][2]) &&
            board[0][2] == board[1][1] &&
            board[1][1] == board[2][0])
        {
            return (true, board[0][2], new List<int[]> { new[] { 0, 2 }, new[] { 1, 1 }, new[] { 2, 0 } });
        }

        return (false, null, new List<int[]>());
    }

    public bool CheckDraw(string[][] board)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (string.IsNullOrEmpty(board[i][j]))
                    return false;
            }
        }
        return true;
    }

    public (int row, int col)? GetComputerMove(Game game)
    {
        if (game.Status != GameStatus.InProgress)
            return null;

        // 1. If O can win, play the winning move
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (string.IsNullOrEmpty(game.Board[i][j]))
                {
                    // Try O move
                    game.Board[i][j] = "O";
                    var (hasWinner, winner, _) = CheckWinner(game.Board);
                    game.Board[i][j] = "";
                    if (hasWinner && winner == "O")
                        return (i, j);
                }
            }
        }

        // 2. If X can win next, block X
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (string.IsNullOrEmpty(game.Board[i][j]))
                {
                    // Check if X would win here
                    game.Board[i][j] = "X";
                    var (hasWinner, winner, _) = CheckWinner(game.Board);
                    game.Board[i][j] = "";
                    if (hasWinner && winner == "X")
                        return (i, j);
                }
            }
        }

        // 3. Take center if available
        if (string.IsNullOrEmpty(game.Board[1][1]))
            return (1, 1);

        // 4. Take a corner if available
        int[][] corners = { new[] { 0, 0 }, new[] { 0, 2 }, new[] { 2, 0 }, new[] { 2, 2 } };
        foreach (var corner in corners)
        {
            if (string.IsNullOrEmpty(game.Board[corner[0]][corner[1]]))
                return (corner[0], corner[1]);
        }

        // 5. Take any available cell
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (string.IsNullOrEmpty(game.Board[i][j]))
                    return (i, j);
            }
        }

        return null;
    }
}
