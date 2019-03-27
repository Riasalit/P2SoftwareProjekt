using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Tile
    {
        public bool isHit { get; private set; }
        private Ship ship;

        public Tile()
        {
            isHit = false;
        }
        public bool CheckShip()
        {
            return !(ship == null);
        }
        public void SetShip(Ship ship)
        {
            this.ship = ship;
        }
        public string GetHit()
        {
            isHit = true;
            //Runs if ship is hit and sunken
            if (ship != null && ship.IsSunken())
            {
                return "You sunk " + ship.name + "!";
            }
            //Runs if ship is hit but not sunken
            else if (ship != null)
            {
                return "You hit a ship";
            }
            else
            {
                return "You missed";
            }
        }
    }
}
