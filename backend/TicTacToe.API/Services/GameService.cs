using TicTacToe.API.Models;

namespace TicTacToe.API.Services;

public interface IGameService
{
    GameState CreateGame(GameMode mode);
    GameState? GetGame(string id);
    (GameState? state, string? error) MakeMove(string id, int row, int col);
    (GameState? state, string? error) UndoMove(string id);
    GameState? ResetGame(string id);
    Scoreboard GetScoreboard();
    Scoreboard ResetScoreboard();
}

public class GameService : IGameService
{
    private readonly Dictionary<string, GameState> _games = new();
    private Scoreboard _scoreboard = new();

    // Win combinations (cell indices)
    private static readonly int[][] WinCombos =
    [
        [0, 1, 2], [3, 4, 5], [6, 7, 8], // rows
        [0, 3, 6], [1, 4, 7], [2, 5, 8], // columns
        [0, 4, 8], [2, 4, 6]             // diagonals
    ];

    public GameState CreateGame(GameMode mode)
    {
        var game = new GameState { Mode = mode };
        _games[game.Id] = game;
        return game;
    }

    public GameState? GetGame(string id)
        => _games.TryGetValue(id, out var g) ? g : null;

    public (GameState? state, string? error) MakeMove(string id, int row, int col)
    {
        if (!_games.TryGetValue(id, out var game))
            return (null, "Game not found");

        if (game.Status != GameStatus.InProgress)
            return (null, "Game is already completed");

        if (row < 0 || row > 2 || col < 0 || col > 2)
            return (null, "Position out of bounds");

        int index = row * 3 + col;
        if (game.Board[index] != null)
            return (null, "Cell is already occupied");

        // Apply move
        ApplyMove(game, row, col, game.CurrentPlayer);
        EvaluateGame(game);

        // Computer move in Computer mode
        if (game.Mode == GameMode.Computer
            && game.Status == GameStatus.InProgress
            && game.CurrentPlayer == Player.O)
        {
            int compIndex = GetComputerMove(game.Board);
            ApplyMove(game, compIndex / 3, compIndex % 3, Player.O);
            EvaluateGame(game);
        }

        UpdateScoreboard(game);
        return (game, null);
    }

    public (GameState? state, string? error) UndoMove(string id)
    {
        if (!_games.TryGetValue(id, out var game))
            return (null, "Game not found");

        if (game.MoveHistory.Count == 0)
            return (null, "No moves to undo");

        // Option A: Undo disabled after game completion
        if (game.Status != GameStatus.InProgress)
            return (null, "Cannot undo after game completion");

        int movesToRemove = game.Mode == GameMode.Computer
            ? Math.Min(2, game.MoveHistory.Count)
            : 1;

        // Rebuild state by replaying from start
        var history = game.MoveHistory.ToList();
        history.RemoveRange(history.Count - movesToRemove, movesToRemove);

        // Reset board
        game.Board = new string?[9];
        game.Winner = null;
        game.WinningCells = null;
        game.Status = GameStatus.InProgress;
        game.CurrentPlayer = Player.X;
        game.MoveHistory = new List<MoveRecord>();

        // Replay remaining moves
        foreach (var move in history)
            ApplyMove(game, move.Row, move.Column, move.Player);

        EvaluateGame(game);
        return (game, null);
    }

    public GameState? ResetGame(string id)
    {
        if (!_games.TryGetValue(id, out var game)) return null;

        game.Board = new string?[9];
        game.CurrentPlayer = Player.X;
        game.Status = GameStatus.InProgress;
        game.Winner = null;
        game.WinningCells = null;
        game.MoveHistory = new List<MoveRecord>();
        game.ScoreboardUpdated = false;
        return game;
    }

    public Scoreboard GetScoreboard() => _scoreboard;

    public Scoreboard ResetScoreboard()
    {
        _scoreboard = new Scoreboard();
        return _scoreboard;
    }

    // ---------- Internals ----------

    private void ApplyMove(GameState game, int row, int col, Player player)
    {
        int index = row * 3 + col;
        game.Board[index] = player.ToString();
        game.MoveHistory.Add(new MoveRecord
        {
            MoveNumber = game.MoveHistory.Count + 1,
            Player = player,
            Row = row,
            Column = col
        });
        game.CurrentPlayer = player == Player.X ? Player.O : Player.X;
    }

    private void EvaluateGame(GameState game)
    {
        foreach (var combo in WinCombos)
        {
            var a = game.Board[combo[0]];
            if (a != null && a == game.Board[combo[1]] && a == game.Board[combo[2]])
            {
                game.Status = GameStatus.Won;
                game.Winner = a == "X" ? Player.X : Player.O;
                game.WinningCells = combo;
                return;
            }
        }

        if (game.Board.All(c => c != null))
            game.Status = GameStatus.Draw;
    }

    private void UpdateScoreboard(GameState game)
    {
        if (game.ScoreboardUpdated) return;
        if (game.Status == GameStatus.Won)
        {
            if (game.Winner == Player.X) _scoreboard.XWins++;
            else _scoreboard.OWins++;
            game.ScoreboardUpdated = true;
        }
        else if (game.Status == GameStatus.Draw)
        {
            _scoreboard.Draws++;
            game.ScoreboardUpdated = true;
        }
    }

    /// <summary>Computer AI: win > block > center > corner > any</summary>
    private int GetComputerMove(string?[] board)
    {
        // Win
        var win = FindBestMove(board, "O");
        if (win.HasValue) return win.Value;

        // Block
        var block = FindBestMove(board, "X");
        if (block.HasValue) return block.Value;

        // Center
        if (board[4] == null) return 4;

        // Corner
        int[] corners = [0, 2, 6, 8];
        var corner = corners.FirstOrDefault(c => board[c] == null, -1);
        if (corner >= 0) return corner;

        // Any
        return Array.FindIndex(board, c => c == null);
    }

    private int? FindBestMove(string?[] board, string player)
    {
        foreach (var combo in WinCombos)
        {
            var cells = combo.Select(i => board[i]).ToArray();
            if (cells.Count(c => c == player) == 2 && cells.Any(c => c == null))
                return combo[Array.IndexOf(cells, null)];
        }
        return null;
    }
}
