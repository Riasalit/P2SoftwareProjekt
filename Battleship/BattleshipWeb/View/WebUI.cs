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
        private bool recievedShips;
        private int shipIndex;
        private string username;
        public WebUI(string username)
        {
            ships = new List<Ship>();
            this.username = username;
            recievedShips = false;
            shipIndex = 0;
        }
        public bool GameComplete(Player[] players, int playerWon)
        {
            throw new NotImplementedException();
        }

        public Ship GetShips(bool correctlyPlaced, string name)
        {
            while (!recievedShips) ;
            return ships[shipIndex++];
            
        }

        public Player[] InitializePlayers(IUserInterface UI)
        {
            Player[] players = new Player[2];
            players[0] = new AI("Strongest AI ever");
            players[1] = new Human(username, UI);
            return players;
        }

        public Point MakeTargetPoint(List<Point> points, string name)
        {
            throw new NotImplementedException();
        }

        public void ReturnInformation(Point point, Tile info)
        {
            throw new NotImplementedException();
        }
        public void ShipsToUI(Ship ship)
        {
            ships.Add(ship);
            if(ships.Count == Settings.shipCount) recievedShips = true;
        }
    }
}
