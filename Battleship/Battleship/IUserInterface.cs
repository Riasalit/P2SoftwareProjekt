using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/***********************************************************************************
 * General interface for communicating with the user                               *
 * This interface needs a function for getting the user inputs: InteractWithUser() *
 * A function to return a complete ship: GetShip()                                 *
 * And a function to return 3 data Chars called: GetData()                         *
 * where:                                                                          *
 * Char[0] is the orientation 'H' or 'V'                                           *
 * Char[1] is the x coordinate of the ship's origin                                *
 * Char[2] is the y coordinate of the ship's origin                                *
 * *********************************************************************************/
namespace Battleship
{
    public interface IUserInterface
    {
        void getPlayers();
        void InteractWithUser();
        Ship GetShip();
        Char[] GetData(); 
    }
}
