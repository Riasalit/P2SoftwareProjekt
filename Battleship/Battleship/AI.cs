using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class AI : Player
    {
        public AI(string name) : base(name)
        {
            
        }
        public override void YourTurn()
        {

        }
        public override void SetShips()
        {
            int x, y, orientation;
            List<Ship> ships = new List<Ship>
            {
                new Ship("Destroyer", 2),
                new Ship("Submarine", 3),
                new Ship("Cruiser", 3),
                new Ship("Battleship", 4),
                new Ship("Carrier", 5)
            };

            foreach(Ship ship in ships)
            {
                bool correctlyPlaced = false;
                while (correctlyPlaced)
                {
                    x = new Random().Next(0, 10);
                    y = new Random().Next(0, 10);
                    orientation = new Random().Next(0, 2);

                    correctlyPlaced = (orientation == 0 ? board.PlaceShips(ship, 'H', x, y) : board.PlaceShips(ship, 'V', x, y));
                }
            }
        }
    }

}
