using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
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
        public bool PlaceShips(Ship ship) 
        {
            //out of bounds check/overlap check
            //returns false if something fail
            if (CheckIfError(ship))
            {
                return false;
            }
            else
            {
                ships.Add(ship);
                //Places ships in their respective directions
                if (ship.orientation == 'H')
                {
                    for (int i = 0; i < ship.length; i++)
                    {
                        gameBoard[ship.y, ship.x + i].SetShip(ship);
                    }
                }
                else if (ship.orientation == 'V')
                {
                    for (int i = 0; i < ship.length; i++)
                    {
                        gameBoard[ship.y + i, ship.x].SetShip(ship);
                    }
                }
                return true;
            }
        }
        private bool CheckIfError(Ship ship)
        {
            //Returns true if a ship is being placed out of bounds or is being placed on an existing ship
            return (ShipOutOfBounds(ship) ||
                    ShipsOverlap(ship));
        }
        private bool ShipOutOfBounds(Ship ship)
        {
            //Returns true if a ship is out of bounds
            return ((ship.orientation == 'H' && ship.x + ship.length - 1 >= 10) ||
                    (ship.orientation == 'V' && ship.y + ship.length - 1 >= 10));
        }
        private bool ShipsOverlap(Ship ship)
        {
            //Returns true if a ship already exists where the new ship is being placed
            if (ship.orientation == 'H')
            {
                for (int i = 0; i < ship.length; i++)
                {
                    if (gameBoard[ship.y, ship.x + i].CheckShip())
                    {
                        return true;
                    }
                }
            }
            else if (ship.orientation == 'V')
            {
                for (int i = 0; i < ship.length; i++)
                {
                    if (gameBoard[ship.y+1, ship.x].CheckShip())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public string ShootAt(Point point)
        {
            //Shoots at x, y on the board if not already shot
            if (!gameBoard[point.Y, point.X].isHit)
            {
                return gameBoard[point.Y, point.X].ShotInformation();
            }
            else
            {
                return "Already hit";
            }
        }
    }
}
