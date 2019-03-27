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
        public override void SetShips()
        {
            IUserInterface UI = new TestUI();
            Ship shipToPlace = new Ship("Anton", 3);
            Char placementOrientation = 'H';
            int x = 0, y = 0;
            bool placementSucces = false;
            for(int i = 0; i<5; i++)
            {
                placementSucces = false;
                while (!placementSucces)
                {
                    UI.InterActWithUser();
                    shipToPlace = UI.GetShip();
                    Char[] Data = UI.GetData();
                    placementOrientation = Data[0];
                    x = Data[1] - '0'; //jeg er grim
                    y = Data[2] - '0'; //same though
                    placementSucces = board.PlaceShips(shipToPlace, placementOrientation, x, y);
                }
                
            }
            

        }
    }
}
