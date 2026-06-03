# Tic Tac Toe API Documentation

## Base URL
- **Development:** `http://localhost:54419` or `https://localhost:54418`
- **API Prefix:** `/api`

---

## Game Endpoints

### 1. Create a New Game
**Endpoint:** `POST /api/games`

**Description:** Creates a new game session with an empty 3×3 board.

**Request:**
```json
{}
```

**Response (201 Created):**
```json
{
  "id": "b34b5b95-dd07-48c3-9e78-a90fc9e02b4f",
  "board": [" ", " ", " ", " ", " ", " ", " ", " ", " "],
  "currentPlayer": "X",
  "moveHistory": [],
  "status": 0,
  "winner": null
}
```

**Status Codes:**
- `201 Created` - Game successfully created
- `400 Bad Request` - Invalid request

---

### 2. Get Game State
**Endpoint:** `GET /api/games/{id}`

**Description:** Retrieves the current state of a game by ID.

**Parameters:**
- `id` (path, required): Game ID

**Response (200 OK):**
```json
{
  "id": "b34b5b95-dd07-48c3-9e78-a90fc9e02b4f",
  "board": ["X", " ", " ", " ", "X", " ", " ", "O", " "],
  "currentPlayer": "O",
  "moveHistory": [4, 7],
  "status": 0,
  "winner": null
}
```

**Status Codes:**
- `200 OK` - Game state retrieved
- `404 Not Found` - Game ID not found

---

### 3. Make a Player Move
**Endpoint:** `POST /api/games/{id}/moves`

**Description:** Submits a move for the player (X or O).

**Parameters:**
- `id` (path, required): Game ID

**Request Body:**
```json
{
  "position": 4,
  "player": "X",
  "playerId": "player-123"
}
```

**Field Descriptions:**
- `position` (int, 0-8): Board position (0=top-left, 8=bottom-right)
- `player` (char): Player symbol ('X' or 'O')
- `playerId` (string): Unique player identifier for scoreboard tracking

**Response (200 OK):**
```json
{
  "id": "b34b5b95-dd07-48c3-9e78-a90fc9e02b4f",
  "board": [" ", " ", " ", " ", "X", " ", " ", " ", " "],
  "currentPlayer": "O",
  "moveHistory": [4],
  "status": 0,
  "winner": null
}
```

**Status Codes:**
- `200 OK` - Move accepted
- `400 Bad Request` - Invalid move (occupied cell, out of range, game finished)
- `404 Not Found` - Game ID not found

---

### 4. Make a Computer Move
**Endpoint:** `POST /api/games/{id}/computer-move`

**Description:** Triggers the AI opponent to make a move. The computer uses a smart strategy:
1. Attempts to win
2. Blocks player winning moves
3. Takes center position
4. Takes corner positions
5. Takes any available position

**Parameters:**
- `id` (path, required): Game ID

**Request Body:**
```json
{
  "playerId": "player-123"
}
```

**Response (200 OK):**
```json
{
  "id": "b34b5b95-dd07-48c3-9e78-a90fc9e02b4f",
  "board": [" ", " ", " ", " ", "X", " ", "O", " ", " "],
  "currentPlayer": "X",
  "moveHistory": [4, 6],
  "status": 0,
  "winner": null
}
```

**Status Codes:**
- `200 OK` - Computer move executed
- `400 Bad Request` - Game already finished
- `404 Not Found` - Game ID not found

---

### 5. Undo Last Move
**Endpoint:** `POST /api/games/{id}/undo`

**Description:** Undoes the last two moves (player move + computer move), returning the game to a previous state.

**Parameters:**
- `id` (path, required): Game ID

**Request:**
```json
{}
```

**Response (200 OK):**
```json
{
  "id": "b34b5b95-dd07-48c3-9e78-a90fc9e02b4f",
  "board": [" ", " ", " ", " ", " ", " ", " ", " ", " "],
  "currentPlayer": "X",
  "moveHistory": [],
  "status": 0,
  "winner": null
}
```

**Status Codes:**
- `200 OK` - Move undone
- `400 Bad Request` - Cannot undo (insufficient moves)
- `404 Not Found` - Game ID not found

---

### 6. Reset Game
**Endpoint:** `POST /api/games/{id}/reset`

**Description:** Resets the game board to initial state while keeping the same game ID.

**Parameters:**
- `id` (path, required): Game ID

**Request:**
```json
{}
```

