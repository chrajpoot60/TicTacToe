import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { GameService } from '../../services/game.service';
import { GameBoard, GameStatus, MoveHistoryEntry } from '../../models/game.model';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.scss']
})
export class GameComponent implements OnInit {
  game: GameBoard | null = null;
  playerId: string = '';
  gameMode: 'selection' | 'twoPlayer' | 'computer' = 'selection';
  loading = false;
  message = '';
  isComputerThinking = false;
  moveHistory: MoveHistoryEntry[] = [];
  winningCells: number[] = [];
  GameStatus = GameStatus; // For use in template

  // Game mode constants
  readonly GameMode = {
    TWO_PLAYER: 'twoPlayer',
    COMPUTER: 'computer'
  };

  // Player state for two-player mode
  private currentHumanPlayer = 'X';

  constructor(private gameService: GameService) {
    this.playerId = this.generatePlayerId();
  }

  ngOnInit(): void {
    // Start with mode selection
  }

  /**
   * Start a new game with the selected mode
   */
  startGame(mode: string): void {
    this.gameMode = mode as any;
    this.currentHumanPlayer = 'X';
    this.newGame();
  }

  /**
   * Create a new game
   */
  newGame(): void {
    this.loading = true;
    this.message = 'Starting new game...';
    this.moveHistory = [];
    this.winningCells = [];

    this.gameService.createGame().subscribe({
      next: (game) => {
        this.game = game;
        this.updateMessage();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error creating game:', err);
        this.message = 'Error creating game. Please try again.';
        this.loading = false;
      }
    });
  }

  /**
   * Reset the current game board (keep scoreboard)
   */
  resetGame(): void {
    if (!this.game) return;

    this.loading = true;
    this.message = 'Resetting game...';

    this.gameService.resetGame(this.game.id).subscribe({
      next: (updatedGame) => {
        this.game = updatedGame;
        this.moveHistory = [];
        this.winningCells = [];
        this.currentHumanPlayer = 'X';
        this.updateMessage();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error resetting game:', err);
        this.message = 'Error resetting game. Please try again.';
        this.loading = false;
      }
    });
  }

  /**
   * Handle cell click
   */
  makeMove(position: number): void {
    if (!this.game || this.game.status !== GameStatus.InProgress || this.game.board[position] !== ' ') {
      return;
    }

    // In two-player mode, alternate between X and O
    const player = this.gameMode === this.GameMode.TWO_PLAYER 
      ? this.game.currentPlayer 
      : 'X';

    this.loading = true;
    this.gameService.makeMove(this.game.id, position, player, this.playerId).subscribe({
      next: (updatedGame) => {
        this.game = updatedGame;
        this.updateMoveHistory();

        if (updatedGame.status !== GameStatus.InProgress) {
          // Game ended
          this.updateMessage();
          this.loading = false;
        } else if (this.gameMode === this.GameMode.COMPUTER && player === 'X') {
          // In computer mode, after player moves, computer should move
          this.message = 'Computer is thinking...';
          this.isComputerThinking = true;
          setTimeout(() => this.computerMove(), 800);
        } else if (this.gameMode === this.GameMode.TWO_PLAYER) {
          // In two-player mode, just update turn
          this.updateMessage();
          this.loading = false;
        }
      },
      error: (err) => {
        console.error('Error making move:', err);
        this.message = 'Error making move. Please try again.';
        this.loading = false;
      }
    });
  }

  /**
   * Computer makes a move
   */
  computerMove(): void {
    if (!this.game) return;

    this.gameService.makeComputerMove(this.game.id, this.playerId).subscribe({
      next: (updatedGame) => {
        this.game = updatedGame;
        this.updateMoveHistory();
        this.isComputerThinking = false;
        this.updateMessage();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error making computer move:', err);
        this.message = 'Error: Computer move failed';
        this.isComputerThinking = false;
        this.loading = false;
      }
    });
  }

  /**
   * Undo the last move(s)
   */
  undoMove(): void {
    if (!this.game) return;

    this.loading = true;
    this.gameService.undoMove(this.game.id).subscribe({
      next: (updatedGame) => {
        this.game = updatedGame;
        this.updateMoveHistory();
        this.message = 'Move undone! Your turn.';
        this.loading = false;
      },
      error: (err) => {
        console.error('Error undoing move:', err);
        this.message = 'Cannot undo: ' + (err.error?.message || 'Invalid operation');
        this.loading = false;
      }
    });
  }

