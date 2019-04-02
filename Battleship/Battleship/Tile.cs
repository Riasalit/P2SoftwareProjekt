using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Battleship
{
    public class Tile
    {
        public enum TileState : int {unknown = 0, hit = 1, missed = 2};
        private Ship ship;
        public int tile;

        public Tile()
        {
            tile =  (int)TileState.unknown;
        }
        public bool CheckShip()
        {
            return !(ship == null);
        }
        public void SetShip(Ship ship)
        {
            this.ship = ship;
        }
        public string ShotInformation()
        {

            //Runs if ship is hit and sunken
            if (ship != null && ship.IsSunken())
            {
                tile = (int)TileState.hit;
                return "You sunk " + ship.name + "!";
            }
            //Runs if ship is hit but not sunken
            else if (ship != null)
            {
                tile = (int)TileState.hit;
                return "You hit a ship";
            }
            else
            {
                tile = (int)TileState.missed;
                return "You missed";
            }
        }
    }
}