**Response (200 OK):**
```json
{
  "id": "b34b5b95-dd07-48c3-9e78-a90fc9e02b4f",
  "board": [" ", " ", " ", " ", " ", " ", " ", " ", " "],
  "currentPlayer": "X",
  "moveHistory": [],
  "status": 0,
  "winner": null
}
```

**Status Codes:**
- `200 OK` - Game reset
- `404 Not Found` - Game ID not found

---

### 7. Delete Game
**Endpoint:** `DELETE /api/games/{id}`

**Description:** Deletes a game session.

**Parameters:**
- `id` (path, required): Game ID

**Response (204 No Content):** *(empty)*

**Status Codes:**
- `204 No Content` - Game deleted
- `404 Not Found` - Game ID not found (still returns 204)

---

## Scoreboard Endpoints

### 1. Get Scoreboard
**Endpoint:** `GET /api/scoreboard`

**Description:** Retrieves all player statistics from the scoreboard.

**Response (200 OK):**
```json
{
  "entries": [
    {
      "playerId": "player-123",
      "playerName": "Player_player",
      "wins": 5,
      "losses": 2,
      "draws": 1,
      "totalGames": 8,
      "winRate": 62.5
    }
  ]
}
```

**Status Codes:**
- `200 OK` - Scoreboard retrieved

---

### 2. Get Player Stats
**Endpoint:** `GET /api/scoreboard/{playerId}`

**Description:** Retrieves statistics for a specific player.

**Parameters:**
- `playerId` (path, required): Player ID

**Response (200 OK):**
```json
{
  "playerId": "player-123",
  "playerName": "Player_player",
  "wins": 5,
  "losses": 2,
  "draws": 1,
  "totalGames": 8,
  "winRate": 62.5
}
```

**Status Codes:**
- `200 OK` - Player stats retrieved
- `400 Bad Request` - Invalid request

---

### 3. Reset Scoreboard
**Endpoint:** `POST /api/scoreboard/reset`

**Description:** Clears all player statistics from the scoreboard.

**Request:**
```json
{}
```

**Response (200 OK):**
```json
{
  "message": "Scoreboard has been reset"
}
```

**Status Codes:**
- `200 OK` - Scoreboard reset
- `400 Bad Request` - Invalid request

---

## Game Status Codes

| Status | Value | Meaning |
|--------|-------|---------|
| InProgress | 0 | Game is ongoing |
| Won | 1 | Game ended with a winner |
| Draw | 2 | Game ended in a draw |

---

## Board Position Layout

The board positions are indexed 0-8 as follows:

```
 0 | 1 | 2
-----------
 3 | 4 | 5
-----------
 6 | 7 | 8
```

---

## Example Game Flow

### 1. Create Game
```
POST /api/games
```

### 2. Player Move (X at position 4)
```
POST /api/games/{gameId}/moves
{
  "position": 4,
  "player": "X",
  "playerId": "player-1"
}
```

### 3. Computer Move (O responds)
```
POST /api/games/{gameId}/computer-move
{
  "playerId": "player-1"
}
```

### 4. Check Scoreboard
```
GET /api/scoreboard
```

### 5. Undo Last Move (Optional)
```
POST /api/games/{gameId}/undo
```

### 6. Reset Game (Optional)
```
POST /api/games/{gameId}/reset
```

---

## Error Handling

All error responses follow this format:

```json
{
  "message": "Error description"
}
```

### Common Error Scenarios

**Invalid Position:**
```json
{
  "message": "Invalid position"
}
```

**Position Already Occupied:**
```json
{
  "message": "Position already occupied"
}
```

**Game Already Finished:**
```json
{
  "message": "Game is already finished"
}
```

**Cannot Undo:**
```json
{
  "message": "Cannot undo. Not enough moves."
}
```

**Game Not Found:**
```json
{
  "message": "Game [gameId] not found"
}
```

---

## CORS Policy

The API is configured to accept requests from:
- Origin: `http://localhost:4200` (Angular Frontend)
- Methods: All HTTP methods
- Headers: All headers

---

## Swagger UI

Access the interactive API documentation at:
- `https://localhost:54418/swagger/ui/index.html` (HTTPS)
- `http://localhost:54419/swagger/ui/index.html` (HTTP)

---

## Notes

- All game data is stored **in-memory** and will be lost when the server restarts
- Computer opponent uses deterministic AI strategy (not random)
- Undo removes both the player's last move and the computer's response
- Scoreboard persists for the duration of the server session
