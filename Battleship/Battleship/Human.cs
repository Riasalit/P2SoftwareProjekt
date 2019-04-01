using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            

        }
        public override void SetShips()
        {
            Ship ship;
            bool correctlyPlaced = false;
            //Places five ships
            for (int i = 0; i < 5; i++)
            {
                correctlyPlaced = false;
                while (!correctlyPlaced)
                {
                    ship = UI.GetShips();
                    correctlyPlaced = board.PlaceShips(ship);
                }          
            }
        }
    }
}
