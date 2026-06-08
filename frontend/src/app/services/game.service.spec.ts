import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { GameService } from './game.service';

describe('GameService', () => {
  let service: GameService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [GameService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(GameService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('createGame should POST to /api/games', () => {
    service.createGame('TwoPlayer').subscribe();
    const req = httpMock.expectOne('http://localhost:5000/api/games');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ mode: 'TwoPlayer' });
    req.flush({});
  });

  it('makeMove should POST to /api/games/:id/moves', () => {
    service.makeMove('abc', 1, 2).subscribe();
    const req = httpMock.expectOne('http://localhost:5000/api/games/abc/moves');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ row: 1, column: 2 });
    req.flush({});
  });

  it('undoMove should POST to /api/games/:id/undo', () => {
    service.undoMove('abc').subscribe();
    const req = httpMock.expectOne('http://localhost:5000/api/games/abc/undo');
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('resetGame should POST to /api/games/:id/reset', () => {
    service.resetGame('abc').subscribe();
    const req = httpMock.expectOne('http://localhost:5000/api/games/abc/reset');
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('getScoreboard should GET /api/scoreboard', () => {
    service.getScoreboard().subscribe();
    const req = httpMock.expectOne('http://localhost:5000/api/scoreboard');
    expect(req.request.method).toBe('GET');
    req.flush({ xWins: 0, oWins: 0, draws: 0 });
  });
});
