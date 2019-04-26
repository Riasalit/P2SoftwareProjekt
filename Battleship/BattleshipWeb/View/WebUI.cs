using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BattleshipWeb
{
    public class WebUI : IUserInterface
    {
        private string username;
        public WebUI(string username)
        {
            this.username = username;
        }
        public bool GameComplete(Player[] players, int playerWon)
        {
            throw new NotImplementedException();
        }

        public Ship GetShips(bool correctlyPlaced, string name)
        {
            throw new NotImplementedException();
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

        public void ReturnInformation(Point point, string info)
        {
            throw new NotImplementedException();
        }
    }
}
