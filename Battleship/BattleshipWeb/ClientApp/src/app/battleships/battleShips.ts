import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { renderTemplate, text } from '@angular/core/src/render3/instructions';
import { forEach } from '@angular/router/src/utils/collection';

@Component({
  selector: 'Battleships',
  templateUrl: './battleships.html'
})




export class BattleshipsComponent {
  public aiTargetBoard: HumanBoardAndProb[][];
  public tiles: NodeData[][];
  public shipInfos: ShipInfo[];
  public shootingCoords: ReturnCoords;
  public playerWhoWon: number;
  public gameState: number; // {0: get username, 1: place ships, 2: game running}
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
  private showProbabilities: boolean;
  private gameIsReady: boolean;
  private turnCounter: number;
  private resetTimeLeft: number;
  private playerAFK: boolean;
  private audio: HTMLAudioElement;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.turnCounter = 0;
    this.url = baseUrl;
    this.htClient = http;
    this.gameState = 0;
    this.firstTileClick = true;
    this.resetTimeLeft = 120; // 120 sec = 2 min
    this.playerAFK = false;
    this.gameIsReady = false;
    this.PlayAudio(0);
    this.audio.loop = true;

    //while (this.serverBusy && this.callDone) {
    let timeLeft = 5;
    let interval = setInterval(() => {
      if (timeLeft > 0) {
        timeLeft--;
      } else {
        http.get<boolean>(baseUrl + 'api/BattleshipWeb/GetGameRunning').subscribe(result => {
          console.log(result.valueOf());
          if (result) {
            timeLeft = 5;
          } else {
            http.get<number>(baseUrl + 'api/BattleshipWeb/StartData').subscribe(result => {
              this.boardSize = result;
            }, error => console.error(error));
            this.htClient.get<ShipInfo[]>(this.url + 'api/BattleshipWeb/GetShipnamesAndLengths').subscribe(result => {
              this.gameState = 0;
              this.shipInfos = result;
              for (var i = 0; i < this.shipInfos.length; i++) {
                this.shipInfos[i].isPlaced = false;
              }
            }, error => console.error(error));
            this.StartAFKTimer();
            this.gameIsReady = true;
            clearInterval(interval);
          }
        })
      }
    }, 1000) //1000 milisec = 1 sec


