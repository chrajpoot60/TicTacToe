namespace TicTacToe.API.Models;

public enum GameStatus { InProgress, Won, Draw }
public enum GameMode { TwoPlayer, Computer }
public enum Player { X, O }

public class MoveRecord
{
    public int MoveNumber { get; set; }
    public Player Player { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
}

public class GameState
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    // "X", "O", or null for each cell
    public string?[] Board { get; set; } = new string?[9];
    public Player CurrentPlayer { get; set; } = Player.X;
    public GameMode Mode { get; set; } = GameMode.TwoPlayer;
    public GameStatus Status { get; set; } = GameStatus.InProgress;
    public Player? Winner { get; set; }
    public int[]? WinningCells { get; set; }
    public List<MoveRecord> MoveHistory { get; set; } = new();
    public bool ScoreboardUpdated { get; set; } = false;
}

public class Scoreboard
{
    public int XWins { get; set; }
    public int OWins { get; set; }
    public int Draws { get; set; }
}

// Request/Response DTOs
public record CreateGameRequest(GameMode Mode = GameMode.TwoPlayer);

public record MoveRequest(int Row, int Column);

public class GameResponse
{
    public string Id { get; set; } = "";
    public string?[] Board { get; set; } = new string?[9];
    public string CurrentPlayer { get; set; } = "X";
    public string Mode { get; set; } = "TwoPlayer";
    public string Status { get; set; } = "InProgress";
    public string? Winner { get; set; }
    public int[]? WinningCells { get; set; }
    public List<MoveRecord> MoveHistory { get; set; } = new();
    public Scoreboard Scoreboard { get; set; } = new();
}
