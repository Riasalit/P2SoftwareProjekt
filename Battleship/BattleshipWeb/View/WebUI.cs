using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BattleshipWeb
{
    public class WebUI : IUserInterface
    {
        private List<Ship> ships;
        private Point currentShootingPoint;
        private bool recievedShips;
        private bool recievedShootingPoint;
        private int shipIndex;
        private string username;
        public bool returnInformationIsReady;
        public bool gameOver{ get; private set; }
        public bool restartGame;
        public bool gotRestartInfo;
        public int playerWhoWon { get; private set; }
        public string returnInformation;
        public AI ai { get; private set; }
        public WebUI(string username)
        {
            gotRestartInfo = false;
            gameOver = false;
            playerWhoWon = 2;
            ai = new AI("Strongest AI ever");
            ships = new List<Ship>();
            this.username = username;
            returnInformationIsReady = false;
            recievedShootingPoint = false;
            recievedShips = false;
            shipIndex = 0;
        }
        public bool GameComplete(Player[] players, int playerWon)
        {
            gameOver = true;
            playerWhoWon = playerWon;
            while (!gotRestartInfo) ;
            return restartGame;
        }

        public Ship GetShips(bool correctlyPlaced, string name)
        {
            while (!recievedShips) ;
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
            while (!recievedShootingPoint) ;
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
            if(ships.Count == Settings.shipCount) recievedShips = true;
        }
        public void CoordToUI(Point shootingPoint)
        {
            currentShootingPoint = shootingPoint;
            recievedShootingPoint = true;
        }
    }
}
