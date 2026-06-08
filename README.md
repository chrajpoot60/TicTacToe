# Tic Tac Toe — Angular 18 + .NET 10

A full-stack Tic Tac Toe application built with an **Angular 18** frontend and a **.NET 10** REST API backend.

---

## Tech Stack

| Layer     | Technology                          |
|-----------|-------------------------------------|
| Frontend  | Angular 18, TypeScript, SCSS        |
| Backend   | .NET 10, ASP.NET Core Web API       |
| API Style | RESTful JSON                        |
| Storage   | In-memory (singleton service)       |
| Testing   | xUnit + FluentAssertions (backend), Jasmine + Karma (frontend) |

---

## Features Implemented

- ✅ 3×3 Tic Tac Toe board
- ✅ Two Player Mode
- ✅ Play Against Computer (with AI priority: win → block → center → corner → random)
- ✅ Turn switching (X always goes first)
- ✅ Win detection (rows, columns, diagonals) with winning cell highlight
- ✅ Draw detection
- ✅ Move history table
- ✅ Undo last move (Two Player: 1 move; Computer: 2 moves)
- ✅ Reset Game (board only, scoreboard preserved)
- ✅ Scoreboard (X wins, O wins, Draws)
- ✅ Reset Scoreboard
- ✅ Backend owns game state (source of truth)
- ✅ Move validation (bounds, occupied cells, wrong turn, game complete)
- ✅ Unit tests for backend and frontend

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 18+](https://nodejs.org/) and npm
- [Angular CLI 18](https://angular.io/cli): `npm install -g @angular/cli`

---

## How to Run — Backend

```bash
cd backend/TicTacToe.API
dotnet run
```

The API will start at **http://localhost:5000**.

---

## How to Run — Frontend

```bash
cd frontend
npm install
ng serve
```

Open your browser at **http://localhost:4200**.

---

## API Endpoint Summary

| Method | Endpoint                    | Description             |
|--------|-----------------------------|-------------------------|
| POST   | /api/games                  | Create a new game       |
| GET    | /api/games/{id}             | Get current game state  |
| POST   | /api/games/{id}/moves       | Submit a player move    |
| POST   | /api/games/{id}/undo        | Undo last move(s)       |
| POST   | /api/games/{id}/reset       | Reset the current game  |
| GET    | /api/scoreboard             | Get scoreboard          |
| POST   | /api/scoreboard/reset       | Reset scoreboard        |

### Create Game Request
```json
{ "mode": "TwoPlayer" }   // or "Computer"
```

### Make Move Request
```json
{ "row": 0, "column": 2 }
```

### Game State Response
```json
{
  "id": "...",
  "board": ["X", null, "O", ...],
  "currentPlayer": "X",
  "mode": "TwoPlayer",
  "status": "InProgress",
  "winner": null,
  "winningCells": null,
  "moveHistory": [
    { "moveNumber": 1, "player": "X", "row": 0, "column": 0 }
  ],
  "scoreboard": { "xWins": 1, "oWins": 0, "draws": 0 }
}
```

---

## How to Run Tests

### Backend Tests
```bash
cd backend
dotnet test
```

### Frontend Tests
```bash
cd frontend
ng test --watch=false --browsers=ChromeHeadless
```

---

## Design Decisions

### Option A — Undo Disabled After Game Completion
Once a game reaches `Won` or `Draw` status, the Undo button is disabled. The scoreboard update is final for that game. This keeps the scoreboard simple and consistent. (**Option A chosen**)

### Backend as Source of Truth
All game logic (validation, win/draw detection, computer moves, undo) lives in the .NET backend. The Angular frontend is a pure rendering layer that calls APIs and displays whatever the backend returns.

### Computer AI Priority
The computer (Player O) follows this move selection order:
1. Win if possible (take winning cell)
2. Block the human (block X's winning move)
3. Take center (index 4)
4. Take a corner (0, 2, 6, 8)
5. Take any available cell

### In-Memory Storage
`GameService` is registered as a **Singleton** in .NET DI, so state persists for the lifetime of the process. On restart, all games and the scoreboard reset. SQLite could be added by injecting a DbContext instead.

### Standalone Angular Components
The app uses Angular 18's standalone component API (no NgModules). `AppComponent` imports only `CommonModule` and is bootstrapped directly in `main.ts`.

---

## Clarifications & Assumptions

- **Undo approach**: Option A (disabled after completion).
- **Computer plays as O**: Human is always X in Computer mode.
- **Scoreboard is session-level**: Resets when the backend process restarts.
- **Game ID persists through reset**: Resetting a game reuses the same game ID.
- **No authentication**: All endpoints are open (local development only).

---

## Known Limitations

- In-memory only: no persistence between server restarts.
- Single scoreboard shared across all sessions.
- No WebSocket — the frontend polls only on user action (no real-time multiplayer across browser tabs).

---

## Future Improvements

- Add SQLite or EF Core for persistent storage
- Add WebSocket support for real-time multiplayer
- Add difficulty levels for computer AI (minimax)
- Add player name customization
- Add animated board transitions
- Add game history / replay feature

---

## AI Tools & Prompt Summary

This solution was built with AI assistance. Key prompts used:
- "Implement Tic Tac Toe problem statement: Angular 18 + .NET 10 backend"
- "Implement game service with win detection, undo, scoreboard, computer AI"
- "Write comprehensive xUnit tests covering all acceptance criteria"
- "Write Angular component with responsive board, move history, scoreboard"

All generated code was reviewed for correctness against the problem statement requirements.
