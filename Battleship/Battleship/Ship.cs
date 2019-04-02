using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Battleship
{
    public class Ship
    {
        public int length { get;  }
        private int hits;
        public string name { get; }
        public char orientation { get; }
        public Point shipCoord = new Point();

        public Ship(string name, int length, Point shipCoord,  char orientation)
        {
            this.length = length;
            this.name = name;
            this.orientation = orientation;
            this.shipCoord = shipCoord;
            hits = 0;

        }
        public bool IsSunken()
        {
            return hits == length;
        }
    }
}
