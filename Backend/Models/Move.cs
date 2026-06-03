namespace TicTacToe.API.Models
{
    public class Move
    {
        public int MoveNumber { get; set; }
        public string Player { get; set; } = null!;
        public int Row { get; set; }
        public int Col { get; set; }
        public string Position => $"Row {Row + 1}, Column {Col + 1}";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
