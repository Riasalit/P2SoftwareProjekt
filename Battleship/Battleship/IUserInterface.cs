using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
/******************************************************************************
 * General interface for communicating with the user                          *
 * This interface needs for a user to create the players: InitializePlayers() *
 * A function to return a complete ship: GetShip()                            *
 * ****************************************************************************/
namespace Battleship
{
    public interface IUserInterface
    {
        Player[] InitializePlayers(IUserInterface UI);
        Ship GetShips(bool correctlyPlaced);
        Point MakeTargetPoint(List<Point> points);
        void ReturnInformation(string info);
    }
}
