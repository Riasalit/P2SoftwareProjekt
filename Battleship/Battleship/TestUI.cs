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
        Ship ship = new Ship("ole", 1);
        Char placementOrientation = 'H';
        int x = 3, y = 5;
        public void InterActWithUser()
        {
            //set randomizing here
        }
        public Ship GetShip()
        {
            return ship;
        }
        public Char[] GetData()
        {
            //The ascii value for numbers is 48 to 57 
            //Makes a Char[3] array with [Orientation, X-coordinat]
            Char[] returnArray= {placementOrientation, Convert.ToChar(x + 48), Convert.ToChar(y + 48)};
            return returnArray;
        }
    }
}
