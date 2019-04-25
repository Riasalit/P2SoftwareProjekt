import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { renderTemplate } from '@angular/core/src/render3/instructions';

@Component({
  selector: 'home',
  templateUrl: './battleships.html'
})

export class BattleshipsComponent {
  public tiles: NodeData[][];
  public gameStarted: boolean;
  public boardSize: number;
  private url: string;
  private htClient: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.url = baseUrl;
    this.htClient = http;
    this.gameStarted = false;
    http.get<number>(baseUrl + 'api/BattleshipWeb/StartData').subscribe(result => {
      console.log(result);
      this.boardSize = result;
    }, error => console.error(error));

    console.log(this.gameStarted);
  }
  public StartGame() {
    this.tiles = [];
    for (var i = 0; i < this.boardSize; i++) {
      this.tiles[i] = []
      for (var j = 0; j < this.boardSize; j++) {
        this.tiles[i][j] = { tileName: String.fromCharCode(65 + i) + j, tileHit: 0};
        console.log(this.tiles[i][j].tileName);
      }
    }
    this.gameStarted = !this.gameStarted;
    console.log(this.gameStarted);
  }
}

interface NodeData {
  tileName: string;
  tileHit: number;
}

interface StartupData {
  boardSize: number;
}