  /**
   * Update status message based on game state
   */
  private updateMessage(): void {
    if (!this.game) return;

    if (this.game.status === GameStatus.Won) {
      const winner = this.game.winner === 'X' 
        ? (this.gameMode === this.GameMode.COMPUTER ? 'You' : 'X')
        : (this.gameMode === this.GameMode.COMPUTER ? 'Computer' : 'O');
      this.message = `🎉 ${winner} won!`;
    } else if (this.game.status === GameStatus.Draw) {
      this.message = "🤝 It's a draw!";
    } else if (this.gameMode === this.GameMode.COMPUTER) {
      this.message = this.game.currentPlayer === 'X' ? "Your turn (X)" : "Computer's turn (O)";
    } else {
      this.message = `${this.game.currentPlayer}'s turn`;
    }
  }

  /**
   * Update move history for display
   */
  private updateMoveHistory(): void {
    if (!this.game) return;

    this.moveHistory = [];
    for (let i = 0; i < this.game.moveHistory.length; i++) {
      const position = this.game.moveHistory[i];
      const player = i % 2 === 0 
        ? (this.gameMode === this.GameMode.COMPUTER ? 'X' : (i % 2 === 0 ? 'X' : 'O'))
        : (this.gameMode === this.GameMode.COMPUTER ? 'O' : (i % 2 === 0 ? 'X' : 'O'));
      
      this.moveHistory.push({
        moveNumber: i + 1,
        player,
        position,
        rowCol: this.getRowColFromPosition(position)
      });
    }
  }

  /**
   * Convert board position (0-8) to row and column
   */
  private getRowColFromPosition(position: number): string {
    const row = Math.floor(position / 3) + 1;
    const col = (position % 3) + 1;
    return `Row ${row}, Column ${col}`;
  }

  /**
   * Check if we can make a move
   */
  canMove(): boolean {
    if (!this.game) return false;
    if (this.game.status !== GameStatus.InProgress) return false;
    if (this.loading || this.isComputerThinking) return false;
    
    // In computer mode, only allow moves when it's player's turn (X)
    if (this.gameMode === this.GameMode.COMPUTER) {
      return this.game.currentPlayer === 'X';
    }
    
    return true;
  }

  /**
   * Check if undo is available
   */
  canUndo(): boolean {
    if (!this.game) return false;
    if (this.game.status !== GameStatus.InProgress) return false;
    if (this.loading || this.isComputerThinking) return false;
    
    // In computer mode, need at least 2 moves to undo (player + computer)
    // In two-player mode, need at least 1 move
    const minMoves = this.gameMode === this.GameMode.COMPUTER ? 2 : 1;
    return this.game.moveHistory.length >= minMoves;
  }

  /**
   * Get board display
   */
  getBoardDisplay(): string[] {
    return this.game?.board || Array(9).fill(' ');
  }

  /**
   * Check if a cell is part of winning combination
   */
  isWinningCell(position: number): boolean {
    return this.winningCells.includes(position);
  }

  /**
   * Generate unique player ID
   */
  private generatePlayerId(): string {
    return 'player_' + Math.random().toString(36).substr(2, 9);
  }

  /**
   * Get player display name based on mode
   */
  getPlayerDisplay(player: string): string {
    if (this.gameMode === this.GameMode.COMPUTER) {
      return player === 'X' ? 'You (X)' : 'Computer (O)';
    }
    return player;
  }
}
        this.loading = false;
      },
      error: (err) => {
        console.error('Error undoing move:', err);
        this.message = 'Cannot undo: ' + (err.error || 'Invalid operation');
        this.loading = false;
      }
    });
  }

  private updateMessage(): void {
    if (this.game?.status === 'Won') {
      this.message = this.game.winner === 'X' ? '🎉 You won!' : '😢 Computer won!';
    } else if (this.game?.status === 'Draw') {
      this.message = "It's a draw! 🤝";
    } else {
      this.message = this.game?.currentPlayer === 'X' ? "Your turn" : "Computer's turn";
    }
  }

  private generatePlayerId(): string {
    return 'player_' + Math.random().toString(36).substr(2, 9);
  }

  getBoardDisplay(): string[] {
    return this.game?.board || Array(9).fill(' ');
  }

  canMove(): boolean {
    return this.game?.status === 'InProgress' && !this.loading;
  }

  canUndo(): boolean {
    return !!this.game && this.game.moveHistory.length >= 2 && !this.loading && !this.isComputerThinking;
  }
}
