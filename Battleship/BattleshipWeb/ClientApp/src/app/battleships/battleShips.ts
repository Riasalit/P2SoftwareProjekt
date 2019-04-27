import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { renderTemplate } from '@angular/core/src/render3/instructions';
import { forEach } from '@angular/router/src/utils/collection';

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
    this.DrawBoard();
  }

  private DrawBoard() {
    this.tiles = [];
    for (var i = 0; i < this.boardSize; i++) {
      this.tiles[i] = []
      for (var j = 0; j < this.boardSize; j++) {
        this.tiles[i][j] = { tileName: String.fromCharCode(65 + i) + (j + 1), tileHit: 0 };
      }
    }
  }

  public ChooseShip(length: number) {
    this.currentShipLength = length;
  }

  public ChooseTile(tile: NodeData) {
    if (this.firstShipPos) {
      tile.tileHit = 3;
      this.CalcSecondTiles(tile.tileName);
    } else {

    }
    this.firstShipPos = !this.firstShipPos;
  }

  private CalcSecondTiles(coord: string) {
    let baseRow = coord.charCodeAt(0) - 65;
    let baseCol = Number.parseInt(coord[1]);
    for (var i = 0; i < this.tiles.length; i++) {
      for (var j = 0; j < this.tiles[i].length; j++) {
        let tempFullname = this.tiles[i][j].tileName;
        let tempRow = tempFullname.charCodeAt(0) - 65;
        let tempCol = Number.parseInt(tempFullname[1]);
        if (!this.testTiles(baseRow, tempRow, baseCol, tempCol)) {
          this.tiles[i][j].tileHit = 3;
        }
      }
    }
  }

  private testTiles(baseRow: Number, tempRow: Number, baseCol: Number, tempCol: Number) {

    return true;
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

