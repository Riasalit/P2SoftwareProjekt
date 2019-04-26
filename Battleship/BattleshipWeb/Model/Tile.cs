
namespace BattleshipWeb
{
    public class Tile
    {
        // TileState explains the state of the tile
        // unknown: tile not shot at
        // hit: tile shot at and ship is hit
        // missed: tile shot at, but no ship on it
        public enum TileState { unknown, hit, missed };
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
        public bool GetShipStatus()
        {
            if (ship != null)
            {
                return ship.IsSunken();
            }

            return false;
        }
        public string ShotInformation()
        {
            if (ship != null)
            {
                ship.IncreaseHits();
            }
            // Runs if ship is hit and sunken
            if (GetShipStatus())
            {
                tile = (int)TileState.hit;
                return "You sunk " + ship.name;
            }
            // Runs if ship is hit, but not sunken
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
