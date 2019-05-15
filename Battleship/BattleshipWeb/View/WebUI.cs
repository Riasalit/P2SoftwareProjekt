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
            ai.probabilitiesReady = true;
            gameOver = true;
            playerWhoWon = playerWon;
            while (!gotRestartInfo) if (gameTimedOut) return false;
            return restartGame;
        }
        public Ship GetShips(bool correctlyPlaced, string name)
        {
            while (!recievedShips)
            {
                if (gameTimedOut)
                {
                    ships = new List<Ship>
                    {
                        new Ship("Cruiser",1, new Point(0,0), 'V'),
                        new Ship("Cruiser",1, new Point(0,1), 'V'),
                        new Ship("Cruiser",1, new Point(0,2), 'V')
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
                if (gameTimedOut)
                {
                    Random rand = new Random();
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
        public void ReturnInformation(Point point, Tile info)
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
            returnInformationIsReady = true;
        }
        public void ShipsToUI(Ship ship)
        {
            ships.Add(ship);

            if (ships.Count == Settings.shipCount)
            {
                recievedShips = true;
            }
        }
        public void CoordToUI(Point shootingPoint)
        {
            currentShootingPoint = shootingPoint;
            recievedShootingPoint = true;
        }
    }
}
