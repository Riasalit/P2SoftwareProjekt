﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class AI : Player
    {
        public AI(string name) : base(name)
        {
            
        }
        public override void YourTurn()
        {

        }
        public override void SetShips()
        {
            int x, y, orientation;
            string[] shipNames = new string[5] {"Destroyer", "Submarine",
                                                "Cruiser", "Battleship", "Carrier"};
            int[] shipLengths = new int[5] { 2, 3, 3, 4, 5 };

            for (int i = 0; i < 5; i++)
            {
                bool correctlyPlaced = false;
                while (!correctlyPlaced)
                {
                    x = new Random().Next(0, 10);
                    y = new Random().Next(0, 10);
                    orientation = new Random().Next(0, 2);
                    correctlyPlaced = board.PlaceShips(new Ship(shipNames[i], shipLengths[i], x, y, 
                                                       orientation == 0 ? 'H' : 'V'));
                }
            }
        }
    }

}
