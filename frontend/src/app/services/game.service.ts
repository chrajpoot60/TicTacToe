import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GameResponse, GameMode, Scoreboard } from '../models/game.models';

@Injectable({ providedIn: 'root' })
export class GameService {
  private readonly base = 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  createGame(mode: GameMode): Observable<GameResponse> {
    var requestMode = mode=='TwoPlayer'? 0 : 1;
    return this.http.post<GameResponse>(`${this.base}/games`, { Mode: requestMode });
  }

  getGame(id: string): Observable<GameResponse> {
    return this.http.get<GameResponse>(`${this.base}/games/${id}`);
  }

  makeMove(gameId: string, row: number, column: number): Observable<GameResponse> {
    return this.http.post<GameResponse>(`${this.base}/games/${gameId}/moves`, { row, column });
  }

  undoMove(gameId: string): Observable<GameResponse> {
    return this.http.post<GameResponse>(`${this.base}/games/${gameId}/undo`, {});
  }

  resetGame(gameId: string): Observable<GameResponse> {
    return this.http.post<GameResponse>(`${this.base}/games/${gameId}/reset`, {});
  }

  getScoreboard(): Observable<Scoreboard> {
    return this.http.get<Scoreboard>(`${this.base}/scoreboard`);
  }

  resetScoreboard(): Observable<Scoreboard> {
    return this.http.post<Scoreboard>(`${this.base}/scoreboard/reset`, {});
  }
}
