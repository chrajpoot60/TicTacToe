import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GameService } from './services/game.service';
import { GameResponse, GameMode } from './models/game.models';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  game: GameResponse | null = null;
  errorMessage: string | null = null;
  selectedMode: GameMode = 'TwoPlayer';
  isLoading = false;

  constructor(private gameService: GameService) {}

  ngOnInit(): void {
    this.startNewGame();
  }

  startNewGame(): void {
    this.isLoading = true;
    this.errorMessage = null;
    this.gameService.createGame(this.selectedMode).subscribe({
      next: g => { this.game = g; this.isLoading = false; },
      error: () => { this.errorMessage = 'Could not connect to backend. Make sure the API is running on http://localhost:5000'; this.isLoading = false; }
    });
  }

  onModeChange(mode: GameMode): void {
    this.selectedMode = mode;
    this.startNewGame();
  }

  onCellClick(index: number): void {
    if (!this.game || this.game.status !== 'InProgress') return;
    if (this.game.board[index]) return;
    // Block click if it's computer's turn in Computer mode
    if (this.game.mode === 'Computer' && this.game.currentPlayer === 'O') return;

    const row = Math.floor(index / 3);
    const col = index % 3;
    this.isLoading = true;
    this.gameService.makeMove(this.game.id, row, col).subscribe({
      next: g => { this.game = g; this.isLoading = false; },
      error: err => {
        this.errorMessage = err.error?.error || 'Move failed';
        this.isLoading = false;
      }
    });
  }

  onUndo(): void {
    if (!this.game) return;
    this.gameService.undoMove(this.game.id).subscribe({
      next: g => { this.game = g; this.errorMessage = null; },
      error: err => this.errorMessage = err.error?.error || 'Cannot undo'
    });
  }

  onReset(): void {
    if (!this.game) return;
    this.gameService.resetGame(this.game.id).subscribe({
      next: g => { this.game = g; this.errorMessage = null; }
    });
  }

  onResetScoreboard(): void {
    if (!this.game) return;
    this.gameService.resetScoreboard().subscribe({
      next: sb => {
        if (this.game) this.game = { ...this.game, scoreboard: sb };
      }
    });
  }

  isWinningCell(index: number): boolean {
    return !!(this.game?.winningCells?.includes(index));
  }

  get canUndo(): boolean {
    return !!(this.game && this.game.moveHistory.length > 0 && this.game.status === 'InProgress');
  }

  get statusMessage(): string {
    if (!this.game) return '';
    if (this.game.status === 'Won') return `🎉 Player ${this.game.winner} Wins!`;
    if (this.game.status === 'Draw') return "🤝 It's a Draw!";
    if (this.game.mode === 'Computer' && this.game.currentPlayer === 'O')
      return '🤖 Computer is thinking...';
    return `Player ${this.game.currentPlayer}'s turn`;
  }

  get rowDescription(): string[] {
    return ['Row 1', 'Row 2', 'Row 3'];
  }

  get colDescription(): string[] {
    return ['Column 1', 'Column 2', 'Column 3'];
  }
}
