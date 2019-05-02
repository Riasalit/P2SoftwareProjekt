using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleshipWeb
{
    public static class Settings
    {
        public const int boardWidth = 8;
        public const int dimension = 2;
        public const int boardSize = boardWidth * boardWidth;
        //largest ships first for ai purposes
        public static readonly Dictionary<string, int> ships = new Dictionary<string, int>
        {
            {"Cruiser", 3},
            {"Submarine", 3},
            {"Destroyer", 2}
        };
        public const int shipCount = 3;
    }
}
//"Destroyer",
//                "battleship"
//, 2, 4