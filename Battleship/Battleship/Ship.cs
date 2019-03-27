using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Ship
    {
        
        public int length { get;  private set; }
        private int hits;
        public string name { get; private set; }

        public Ship(string name, int length)
        {
            this.length = length;
            this.name = name;
            this.hits = 0;
        }
        public bool IsSunken()
        {
            return hits == length;
        }
    }
}
