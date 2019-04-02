using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace Battleship
{
    public class Human : Player
    {
        private IUserInterface UI;
        public Human(string name, IUserInterface UI) : base(name)
        {
            this.UI = UI;
        }
        public override void YourTurn()
        {
            Point target = UI.MakeTargetPoint(points);
            points.Add(target);
            UI.ReturnInformation(ShootOpponent(target));
        }
        public override void SetShips()
        {
            Ship ship;
            bool correctlyPlaced = false;
            bool lastShipCorrectlyPlaced = false;
            //Places five ships
            for (int i = 0; i < 5; i++)
            {
                correctlyPlaced = false;
                while (!correctlyPlaced)
                {
                    ship = UI.GetShips(lastShipCorrectlyPlaced);
                    correctlyPlaced = board.PlaceShips(ship);
                    lastShipCorrectlyPlaced = correctlyPlaced;
                }          
            }
        }
    }
}
