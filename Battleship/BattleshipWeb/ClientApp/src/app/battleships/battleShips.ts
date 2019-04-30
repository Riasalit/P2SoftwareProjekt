import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { renderTemplate, text } from '@angular/core/src/render3/instructions';
import { forEach } from '@angular/router/src/utils/collection';

@Component({
  selector: 'home',
  templateUrl: './battleships.html'
})

export class BattleshipsComponent {
  public tiles: NodeData[][];
  public shipInfos: ShipInfo[];
  public shootingCoords: ReturnCoords;
  public gameState: number; // {0: not startet, 1: waiting for name}
  public boardSize: number;
  public username: string;
  public sunkenShips: string[];
  private currentShip: ShipInfo;
  private firstTileRow: number;
  private firstTileCol: number;
  private currentShipLength: number;
  private amountOfShipsPlaced: number;
  private url: string;
  private htClient: HttpClient;
  private firstTileClick: boolean;
  private tilesDisabled: boolean;
  private readyForShot: boolean;

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
      for (var i = 0; i < this.shipInfos.length; i++) {
        this.shipInfos[i].isPlaced = false;
      }
    }, error => console.error(error));
    this.sunkenShips = [];
    console.log(this.gameState);
  }

  public StartGame() {
    let htmltext = <HTMLTextAreaElement>document.getElementById("nameField");
    this.username = htmltext.value;
    this.amountOfShipsPlaced = 0;
    this.htClient.post<void>(this.url + 'api/BattleshipWeb/StartGame', this.username).subscribe();
    this.gameState = 1;
    this.tilesDisabled = true;
    this.DrawBoard();
    let tileButtons = document.getElementsByClassName("tileButton");
    for (var i = 0; i < tileButtons.length; i++) {
      tileButtons[i].setAttribute("disabled", "disabled");
    }
  }

  private DrawBoard() {
    this.tiles = [];
    for (var i = 0; i < this.boardSize; i++) {
      this.tiles[i] = []
      for (var j = 0; j < this.boardSize; j++) {
        this.tiles[i][j] = { tileName: String.fromCharCode(65 + i) + (j + 1), tileHit: 0, x: j, y:i};
      }
    }
  }

  public ChooseShip(ship: ShipInfo) {
    this.currentShipLength = ship.length;
    this.currentShip = ship;
    this.tilesDisabled = false;
  }

  public ChooseTile(tile: NodeData) {
    tile.tileHit = 3;
    if (this.firstTileClick) {
      this.CalcSecondTiles(tile);
    } else {
      this.currentShip.isPlaced = true;
      this.tilesDisabled = true;
      this.SetShipStatesBetweenPoints(tile);
      for (var i = 0; i < this.tiles.length; i++) {
        for (var j = 0; j < this.tiles[i].length; j++) {
          if (this.tiles[i][j].tileHit == 4) {
            this.tiles[i][j].tileHit = 0;
          }
        }
      }
      this.amountOfShipsPlaced++;
    }
    this.firstTileClick = !this.firstTileClick;
  }

  private CalcSecondTiles(tile: NodeData) {
    this.firstTileRow = tile.y;
    this.firstTileCol = tile.x;
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
    var canBePlaced = (xStart + xDir * (this.currentShipLength - 1) >= 0 && xStart + xDir * (this.currentShipLength - 1) < this.boardSize && yStart + yDir * (this.currentShipLength - 1) >= 0 && yStart + yDir * (this.currentShipLength - 1) < this.boardSize);
    if (canBePlaced) {
      for (var i = 1; i < this.currentShipLength; i++) {
        if (this.tiles[xStart + xDir * i][yStart + yDir * i].tileHit == 3) {
          console.log("Already ship");
          canBePlaced = false;
        }
      }
    }
    if (canBePlaced) {
      this.tiles[xStart + xDir * (this.currentShipLength - 1)][yStart + yDir * (this.currentShipLength - 1)].tileHit = 0;
    }
  }

  private SetShipStatesBetweenPoints(tile: NodeData) {
    let secondRow = tile.y;
    let secondCol = tile.x;
    if (secondRow - this.firstTileRow == 0) {
      this.currentShip.orientation = "H";
      if (this.firstTileCol > secondCol) {
        this.currentShip.yStart = secondCol;
        this.currentShip.xStart = secondRow;
        this.currentShip
        for (var i = secondCol + 1; i < this.firstTileCol; i++) {
          this.tiles[secondRow][i].tileHit = 3;
        }
      } else {
        this.currentShip.yStart = this.firstTileCol;
        this.currentShip.xStart = this.firstTileRow;
        for (var i = this.firstTileCol + 1; i < secondCol; i++) {
          this.tiles[secondRow][i].tileHit = 3;
        }
      }
    } else {
      this.currentShip.orientation = "V";
      if (this.firstTileRow > secondRow) {
        this.currentShip.yStart = secondCol;
        this.currentShip.xStart = secondRow;
        for (var i = secondRow + 1; i < this.firstTileRow; i++) {
          this.tiles[i][secondCol].tileHit = 3;
        }
      } else {
        this.currentShip.yStart = this.firstTileCol;
        this.currentShip.xStart = this.firstTileRow;
        for (var i = this.firstTileRow + 1; i < secondRow; i++) {
          this.tiles[i][secondCol].tileHit = 3;
        }
      }
    }
  }

  public resetBoard() {
    for (var i = 0; i < this.boardSize; i++) {
      for (var j = 0; j < this.boardSize; j++) {
        this.tiles[i][j].tileHit = 0;
      }
    }
    for (var i = 0; i < this.shipInfos.length; i++) {
      this.shipInfos[i].isPlaced = false;
    }
    this.amountOfShipsPlaced = 0;
  }

  public resetShip(ship: ShipInfo) {
    ship.isPlaced = false;
    for (var i = 0; i < ship.length; i++) {
      if (ship.orientation == "H") {
        this.tiles[ship.xStart][ship.yStart + i].tileHit = 0;
      } else {
        this.tiles[ship.xStart + i][ship.yStart].tileHit = 0;
      }
    }
    this.amountOfShipsPlaced--;
  }

  public BoolCheck(tile: NodeData) {
    return tile.tileHit == 0;
  }

  public ShipsEntered() {
    this.gameState = 2;
    this.readyForShot = true;
    for (var i = 0; i < this.tiles.length; i++) {
      for (var j = 0; j < this.tiles[i].length; j++) {
        this.tiles[i][j].tileHit = 0;
      }
    }
    for (var i = 0; i < this.shipInfos.length; i++) {
      console.log(this.shipInfos[i]);
      this.htClient.post<void>(this.url + 'api/BattleshipWeb/SendShips', this.shipInfos[i]).subscribe();
    }
  }

  public ShootAtTile(tile: NodeData) {
    this.readyForShot = false;
    this.shootingCoords = { x: tile.x, y: tile.y };
    this.htClient.post<string>(this.url + 'api/BattleshipWeb/SendShootingCoords', this.shootingCoords, {responseType: 'text' as 'json'}).subscribe(result => {
      console.log(result);
      if (result == 'Missed') {
        tile.tileHit = 1;
      } else if (result == 'Hit a ship') {
        tile.tileHit = 2;
      } else {
        tile.tileHit = 2;
        this.sunkenShips.push(result);
      }
      this.readyForShot = true;
    }, error => console.error(error));
  }
}

interface ReturnCoords {
  x: number;
  y: number; 
}

interface NodeData {
  tileName: string;
  tileHit: number;
  x: number;
  y: number;
}

interface ShipInfo {
  name: string;
  length: number;
  xStart: number;
  yStart: number;
  orientation: string;
  isPlaced: boolean; 
}
interface ShootingResult {
  result: string;
}

