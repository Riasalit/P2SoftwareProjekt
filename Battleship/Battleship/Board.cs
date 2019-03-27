using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Board
    {
        private Tile[][] gameBoard;
        private List<Ship> ships;
        public int sunkenShips { get; private set; }

        public Board()
        {
            gameBoard = new Tile[10][];
            for (int i = 0; i < 10; i++)
            {
                gameBoard[i] = new Tile[10];
                for (int j = 0; j < 10; j++)
                {
                    gameBoard[i][j] = new Tile();
                    
                    
                }
            }
            sunkenShips = 0;
        }
        public bool PlaceShips(Ship ship, char orientation, int x, int y) 
        {
            //out of bounds check/overlap check
            
            if (CheckIfError(ship, orientation, x, y) == true)
            {
                return false;
            }
            PlaceShips(ship, orientation, x, y);
            return true;

        }
        private bool CheckIfError(Ship ship, char orientation, int x, int y)
        {
            if (ShipOutOfBoundsCheck(ship, orientation, x, y) ||
                ShipOverlapCheck(ship, orientation, x, y))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool ShipOutOfBoundsCheck(Ship ship, char orientation, int x, int y)
        {
            if ((orientation == 'H' && x + ship.length - 1 >= 10) ||
                (orientation == 'V' && y + ship.length - 1 >= 10))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool ShipOverlapCheck(Ship ship, char orientation, int x, int y)
        {
            if (orientation == 'H')
            {
                for (int i = 0; i < ship.length; i++)
                {
                    if (gameBoard[y][x + i].CheckShip() == true)
                    {
                        return true;
                    }
                }

            }
            else if (orientation == 'V')
            {
                for (int i = 0; i < ship.length; i++)
                {
                    if (gameBoard[y+1][x].CheckShip() == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void Place(Ship ship, char orientation, int x, int y)
        {
            ships.Add(ship);
            if (orientation == 'H')
            {
                for (int i = 0; i < ship.length; i++)
                {
                    gameBoard[y][x + i].SetShip(ship);
                }

            }
            else if (orientation == 'V')
            {
                for (int i = 0; i < ship.length; i++)
                {
                    gameBoard[y + i][x].SetShip(ship);
                }
            }
        }
        public string FireAt(int x, int y)
        {
            return gameBoard[y][x].GetHit();
        }
    }
}
