using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleshipWeb
{
    public static class Settings
    {
        public const int boardWidth = 7;
        public const int boardSize = boardWidth * boardWidth;
        //largest ships first for ai purposes
        public static readonly string[] shipNames = 
            {
                "Carrier",
                "Battleship",
                "Submarine",
                "Cruiser"
            };
        public static readonly int[] shipLengths =
            {
                 5, 4, 3, 3
            };
        public const int shipCount = 4;

    }
}
//"Destroyer",
//                "battleship"
//, 2, 4