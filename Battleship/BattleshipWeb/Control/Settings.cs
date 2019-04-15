using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleshipWeb
{
    public static class Settings
    {
        public const int boardWidth = 10;
        public const int boardSize = boardWidth * boardWidth;
        public static readonly string[] shipNames = 
            {
                "Destroyer",
                "Submarine",
                "Cruiser",
                "battleship"
            };
        public static readonly int[] shipLengths =
            {
                2, 3, 3, 4
            };
        public const int shipCount = 4;

    }
}
