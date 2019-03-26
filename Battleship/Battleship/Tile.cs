using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Tile
    {
        private bool isHit;
        private Ship ship;

        public Tile()
        {
            isHit = false;
        }

        public void SetShip(Ship ship)
        {
            this.ship = ship;
        }
        public string GetHit()
        {
            return "wow";
        }
    }
}
