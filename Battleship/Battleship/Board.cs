using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Board
    {
        private Tile[,] gameBoard;
        private List<Ship> ships;
        public int sunkenShips { get; private set; }

        public Board()
        {
            ships = new List<Ship>();
            //Creates game board
            gameBoard = new Tile[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    gameBoard[i, j] = new Tile();
                }
            }
            sunkenShips = 0;
        }
        public bool PlaceShips(Ship ship, char orientation, int x, int y) 
        {
            //out of bounds check/overlap check
            //returns false if something fail
            if (CheckIfError(ship, orientation, x, y))
            {
                return false;
            }
            else
            {
                ships.Add(ship);
                //Places ships in their respective directions
                if (orientation == 'H')
                {
                    for (int i = 0; i < ship.length; i++)
                    {
                        gameBoard[y, x + i].SetShip(ship);
                    }
                }
                else if (orientation == 'V')
                {
                    for (int i = 0; i < ship.length; i++)
                    {
                        gameBoard[y + i, x].SetShip(ship);
                    }
                }
                return true;
            }
        }
        private bool CheckIfError(Ship ship, char orientation, int x, int y)
        {
            //Returns true if a ship is being placed out of bounds or is being placed on an existing ship
            return (ShipOutOfBounds(ship, orientation, x, y) ||
                    ShipsOverlap(ship, orientation, x, y));
        }
        private bool ShipOutOfBounds(Ship ship, char orientation, int x, int y)
        {
            //Returns true if a ship is out of bounds
            return ((orientation == 'H' && x + ship.length - 1 >= 10) ||
                    (orientation == 'V' && y + ship.length - 1 >= 10));
        }
        private bool ShipsOverlap(Ship ship, char orientation, int x, int y)
        {
            //Returns true if a ship already exists where the new ship is being placed
            if (orientation == 'H')
            {
                for (int i = 0; i < ship.length; i++)
                {
                    if (gameBoard[y, x + i].CheckShip())
                    {
                        return true;
                    }
                }
            }
            else if (orientation == 'V')
            {
                for (int i = 0; i < ship.length; i++)
                {
                    if (gameBoard[y+1, x].CheckShip())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public string FireAt(int x, int y)
        {
            //Shoots at x, y on the board if not already shot
            if (!gameBoard[y, x].isHit)
            {
                return gameBoard[y, x].GetHit();
            }
            else
            {
                return "Already hit";
            }
        }
    }
}
