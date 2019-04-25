using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BattleshipWeb.View
{
    public class WebUI : IUserInterface
    {
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
            throw new NotImplementedException();
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
