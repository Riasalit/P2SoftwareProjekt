import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { renderTemplate } from '@angular/core/src/render3/instructions';

@Component({
  selector: 'home',
  templateUrl: './battleships.html'
})

export class BattleshipsComponent {
  public tiles: NodeData[][];
  public shipInfos: ShipInfo[];
  public gameState: number; // {0: not startet, 1: waiting for name}
  public boardSize: number;
  public username: string;
  private firstShipPos: boolean;
  private currentShipLength: number;
  private url: string;
  private htClient: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.url = baseUrl;
    this.htClient = http;
    this.gameState = 0;
    this.firstShipPos = true;
    http.get<number>(baseUrl + 'api/BattleshipWeb/StartData').subscribe(result => {
      console.log(result);
      this.boardSize = result;
    }, error => console.error(error));
    this.htClient.get<ShipInfo[]>(this.url + 'api/BattleshipWeb/GetShipnamesAndLengths').subscribe(result => {
      this.shipInfos = result;
    }, error => console.error(error));
    console.log(this.gameState);
  }
  public StartGame() {

    let htmltext: HTMLTextAreaElement;
    htmltext = <HTMLTextAreaElement>document.getElementById("nameField");
    this.username = htmltext.value;
    this.htClient.post<void>(this.url + 'api/BattleshipWeb/StartGame', this.username).subscribe();
    this.gameState = 1;
    console.log(this.gameState);

    
    this.DrawBoard();
  }


  private DrawBoard() {
    this.tiles = [];
    for (var i = 0; i < this.boardSize; i++) {
      this.tiles[i] = []
      for (var j = 0; j < this.boardSize; j++) {
        this.tiles[i][j] = { tileName: String.fromCharCode(65 + i) + (j + 1), tileHit: 0 };
        console.log(this.tiles[i][j].tileName);
      }
    }
  }

  public ChooseShip(length: number) {
    this.currentShipLength = length;
  }

  public ChooseTile(coord: string) {
    if (this.firstShipPos) {
      this.CalcSecondTiles(coord);
    } else {

    }
    this.firstShipPos = !this.firstShipPos;
  }

  private CalcSecondTiles(coord: string) {

  }


}

interface NodeData {
  tileName: string;
  tileHit: number;
}

interface ShipInfo {
  name: string;
  length: number;
}

