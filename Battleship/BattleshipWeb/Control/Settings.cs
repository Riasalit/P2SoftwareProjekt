using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleshipWeb
{
    public static class Settings
    {
        public const int boardWidth = 10;
        public const int dimension = 2;
        public const int boardSize = boardWidth * boardWidth;
        //largest ships first for ai purposes
        public static readonly Dictionary<string, int> ships = new Dictionary<string, int>
        {
            {"Carrier", 5},
            {"Battleship", 4},
            {"Submarine", 3},
            {"Submarine2", 3},
            {"BesteSkib", 2}
        };
        public const int shipCount = 5;
    }
}
//"Destroyer",
//                "battleship"
//, 2, 4