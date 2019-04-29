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
  private currentShip: ShipInfo;
  private firstTileRow: number;
  private firstTileCol: number;
  private firstTileClick: boolean;
  private currentShipLength: number;
  private url: string;
  private htClient: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.url = baseUrl;
    this.htClient = http;
    this.gameState = 0;
    this.firstTileClick = true;
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

  public ChooseShip(ship: ShipInfo) {
    this.currentShipLength = ship.length;
    this.currentShip = ship;
  }

  public ChooseTile(tile: NodeData) {
    tile.tileHit = 3;
    if (this.firstTileClick) {
      this.CalcSecondTiles(tile.tileName);
    } else {
      this.shipInfos.splice(this.shipInfos.indexOf(this.currentShip),1);
      this.SetShipStatesBetweenPoints(tile.tileName);
      for (var i = 0; i < this.tiles.length; i++) {
        for (var j = 0; j < this.tiles[i].length; j++) {
          if (this.tiles[i][j].tileHit == 4) {
            this.tiles[i][j].tileHit = 0;
          }
        }
      }
    }
    this.firstTileClick = !this.firstTileClick;
  }

  private CalcSecondTiles(coord: string) {
    this.firstTileRow = coord.charCodeAt(0) - 65;
    this.firstTileCol = Number.parseInt(coord[1]) - 1;
    for (var i = 0; i < this.tiles.length; i++) {
      for (var j = 0; j < this.tiles[i].length; j++) {
        if (this.tiles[i][j].tileHit == 0) {
          this.tiles[i][j].tileHit = 4;
        }
      }
    }
    this.SetPossibleShipStatesOnTiles(1, 0, this.firstTileRow, this.firstTileCol)
    this.SetPossibleShipStatesOnTiles(-1, 0, this.firstTileRow, this.firstTileCol);
    this.SetPossibleShipStatesOnTiles(0, 1, this.firstTileRow, this.firstTileCol);
    this.SetPossibleShipStatesOnTiles(0, -1, this.firstTileRow, this.firstTileCol);
  }

  private SetPossibleShipStatesOnTiles(xDir: number, yDir: number, xStart: number, yStart: number) {
    var canBePlaced = (xStart + xDir * (this.currentShipLength-1) >= 0 && xStart + xDir * (this.currentShipLength-1) < this.boardSize && yStart + yDir * (this.currentShipLength-1) >= 0 && yStart + yDir * (this.currentShipLength-1) < this.boardSize);
    if (canBePlaced) {
      for (var i = 1; i < this.currentShipLength; i++) {
        if (this.tiles[xStart + xDir * i][yStart + yDir * i].tileHit == 3) {
          console.log("Already ship");
          canBePlaced = false;
        }
      }
    }
    if (canBePlaced) {

        this.tiles[xStart + xDir * (this.currentShipLength-1)][yStart + yDir * (this.currentShipLength-1)].tileHit = 0;
    }
  }

  private SetShipStatesBetweenPoints(coord: string) {
    let secondRow = coord.charCodeAt(0) - 65;
    let secondCol = Number.parseInt(coord[1]) - 1;
    if (secondRow - this.firstTileRow == 0) {
      if (this.firstTileCol > secondCol) {
        for (var i = secondCol + 1; i < this.firstTileCol; i++) {
          this.tiles[secondRow][i].tileHit = 3;
        }
      } else {
        for (var i = this.firstTileCol + 1; i < secondCol; i++) {
          this.tiles[secondRow][i].tileHit = 3;
        }
      }
    } else {
      if (this.firstTileRow > secondRow) {
        for (var i = secondRow + 1; i < this.firstTileRow; i++) {
          this.tiles[i][secondCol].tileHit = 3;
        }
      } else {
        for (var i = this.firstTileRow + 1; i < secondRow; i++) {
          this.tiles[i][secondCol].tileHit = 3;
        }
      }
    }
  }

  public BoolCheck(tile: NodeData) {
    return tile.tileHit == 0;
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

