import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { renderTemplate, text } from '@angular/core/src/render3/instructions';
import { forEach } from '@angular/router/src/utils/collection';

@Component({
  selector: 'Battleships',
  templateUrl: './battleships.html'
})


  /**************************************************************
   * A lot of the coordinates have been swapped between x and y *
   * before and after server communication because of arrays   *
   * ************************************************************/


export class BattleshipsComponent {
  private aiTargetBoard: HumanBoardAndProb[][];
  private tiles: TileData[][];
  private shipInfos: ShipInfo[]; //lengths and names
  private shootingCoords: ReturnCoords;
  private playerWhoWon: number;
  private gameState: number; //{0: get username, 1: place ships, 2: game running}
  private boardSize: number;
  private username: string;
  private sunkenShips: string[];
  private currentShip: ShipInfo; //ship currently being placed
  private firstTileRow: number; //used for placing ships
  private firstTileCol: number; //used for placing ships
  private firstTileClick: boolean;
  private currentShipLength: number; //length of the ship currently being placed
  private amountOfShipsPlaced: number;
  private url: string;
  private htClient: HttpClient;
  private tilesDisabled: boolean;
  private readyForShot: boolean; //server has answered and is ready to receive a new shot coord
  private showProbabilities: boolean;
  private gameIsReady: boolean; // the client has received all startup info fromn the server
  private turnCounter: number;
  private resetTimeLeft: number; //time left before reset
  private playerAFK: boolean;
  private audio: HTMLAudioElement;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.turnCounter = 0;
    this.url = baseUrl;
    this.htClient = http;
    this.gameState = 0;
    this.firstTileClick = true;
    this.resetTimeLeft = 120;// 120 sec = 2 min
    this.playerAFK = false;
    this.gameIsReady = false;
    this.PlayAudio(0);
    this.audio.loop = true;
    
