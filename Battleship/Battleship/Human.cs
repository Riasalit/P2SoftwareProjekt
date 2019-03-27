using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Human : Player
    {
        public Human(string name) : base(name)
        {

        }
        public override void YourTurn()
        {
            

        }
        public override void SetShips(IUserInterface UI)
        {
            //Creates an UI
            Ship ship;
            Char orientation;
            int x, y;
            bool correctlyPlaced = false;
            //Places five ships
            for (int i = 0; i < 5; i++)
            {
                correctlyPlaced = false;
                while (!correctlyPlaced)
                {
                    UI.InteractWithUser();
                    ship = UI.GetShip();
                    Char[] Data = UI.GetData();
                    orientation = Data[0];
                    x = Data[1] - '0'; 
                    y = Data[2] - '0'; 
                    correctlyPlaced = board.PlaceShips(ship, orientation, x, y);
                }
                
            }
            

        }
    }
}
