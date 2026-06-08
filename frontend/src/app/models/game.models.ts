export type Player = 'X' | 'O';
export type GameStatus = 'InProgress' | 'Won' | 'Draw';
export type GameMode = 'TwoPlayer' | 'Computer';

export interface MoveRecord {
  moveNumber: number;
  player: Player;
  row: number;
  column: number;
}

export interface Scoreboard {
  xWins: number;
  oWins: number;
  draws: number;
}

export interface GameResponse {
  id: string;
  board: (string | null)[];
  currentPlayer: Player;
  mode: GameMode;
  status: GameStatus;
  winner: Player | null;
  winningCells: number[] | null;
  moveHistory: MoveRecord[];
  scoreboard: Scoreboard;
}
