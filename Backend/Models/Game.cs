using TicTacToe.API.Enums;

namespace TicTacToe.API.Models;

public class Game
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string[][] Board { get; set; } = null!;
    public string CurrentPlayer { get; set; } = "X";
    public GameMode Mode { get; set; }
    public GameStatus Status { get; set; } = GameStatus.InProgress;
    public string? Winner { get; set; }
    public List<int[]> WinningCells { get; set; } = new();
    public List<Move> MoveHistory { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Game()
    {
        Board = new string[3][];
        for (int i = 0; i < 3; i++)
        {
            Board[i] = new string[3];
            for (int j = 0; j < 3; j++)
            {
                Board[i][j] = "";
            }
        }
    }
}

