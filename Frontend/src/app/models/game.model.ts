/**
 * Game status enum
 * 0 = InProgress
 * 1 = Won
 * 2 = Draw
 */
export enum GameStatus {
  InProgress = 0,
  Won = 1,
  Draw = 2
}

export interface GameBoard {
  id: string;
  board: string[];
  currentPlayer: string;
  moveHistory: number[];
  status: GameStatus;
  winner: string | null;
  winningCells: number[];
  gameMode: string;
}

export interface ScoreboardEntry {
  playerId: string;
  playerName: string;
  wins: number;
  losses: number;
  draws: number;
  totalGames: number;
  winRate: number;
}

export interface Scoreboard {
  entries: ScoreboardEntry[];
}

export interface MoveRequest {
  position: number;
  player: string;
  playerId: string;
}

export interface ComputerMoveRequest {
  playerId: string;
}

export interface MoveHistoryEntry {
  moveNumber: number;
  player: string;
  position: number;
  rowCol: string;
}
