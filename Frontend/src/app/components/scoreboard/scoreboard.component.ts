import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { GameService } from '../../services/game.service';
import { ScoreboardEntry } from '../../models/game.model';

@Component({
  selector: 'app-scoreboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './scoreboard.component.html',
  styleUrls: ['./scoreboard.component.scss']
})
export class ScoreboardComponent implements OnInit {
  entries: ScoreboardEntry[] = [];
  loading = false;
  showResetConfirm = false;
  message = '';

  constructor(private gameService: GameService) {}

  ngOnInit(): void {
    this.loadScoreboard();
  }

  /**
   * Load scoreboard from backend
   */
  loadScoreboard(): void {
    this.loading = true;
    this.message = '';
    this.gameService.getScoreboard().subscribe({
      next: (scoreboard) => {
        this.entries = scoreboard.entries.sort((a, b) => b.wins - a.wins);
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading scoreboard:', err);
        this.message = 'Error loading scoreboard';
        this.loading = false;
      }
    });
  }

  /**
   * Reset scoreboard after confirmation
   */
  confirmReset(): void {
    this.showResetConfirm = true;
  }

  /**
   * Cancel reset operation
   */
  cancelReset(): void {
    this.showResetConfirm = false;
  }

  /**
   * Actually reset the scoreboard
   */
  resetScoreboard(): void {
    this.loading = true;
    this.gameService.resetScoreboard().subscribe({
      next: () => {
        this.entries = [];
        this.message = '✓ Scoreboard has been reset!';
        this.showResetConfirm = false;
        this.loading = false;
        setTimeout(() => {
          this.message = '';
        }, 3000);
      },
      error: (err) => {
        console.error('Error resetting scoreboard:', err);
        this.message = 'Error resetting scoreboard';
        this.showResetConfirm = false;
        this.loading = false;
      }
    });
  }

  /**
   * Get medal emoji for ranking
   */
  getMedalEmoji(index: number): string {
    switch (index) {
      case 0: return '🥇';
      case 1: return '🥈';
      case 2: return '🥉';
      default: return '•';
    }
  }

  /**
   * Get rank suffix (1st, 2nd, 3rd, etc.)
   */
  getRankSuffix(index: number): string {
    const rank = index + 1;
    if (rank % 10 === 1 && rank % 100 !== 11) return `${rank}st`;
    if (rank % 10 === 2 && rank % 100 !== 12) return `${rank}nd`;
    if (rank % 10 === 3 && rank % 100 !== 13) return `${rank}rd`;
    return `${rank}th`;
  }
}
