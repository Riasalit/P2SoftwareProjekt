using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************
 * JEG ER EN TEST CLASS FOR TESTING :3*
 * ************************************/
namespace Battleship
{
    class TestUI : IUserInterface
    {
        Ship ship;
        Char orientation;
        int x, y;
        public void getPlayers()
        {
            
        }
        public void InteractWithUser()
        {
            ship = new Ship("Submarine", 3);
            orientation = 'H';
            x = 9;
            y = 0;
        }
        public Ship GetShip()
        {
            return ship;
        }
        public Char[] GetData()
        {
            //Makes a Char[3] array with [Orientation, X-coordinat]
            Char[] returnArray = {orientation, Convert.ToChar(x + '0'), Convert.ToChar(y + '0')};
            return returnArray;
        }
    }
}
