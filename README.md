# Tic Tac Toe Application

A browser-based Tic Tac Toe solution with an Angular frontend and an ASP.NET Core Web API backend.

## Current Status

- Backend builds successfully.
- Frontend currently does not build. The Angular app has TypeScript and SCSS syntax issues that need to be cleaned up before it can run.
- Backend and frontend contracts are not fully aligned yet. In particular, the frontend currently expects a flat board and `id`, while the backend returns `gameId` and a 2D board.

## Project Structure

```text
TicTacToe/
+-- Backend/                 # ASP.NET Core Web API
|   +-- Controllers/
|   |   +-- GameController.cs
|   |   +-- ScoreboardController.cs
|   +-- Enums/
|   |   +-- Enums.cs
|   +-- Models/
|   |   +-- GameBoard.cs
|   |   +-- Move.cs
|   |   +-- Scoreboard.cs
|   +-- Services/
|   |   +-- GameService.cs
|   |   +-- IGameService.cs
|   |   +-- ScoreboardService.cs
|   |   +-- IScoreboardService.cs
|   +-- Program.cs
|   +-- TicTacToe.API.csproj
|   +-- TicTacToe.API.sln
|
+-- Frontend/                # Angular application
    +-- src/
    |   +-- app/
    |       +-- components/
    |       |   +-- game/
    |       |   +-- scoreboard/
    |       +-- models/
    |       +-- services/
    |       +-- app.routes.ts
    +-- angular.json
    +-- package.json
    +-- proxy.conf.json
    +-- tsconfig.json
```

## Technology Stack

- Frontend: Angular 18, TypeScript
- Backend: ASP.NET Core Web API targeting `net10.0`
- API style: REST
- Storage: In-memory singleton services
- Communication: HTTP/HTTPS

## Backend

### Prerequisites

- .NET SDK compatible with `net10.0`

### Run

```bash
cd Backend
dotnet restore
dotnet run
```

The backend launch profile is configured for:

- HTTPS: `https://localhost:54418`
- HTTP: `http://localhost:54419`

Swagger is enabled in development.

### Backend Build Check

```bash
dotnet build Backend/TicTacToe.API.csproj
```

Current result: succeeds with nullable reference warnings in `GameService.cs`.

## Frontend

### Prerequisites

- Node.js 18+
- npm

### Install

```bash
cd Frontend
npm install
```

### Run

```bash
npm start
```

The Angular app is configured to serve at `http://localhost:4200`.

### Frontend Build Check

```bash
cd Frontend
npm run build
```

Current result: fails due to syntax errors in:

- `src/app/components/game/game.component.ts`
- `src/app/components/scoreboard/scoreboard.component.scss`

## API Endpoints

### Games

- `POST /api/games` - Create a new game. Optional query parameter: `mode`
- `GET /api/games/{id}` - Get current game state
- `POST /api/games/{id}/moves` - Submit a move
- `POST /api/games/{id}/undo` - Undo the last move, or the last player/computer turn pair in computer mode
- `POST /api/games/{id}/reset` - Reset the current board

### Scoreboard

- `GET /api/scoreboard` - Get aggregate scoreboard
- `POST /api/scoreboard/reset` - Reset scoreboard

## Implemented Backend Features

- Create and retrieve game sessions
- Two-player mode enum support
- Computer mode enum support
- In-memory game storage
- Move validation
- Winner detection
- Draw detection
- Move history
- Undo
- Reset board
- Simple computer move strategy
- Aggregate scoreboard for X wins, O wins, and draws

## Computer Move Strategy

The backend AI chooses a move using this order:

1. Win if O has an immediate winning move.
2. Block X if X has an immediate winning move.
3. Take the center.
4. Take a corner.
5. Take any remaining empty cell.

## Known Gaps

- Frontend does not currently compile.
- Frontend move requests use a single `position`, while the backend expects `row` and `col`.
- Frontend expects `id`, but backend responses use `gameId`.
- Frontend expects a flat `string[]` board, while backend returns `string[][]`.
- Frontend expects scoreboard entries, while backend returns aggregate scoreboard fields.
- Frontend calls `/api/games/{id}/computer-move`, but that backend endpoint is commented out. The backend currently performs the computer move automatically inside the player move endpoint when the game mode is `Computer`.
- Frontend does not currently pass the selected game mode when creating a game.

## Next Fixes

1. Clean up the corrupted/duplicated code in `game.component.ts`.
2. Fix the malformed SCSS in `scoreboard.component.scss`.
3. Align frontend models with backend response DTOs.
4. Convert frontend move positions into backend `row` and `col`, or change the backend to accept a flat position.
5. Decide whether computer moves should be automatic or exposed through a separate endpoint.
6. Add focused backend and frontend tests around move flow, undo, draw, win, and scoreboard updates.