    let interval = setInterval(() => { //starts an asynchronous function that runs once every second
      http.get<boolean>(baseUrl + 'api/BattleshipWeb/GetGameRunning').subscribe(result => {
        if (!result) { //result is true when the server is busy
          this.SetupGame(baseUrl, http);
          clearInterval(interval); //ends the asynchronous function
        }
      }, error => console.error(error)); //logs error from requests if any was received
    }, 5000) //1000 milisec = 1 sec
    this.sunkenShips = [];
  }

  private SetupGame(baseUrl:string, http:HttpClient) {
    http.get<number>(baseUrl + 'api/BattleshipWeb/StartData').subscribe(result => { //requests board size from server
      this.boardSize = result;
    }, error => console.error(error)); 
    this.htClient.get<ShipInfo[]>(this.url + 'api/BattleshipWeb/GetShipNamesAndLengths').subscribe(result => { //requests the ships from the server
      this.gameState = 0;
      this.shipInfos = result;
      for (var i = 0; i < this.shipInfos.length; i++) {
        this.shipInfos[i].isPlaced = false;
      }
    }, error => console.error(error));
    this.StartAFKTimer(); //starts the clientside afk timer
    this.gameIsReady = true;
  }

  public StartGame() {
    this.resetTimeLeft = 120;
    let htmltext = <HTMLInputElement>document.getElementById("name"); //gets access to the input field for a username from the html page
    this.username = htmltext.value; //saves the userinput as their username
    this.amountOfShipsPlaced = 0;
    this.htClient.post<void>(this.url + 'api/BattleshipWeb/StartGame', this.username)
                                            .subscribe(error => console.error(error)); //sends a request to the server for starting the game
    this.gameState = 1; 
    this.tilesDisabled = true; //prevents the user from clicking the tiles before choosing a ship
    this.CreateAllTiles();
  }

  private CreateAllTiles() {
    this.tiles = [];
    for (var i = 0; i < this.boardSize; i++) {
      this.tiles[i] = [];
      for (var j = 0; j < this.boardSize; j++) {
        this.tiles[i][j] = { tileState: 0, x: j, y: i }; //creates all the tiles as interfaces
      }
    }
  }

  public ChooseShip(ship: ShipInfo) { //gets input from HTML
    this.currentShipLength = ship.length;
    this.currentShip = ship;
    this.tilesDisabled = false;
  }

  public ChooseTile(tile: TileData) { //gets input from HTML
    tile.tileState = 3; //look at tileData to see tilestate info
    if (this.firstTileClick) {
      this.CalcSecondTiles(tile); //calculates tiles that can be chosen as ship end point
    } else {
      this.currentShip.isPlaced = true;
      this.tilesDisabled = true;
      this.SetShipStatesBetweenPoints(tile); 
      for (var i = 0; i < this.tiles.length; i++) {
        for (var j = 0; j < this.tiles[i].length; j++) {
          if (this.tiles[i][j].tileState == 4) {
            this.tiles[i][j].tileState = 0; //resets tiles where a ship isnt placed
          }
        }
      }
      this.amountOfShipsPlaced++;
    }
    this.firstTileClick = !this.firstTileClick; //flip boolean
  }

  private CalcSecondTiles(tile: TileData) {
    this.firstTileRow = tile.y;
    this.firstTileCol = tile.x;
    for (var i = 0; i < this.tiles.length; i++) {
      for (var j = 0; j < this.tiles[i].length; j++) {
        if (this.tiles[i][j].tileState == 0) {
          this.tiles[i][j].tileState = 4;
        }
      }
    }
    this.SetPossibleShipStatesOnTiles(1, 0, tile.y, tile.x)
    this.SetPossibleShipStatesOnTiles(-1, 0, tile.y, tile.x);
    this.SetPossibleShipStatesOnTiles(0, 1, tile.y, tile.x);
    this.SetPossibleShipStatesOnTiles(0, -1, tile.y, tile.x);
  }

  private SetPossibleShipStatesOnTiles(xDir: number, yDir: number, xStart: number, yStart: number) { //set tiles where a ships end points can be to clickable
    var canBePlaced = (xStart + xDir * (this.currentShipLength - 1) >= 0 &&
                       xStart + xDir * (this.currentShipLength - 1) < this.boardSize &&
                       yStart + yDir * (this.currentShipLength - 1) >= 0 &&
                       yStart + yDir * (this.currentShipLength - 1) < this.boardSize); //checks that a tile is inside all the boards borders
    if (canBePlaced) {
      for (var i = 1; i < this.currentShipLength; i++) {
        if (this.tiles[xStart + xDir * i][yStart + yDir * i].tileState == 3) { //checks if a ship is between the clicked tile and potential second tile
          canBePlaced = false;
        }
      }
    }
    if (canBePlaced) {
      this.tiles[xStart + xDir * (this.currentShipLength - 1)][yStart + yDir * (this.currentShipLength - 1)].tileState = 0; //if both of the above checks passed, make the tile clickabel
    }
  }

  private SetShipStatesBetweenPoints(tile: TileData) {
    let secondRow = tile.y;
    let secondCol = tile.x;
    if (secondRow - this.firstTileRow == 0) { //if the two ycoords subtracted is zero the ship must be horizontal
      this.currentShip.orientation = "H";
      if (this.firstTileCol > secondCol) { //as we need to get the tile with the lowest coords for the server we set those into the ships
        this.currentShip.yStart = secondCol;
        this.currentShip.xStart = secondRow;
        this.currentShip
        for (var i = secondCol + 1; i < this.firstTileCol; i++) {
          this.tiles[secondRow][i].tileState = 3; //sets all tiles between first and second to 3 (a ship)
        }
      } else { //if second column wasnt the lowest then the first one must be 
        this.currentShip.yStart = this.firstTileCol;
        this.currentShip.xStart = this.firstTileRow;
        for (var i = this.firstTileCol + 1; i < secondCol; i++) {
          this.tiles[secondRow][i].tileState = 3;
        }
      }
    } else { //if the ship isnt horizontal it must be vertical
      this.currentShip.orientation = "V";
      if (this.firstTileRow > secondRow) {
        this.currentShip.yStart = secondCol;
        this.currentShip.xStart = secondRow;
        for (var i = secondRow + 1; i < this.firstTileRow; i++) {
          this.tiles[i][secondCol].tileState = 3;
        }
      } else {
        this.currentShip.yStart = this.firstTileCol;
        this.currentShip.xStart = this.firstTileRow;
        for (var i = this.firstTileRow + 1; i < secondRow; i++) {
          this.tiles[i][secondCol].tileState = 3;
        }
      }
    }
  }

  public resetBoard() {  // from ship placing state
    for (var i = 0; i < this.boardSize; i++) {
      for (var j = 0; j < this.boardSize; j++) {
        this.tiles[i][j].tileState = 0; //removes all ships from the board
      }
    }
    for (var i = 0; i < this.shipInfos.length; i++) {
      this.shipInfos[i].isPlaced = false; //unplaces the ships
    }
    this.amountOfShipsPlaced = 0;
    this.firstTileClick = true;
    this.tilesDisabled = true; // must choose a ship before clicking a tile
  }

  public resetShip(ship: ShipInfo) { //resets a single ship, gets ship from HTML
    ship.isPlaced = false;
    for (var i = 0; i < ship.length; i++) {
      if (ship.orientation == "H") { //removes a ship in the dirction its placed from its start coords
        this.tiles[ship.xStart][ship.yStart + i].tileState = 0;
      } else {
        this.tiles[ship.xStart + i][ship.yStart].tileState = 0;
      }
    }
    this.amountOfShipsPlaced--;
  }

  public ShipsEntered() {
    this.audio.pause(); //stop epic start screen music
    this.resetTimeLeft = 120;
    this.gameState = 2;
    this.readyForShot = true;
    for (var i = 0; i < this.tiles.length; i++) {
      for (var j = 0; j < this.tiles[i].length; j++) {
        this.tiles[i][j].tileState = 0; //resets all tilestates
      }
    }
    for (var i = 0; i < this.shipInfos.length; i++) {
      this.htClient.post<void>(this.url + 'api/BattleshipWeb/SendShips', this.shipInfos[i]).subscribe(); //send all ships to the server
    }
    this.htClient.get<HumanBoardAndProb[][]>(this.url + 'api/BattleshipWeb/getHumanBoardAndProb').subscribe(result => { //gets the ai board probability distribution
      this.aiTargetBoard = result;
    }, error => console.error(error));
  }

  public ShootAtTile(tile: TileData) {
    this.resetTimeLeft = 120;
    this.turnCounter++;
    this.readyForShot = false;
    this.shootingCoords = { x: tile.x, y: tile.y }; //create interface instance of a ReturnCoord
    // following post method sends the coord the user wants to shoot at to the server, and gets a response for whether or not the user hit something or a shipname if the user sunk a ship
    this.htClient.post<string>(this.url + 'api/BattleshipWeb/SendShootingCoords', this.shootingCoords, { responseType: 'text' as 'json' }).subscribe(result => {
      if (result == 'Missed') {
        tile.tileState = 1;
        this.PlayAudio(1); //splash sound
      } else if (result == 'Hit a ship') {
        tile.tileState = 2;
        this.PlayAudio(2) //smol BOOOM
      } else {
        tile.tileState = 2;
        this.sunkenShips.push(result); //adds the name of the sunken ship to a list of names for sunken ships
        this.PlayAudio(3); //big BOOOOOOOM!!!
      }
    }, error => console.error(error));
    this.htClient.get<HumanBoardAndProb[][]>(this.url + 'api/BattleshipWeb/getHumanBoardAndProb').subscribe(result => { //gets the ai board probability distribution
      this.aiTargetBoard = result;
      this.htClient.get<GameOverInfo>(this.url + 'api/BattleshipWeb/GetGameOverInfo').subscribe(result => { //ask the server if the game is done
        console.log(result.gameOver);
        if (result.gameOver) {
          this.playerWhoWon = result.playerWhoWon; //player who won { 0 = AI, 1 = user }
          if (result.playerWhoWon == 0) {
            this.PlayAudio(5); //sad victory
          } else {
            this.PlayAudio(4); //glorious victory
          }
          this.gameState = 3;
        } else {
          this.readyForShot = true;
        }
      }, error => console.error(error));
    }, error => console.error(error));
  }

  public isShipOnTile(tile: HumanBoardAndProb) { //makes users board which the ai shoots at blue if the tile doesnt contain a tile and gray if it does
    for (var i = 0; i < this.shipInfos.length; i++) { //go through all ships
      for (var j = 0; j < this.shipInfos[i].length; j++) { //go through all tiles in a ship
        if (this.shipInfos[i].orientation == 'H') {
          if (tile.x == this.shipInfos[i].yStart + j && tile.y == this.shipInfos[i].xStart) {
            return "rgb(93, 93, 93)"; //grey
          }
        } else {
          if (tile.y == this.shipInfos[i].xStart + j && tile.x == this.shipInfos[i].yStart) {
            return "rgb(93, 93, 93)"; //grey
          }
        }
      }
    }
   return "#436B98"; //blue
  }

  public CalculateProbabilities(value: number) { //converts probabilities from a probability value to a percentage
    let newVal = value * 100;
    return newVal.toFixed(0) + "%";
  }

  public MakeBackgroundColor(button: HTMLButtonElement, probability: number) {
    if (probability < (1/3)) {
      return "rgb(180," + 180 * (3 * probability) + ",0)"; //red to yellow
    } else {
      return "rgb(" + (180 - (180*(probability-(1/3))*(3))) + ",180,0)"; //yellow to green
    } //the above algorithm is standard for mapping two value ranges
  }

  public GetPlayerWhoWon() {
    if (this.playerWhoWon == 0) {
      return "The AI";
    } else {
      return this.username;
    }
  }

  public RestartOrEndGame(restart: boolean) {
    this.htClient.post<void>(this.url + 'api/BattleshipWeb/RestartOrEndGame', restart).subscribe();
    this.audio.pause(); //stops victory music
    if (restart) {
      this.ResetClient();
    } else {
      this.Refresh(); //refreshes browser page
    }
  }

  public PrintImage(tile: HumanBoardAndProb) { //if ai hit make a green dot, if it missed make a red
    console.log(this.CalculateProbabilities(tile.probability));
    if (tile.probability == 1) {
      return "../../assets/Images/green_dot.png";
    } else {
      return "../../assets/Images/red_dot.png"
    }
  }


  private ResetClient() { //and start the server again
    this.resetTimeLeft = 120;
    this.htClient.post<void>(this.url + 'api/BattleshipWeb/StartGame', this.username).subscribe(); //sends username to server again
    this.turnCounter = 0;
    this.gameState = 1;
    this.sunkenShips.splice(0, this.sunkenShips.length);
    this.firstTileClick = true;
    this.htClient.get<ShipInfo[]>(this.url + 'api/BattleshipWeb/GetShipNamesAndLengths').subscribe(result => {
      this.shipInfos = result;
      for (var i = 0; i < this.shipInfos.length; i++) {
        this.shipInfos[i].isPlaced = false;
      }
    }, error => console.error(error));
    this.amountOfShipsPlaced = 0;
    this.tilesDisabled = true;
    this.CreateAllTiles();
  }

  private StartAFKTimer() {

    let afkTimer = setInterval(() => {
      if (this.resetTimeLeft <= 0) {
        this.playerAFK = true;
        clearInterval(afkTimer);
      } else {
        this.resetTimeLeft--;
      }
    }, 1000)
  }

  public Refresh() {
    window.location.reload(); //reloads page
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
    } else if (sound == 5) { //Game Lost (sad victory)
      this.audio.src = "../assets/SoundEffects/SadVictory.wav";
    }
    this.audio.load();
    this.audio.loop = false;
    this.audio.play();
  }

}

interface ReturnCoords { //shooting coords
  x: number;
  y: number;
}

interface TileData {
  tileState: number; //{0 = clickable/unknown, 1 = hit and is ship, 2 = hit and is water, 3 = placed 4ship, 4 = not able to place ship here}
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
