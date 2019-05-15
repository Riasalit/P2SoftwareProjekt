using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
/******************************************************************************
 * General interface for communicating with the user                          *
 * InitializePlayers(): Allows a user to initialize the players               *
 * GetShips(): Gets a ship from a human player                                *
 * MakeTargetPoint(): Gets a specific coordinate from player to shoot at      *
 * ReturnInformation(): Returns shooting information for a specific tile      *
 * ****************************************************************************/
namespace BattleshipWeb
{
    public interface IUserInterface
    {
        Player[] InitializePlayers(IUserInterface UI);
        Ship GetShips(bool correctlyPlaced, string name);
        Point MakeTargetPoint(List<Point> points, string name);
        void ReturnInformation(Point point, Tile info);
        bool GameComplete(Player[] players, int playerWon);

    }
}
