import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GameBoard, Scoreboard, MoveRequest } from '../models/game.model';

/**
 * Service for communicating with the Tic Tac Toe backend API
 */
@Injectable({
  providedIn: 'root'
})
export class GameService {
  private apiUrl = 'http://localhost:54419/api';

  constructor(private http: HttpClient) {}

  /**
   * Create a new game session
   */
  createGame(): Observable<GameBoard> {
    return this.http.post<GameBoard>(`${this.apiUrl}/games`, {});
  }

  /**
   * Get current game state
   */
  getGame(gameId: string): Observable<GameBoard> {
    return this.http.get<GameBoard>(`${this.apiUrl}/games/${gameId}`);
  }

  /**
   * Make a player move
   */
  makeMove(gameId: string, position: number, player: string, playerId: string): Observable<GameBoard> {
    const request: MoveRequest = { position, player, playerId };
    return this.http.post<GameBoard>(`${this.apiUrl}/games/${gameId}/moves`, request);
  }

  /**
   * Make a computer move (AI opponent)
   */
  makeComputerMove(gameId: string, playerId: string): Observable<GameBoard> {
    return this.http.post<GameBoard>(`${this.apiUrl}/games/${gameId}/computer-move`, { playerId });
  }

  /**
   * Undo the last move(s)
   */
  undoMove(gameId: string): Observable<GameBoard> {
    return this.http.post<GameBoard>(`${this.apiUrl}/games/${gameId}/undo`, {});
  }

  /**
   * Reset the current game board
   */
  resetGame(gameId: string): Observable<GameBoard> {
    return this.http.post<GameBoard>(`${this.apiUrl}/games/${gameId}/reset`, {});
  }

  /**
   * Delete a game session
   */
  deleteGame(gameId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/games/${gameId}`);
  }

  /**
   * Get all player statistics
   */
  getScoreboard(): Observable<Scoreboard> {
    return this.http.get<Scoreboard>(`${this.apiUrl}/scoreboard`);
  }

  /**
   * Reset the scoreboard (clear all statistics)
   */
  resetScoreboard(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/scoreboard/reset`, {});
  }
}
