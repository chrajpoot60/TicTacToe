import { Routes } from '@angular/router';
import { GameComponent } from './components/game/game.component';
import { ScoreboardComponent } from './components/scoreboard/scoreboard.component';

export const routes: Routes = [
  { path: '', component: GameComponent },
  { path: 'scoreboard', component: ScoreboardComponent },
  { path: '**', redirectTo: '' }
];
