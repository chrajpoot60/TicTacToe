import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { AppComponent } from './app.component';
import { GameResponse } from './models/game.models';

const mockGame: GameResponse = {
  id: 'test-id',
  board: Array(9).fill(null),
  currentPlayer: 'X',
  mode: 'TwoPlayer',
  status: 'InProgress',
  winner: null,
  winningCells: null,
  moveHistory: [],
  scoreboard: { xWins: 0, oWins: 0, draws: 0 }
};

describe('AppComponent', () => {
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()]
    }).compileComponents();
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should create and load a game on init', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();

    const req = httpMock.expectOne('http://localhost:5000/api/games');
    expect(req.request.method).toBe('POST');
    req.flush(mockGame);

    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.board')).toBeTruthy();
  });

  it('should display 9 cells on the board', fakeAsync(() => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    httpMock.expectOne('http://localhost:5000/api/games').flush(mockGame);
    tick();
    fixture.detectChanges();

    const cells = fixture.nativeElement.querySelectorAll('.cell');
    expect(cells.length).toBe(9);
  }));

  it('should show TwoPlayer mode as default', fakeAsync(() => {
    const fixture = TestBed.createComponent(AppComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();
    httpMock.expectOne('http://localhost:5000/api/games').flush(mockGame);
    tick();

    expect(component.selectedMode).toBe('TwoPlayer');
  }));

  it('should disable undo when no moves', fakeAsync(() => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    httpMock.expectOne('http://localhost:5000/api/games').flush(mockGame);
    tick();
    fixture.detectChanges();

    const undoBtn = fixture.nativeElement.querySelector('.btn-secondary') as HTMLButtonElement;
    expect(undoBtn.disabled).toBeTrue();
  }));

  it('should display winning message on win', fakeAsync(() => {
    const fixture = TestBed.createComponent(AppComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();
    const wonGame: GameResponse = {
      ...mockGame,
      status: 'Won',
      winner: 'X',
      winningCells: [0, 1, 2]
    };
    httpMock.expectOne('http://localhost:5000/api/games').flush(wonGame);
    tick();
    fixture.detectChanges();

    expect(component.statusMessage).toContain('X Wins');
  }));

  it('should display draw message on draw', fakeAsync(() => {
    const fixture = TestBed.createComponent(AppComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();
    const drawGame: GameResponse = { ...mockGame, status: 'Draw' };
    httpMock.expectOne('http://localhost:5000/api/games').flush(drawGame);
    tick();
    fixture.detectChanges();

    expect(component.statusMessage).toContain('Draw');
  }));

  it('should call moves API on cell click', fakeAsync(() => {
    const fixture = TestBed.createComponent(AppComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();
    httpMock.expectOne('http://localhost:5000/api/games').flush(mockGame);
    tick();
    fixture.detectChanges();

    component.onCellClick(0);
    const moveReq = httpMock.expectOne('http://localhost:5000/api/games/test-id/moves');
    expect(moveReq.request.method).toBe('POST');
    expect(moveReq.request.body).toEqual({ row: 0, column: 0 });
    moveReq.flush({ ...mockGame, board: ['X', ...Array(8).fill(null)] });
  }));
});
