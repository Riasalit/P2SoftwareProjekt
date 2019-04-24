using System.Drawing;

namespace BattleshipWeb
{
    public class Ship
    {
        public int length { get;  }
        private int hits;
        public string name { get; }
        public char orientation { get; }
        public Point shipCoord { get; }

        public Ship(string name, int length, Point shipCoord, char orientation)
        {
            this.length = length;
            this.name = name;
            this.orientation = orientation;
            this.shipCoord = shipCoord;
            hits = 0;
        }
        public void IncreaseHits()
        {
            hits++;
        }
        public bool IsSunken()
        {
            return hits == length;
        }
    }
}
