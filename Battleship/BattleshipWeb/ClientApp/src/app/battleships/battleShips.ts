import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { renderTemplate } from '@angular/core/src/render3/instructions';

@Component({
  selector: 'home',
  templateUrl: './battleships.html'
})

export class BattleshipsComponent {
  public tiles: nodeData[][];
  public gameStarted: boolean;
  private url: string;
  private htClient: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.htClient.get<boolean>(this.url + 'api/BattleshipWeb/TestStartGame').subscribe(result => {
      this.gameStarted = result;
    })
    this.tiles = [];
    this.url = baseUrl;
    this.htClient = http;
    this.StartGame();
  }
  public StartGame() {
    this.htClient.get<boolean>(this.url + 'api/BattleshipWeb/TestStartGame').subscribe(result => {
      this.gameStarted = result;
    })
  }
}

interface nodeData {
  tileName: string;
  tileHit: number;
}

