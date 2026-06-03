namespace TicTacToe.API.Models;

public class Scoreboard
{
    public char Player { get; set; }
    public int XWins { get; set; }
    public int OWins { get; set; }
    public int Draws { get; set; }

    public int TotalGames => XWins + OWins + Draws;
    public double WinRate => TotalGames > 0 ? (double)(XWins + OWins) / TotalGames * 100 : 0;
}

