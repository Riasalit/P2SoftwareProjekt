using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Ship
    {
        public int length { get;  }
        private int hits;
        public string name { get; }
        public char orientation { get; }
        public int x { get; }
        public int y { get; }
        
        public Ship(string name, int length, int x, int y, char orientation)
        {
            this.length = length;
            this.name = name;
            this.x = x;
            this.y = y;
            this.orientation = orientation;
            hits = 0;
            
        }
        public bool IsSunken()
        {
            return hits == length;
        }
    }
}