    this.sunkenShips = [];
  }

  public StartGame() {
    this.resetTimeLeft = 120;
    let htmltext = <HTMLInputElement>document.getElementById("name");
    this.username = htmltext.value;
    this.amountOfShipsPlaced = 0;
    this.htClient.post<void>(this.url + 'api/BattleshipWeb/StartGame', this.username).subscribe(error => console.error(error));
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
        this.tiles[i][j] = { tileName: String.fromCharCode(65 + i) + (j + 1), tileHit: 0, x: j, y: i };
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
    this.audio.pause();
    this.resetTimeLeft = 120;
    this.gameState = 2;
    this.readyForShot = true;
    for (var i = 0; i < this.tiles.length; i++) {
      for (var j = 0; j < this.tiles[i].length; j++) {
        this.tiles[i][j].tileHit = 0;
      }
    }
    for (var i = 0; i < this.shipInfos.length; i++) {
      this.htClient.post<void>(this.url + 'api/BattleshipWeb/SendShips', this.shipInfos[i]).subscribe();
    }
    this.htClient.get<HumanBoardAndProb[][]>(this.url + 'api/BattleshipWeb/getHumanBoardAndProb').subscribe(result => {
      this.aiTargetBoard = result;
    }, error => console.error(error));
  }

  public ShootAtTile(tile: NodeData) {
    this.resetTimeLeft = 120;
    this.turnCounter++;
    this.readyForShot = false;
    this.shootingCoords = { x: tile.x, y: tile.y };
    this.htClient.post<string>(this.url + 'api/BattleshipWeb/SendShootingCoords', this.shootingCoords, { responseType: 'text' as 'json' }).subscribe(result => {
      if (result == 'Missed') {
        tile.tileHit = 1;
        this.PlayAudio(1);
      } else if (result == 'Hit a ship') {
        tile.tileHit = 2;
        this.PlayAudio(2)
      } else {
        tile.tileHit = 2;
        this.sunkenShips.push(result);
        this.PlayAudio(3);
      }
    }, error => console.error(error));
    this.htClient.get<HumanBoardAndProb[][]>(this.url + 'api/BattleshipWeb/getHumanBoardAndProb').subscribe(result => {
      this.aiTargetBoard = result;
      this.htClient.get<GameOverInfo>(this.url + 'api/BattleshipWeb/GetGameOverInfo').subscribe(result => {
        console.log(result.gameOver);
        if (result.gameOver) {
          this.playerWhoWon = result.playerWhoWon;
          this.gameState = 3;
        } else {
          this.readyForShot = true;
        }
      }, error => console.error(error));
    }, error => console.error(error));
  }
  public isShipOnTile(tile: HumanBoardAndProb) {
    let isShip = false;
    for (var i = 0; i < this.shipInfos.length; i++) {
      for (var j = 0; j < this.shipInfos[i].length; j++) {
        if (this.shipInfos[i].orientation == 'H') {
          if (tile.x == this.shipInfos[i].yStart + j && tile.y == this.shipInfos[i].xStart) {
            isShip = true;
          }
        } else {
          if (tile.y == this.shipInfos[i].xStart + j && tile.x == this.shipInfos[i].yStart) {
            isShip = true;
          }
        }
      }
    }
    if (isShip) {
      return "rgb(93, 93, 93)";
    } else {
      return "#436B98";
    }

  }
  public CalculateProbabilities(value: number) {
    let newVal = value * 100;
    return newVal.toFixed(0) + "%";
  }

  public MakebackgroundColor(button: HTMLButtonElement, probability: number) {
    if (probability < (1/3)) {
      return "rgb(180," + 180 * (3 * probability) + ",0)";
    } else {
      return "rgb(" + (180 - (180*(probability-(1/3))*(3))) + ",180,0)";
    }
    
  }

  public GetPlayerWhoWon() {
    if (this.playerWhoWon == 0) {
      this.PlayAudio(5);
      return "The best AI ever :3 ";
    } else {
      this.PlayAudio(4);
      return "" + name;
    }
  }

  public RestartOrEndGame(restart: boolean) {
    this.resetTimeLeft = 120;
    this.htClient.post<void>(this.url + 'api/BattleshipWeb/RestartOrEndGame', restart).subscribe();
    this.audio.pause();
    if (restart) {
      this.ResetClient();
    } else {
      window.location.href = this.url;
    }
  }

  public PrintImage(tile: HumanBoardAndProb) {
    console.log(this.CalculateProbabilities(tile.probability));
    if (tile.probability == 1) {
      return "../../assets/Images/green_dot.png";
    } else {
      return "../../assets/Images/red_dot.png"
    }
  }


  private ResetClient() {
    this.htClient.post<void>(this.url + 'api/BattleshipWeb/StartGame', this.username).subscribe();
    this.turnCounter = 0;
    this.gameState = 1;
    this.sunkenShips.splice(0, this.sunkenShips.length);
    this.firstTileClick = true;
    this.htClient.get<ShipInfo[]>(this.url + 'api/BattleshipWeb/GetShipnamesAndLengths').subscribe(result => {
      this.shipInfos = result;
      for (var i = 0; i < this.shipInfos.length; i++) {
        this.shipInfos[i].isPlaced = false;
      }
    }, error => console.error(error));
    this.amountOfShipsPlaced = 0;
    this.tilesDisabled = true;
    this.DrawBoard();
  }

  private StartAFKTimer() {

    let afkTimer = setInterval(() => {
      if (this.resetTimeLeft <= 0) {
        this.playerAFK = true;
        this.resetTimeLeft = 120
      } else {
        this.resetTimeLeft--;
      }
    }, 1000)
  }

  public Refresh() {
    window.location.reload();
  }

  public PlayAudio(sound: number) {
    this.audio = new Audio();
    if (sound == 0) { //intro
      this.audio.src = "../assets/SoundEffects/IntroSound.wav";
    } else if (sound == 1) { //Hit
      this.audio.src = "../assets/SoundEffects/Hit.wav";
    } else if (sound == 2) { //Miss
      this.audio.src = "../assets/SoundEffects/Miss.wav";
    } else if (sound == 3) { //Sunk
      this.audio.src = "../assets/SoundEffects/Sunk.wav";
    } else if (sound == 4) { //Game Won
      this.audio.src = "../assets/SoundEffects/GladVictory.wav";
    } else if (sound == 5) {
      this.audio.src = "../assets/SoundEffects/SadVictory.wav";
    }
    this.audio.load();
    this.audio.loop = false;
    this.audio.play();
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

interface HumanBoardAndProb {
  tileShot: boolean;
  probability: number;
  x: number;
  y: number;
}

interface GameOverInfo {
  playerWhoWon: number;
  gameOver: boolean;
}
