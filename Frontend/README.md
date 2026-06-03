# Tic Tac Toe - Angular Frontend

Modern Angular 18 frontend for the Tic Tac Toe application.

## Features

✅ **Latest Angular 18** - Standalone components with signals and modern features  
✅ **Responsive Design** - Works on desktop, tablet, and mobile  
✅ **Real-time Game Updates** - Smooth animations and interactions  
✅ **Scoreboard** - Track game statistics  
✅ **Beautiful UI** - Gradient backgrounds and smooth transitions  

## Requirements

- **Node.js** 18.13.0 or higher
- **npm** 9 or higher (or yarn/pnpm)

## Installation

```bash
npm install
```

## Development Server

```bash
npm start
```

The application will automatically open at `http://localhost:4200`

## Build

```bash
npm run build
```

Builds the project for production to the `dist/` directory.

```bash
npm run build:prod
```

## Project Structure

```
src/
├── app/
│   ├── components/
│   │   ├── game/
│   │   │   ├── game.component.ts
│   │   │   ├── game.component.html
│   │   │   └── game.component.scss
│   │   └── scoreboard/
│   │       ├── scoreboard.component.ts
│   │       ├── scoreboard.component.html
│   │       └── scoreboard.component.scss
│   ├── services/
│   │   └── game.service.ts
│   ├── models/
│   │   └── game.model.ts
│   ├── app.component.ts
│   └── app.routes.ts
├── index.html
├── main.ts
└── styles.scss
```

## API Integration

The frontend communicates with the backend API at `https://localhost:7200/api`. Make sure the backend is running before starting the frontend.

### Available Endpoints

- `POST /api/game/create` - Create new game
- `GET /api/game/{gameId}` - Get game state
- `POST /api/game/{gameId}/move` - Make player move
- `POST /api/game/{gameId}/computer-move` - Computer move
- `POST /api/game/{gameId}/undo` - Undo moves
- `GET /api/scoreboard` - Get scoreboard

## Technologies Used

- **Angular 18** - Latest framework
- **TypeScript 5.4** - Type-safe development
- **SCSS** - Advanced styling
- **RxJS** - Reactive programming
- **Standalone Components** - Modern Angular architecture

## Testing

```bash
ng test
```

Runs unit tests.

## Linting

```bash
ng lint
```

Runs ESLint to check code quality.
