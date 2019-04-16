using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleshipWeb
{
    public static class Settings
    {
        public const int boardWidth = 3;
        public const int boardSize = boardWidth * boardWidth;
        public static readonly string[] shipNames = 
            {
                "Destroyer",
                "Submarine",
                
            };
        public static readonly int[] shipLengths =
            {
                2, 3
            };
        public const int shipCount = 2;

    }
}
//"Cruiser",
//                "battleship"
//, 3, 4