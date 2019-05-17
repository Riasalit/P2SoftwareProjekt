using System;
using System.Collections.Generic;
using System.Drawing;

namespace BattleshipWeb
{
    public class WebUI : IUserInterface
    {
        private List<Ship> ships;
        private Point currentShootingPoint;
        public AI ai { get; private set; }
        private bool recievedShips;
        private bool recievedShootingPoint;
        public bool returnInformationIsReady;
        public bool gameOver{ get; private set; }
        public bool restartGame;
        public bool gotRestartInfo;
        public bool gameTimedOut;
        public int playerWhoWon { get; private set; }
        private int shipIndex;
        private string username;
        public string returnInformation;

        public WebUI(string username)
        {
            ships = new List<Ship>();
            ai = new AI("Strongest AI ever");
            recievedShips = false;
            recievedShootingPoint = false;
            returnInformationIsReady = false;
            gameOver = false;
            gotRestartInfo = false;
            gameTimedOut = false;
            playerWhoWon = 2;
            shipIndex = 0;
            this.username = username;
        }
        public bool GameComplete(Player[] players, int playerWon)
        {
            ai.probabilitiesReady = true; //prevents controller from being stuck
            gameOver = true;
            playerWhoWon = playerWon;
            while (!gotRestartInfo) if (gameTimedOut) return false; //if server times out before receiving user input, just end the game
            return restartGame;
        }
        public Ship GetShips(bool correctlyPlaced, string name)
        {
            while (!recievedShips)
            {
                if (gameTimedOut)
                {
                    ships = new List<Ship> //if game times out before all ships are recieved, then just create some dummy ships to get the game to move on and end
                    {
                        new Ship("Dummy",1, new Point(0,0), 'V'),
                        new Ship("Dummy",1, new Point(0,1), 'V'),
                        new Ship("Dummy",1, new Point(0,2), 'V')
                    };
                    recievedShips = true;
                }
            }
            return ships[shipIndex++];
        }
        public Player[] InitializePlayers(IUserInterface UI)
        {
            Player[] players = new Player[2];
            players[0] = ai;
            players[1] = new Human(username, UI);
            return players;
        }
        public Point MakeTargetPoint(List<Point> points, string name)
        {
            while (!recievedShootingPoint)
            {
                if (gameTimedOut) //if the server times out while waiting for a shot, generate a random valid shot so the gameflow can end
                {
                    Random rand = new Random();
                    currentShootingPoint = new Point(rand.Next(Settings.boardWidth), rand.Next(Settings.boardWidth));
                    while (points.Contains(currentShootingPoint))
                    {
                        currentShootingPoint = new Point(rand.Next(Settings.boardWidth), rand.Next(Settings.boardWidth));
                    }
                    recievedShootingPoint = true;
                }
            }
            recievedShootingPoint = false;

            return currentShootingPoint;
        }

        public void ReturnInformation(Point point, Tile info) //returns "Missed", "Hit a ship" or the name of a sunken ship
        {
            if(info.tile == (int)Tile.TileState.sunk)
            {
                returnInformation = $"{info.GetSunkenShip()}";
            }
            else if(info.tile == (int)Tile.TileState.hit)
            {
                returnInformation = "Hit a ship";
            }
            else
            {
                returnInformation = "Missed";
            }
            returnInformationIsReady = true; //tells the web controller that returninformation is ready
        }
        public void ShipsToUI(Ship ship) //is used by the controller to send ships to ui
        {
            ships.Add(ship);
            if(ships.Count == Settings.shipCount) recievedShips = true; //is true when all ships are received
        }
        public void CoordToUI(Point shootingPoint) //is used by the controller to send shooting point to ui
        {
            currentShootingPoint = shootingPoint;
            recievedShootingPoint = true;
        }
    }
}
