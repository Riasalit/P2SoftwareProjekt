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
            //if (orientation == 'H' && x+ship.length >= 10 ||
            //    orientation == 'V' && y+ship.length >= 10 || 
            //    ) 
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
                    gameBoard[y+i][x].SetShip(ship);
                }
            }

            return true;

        }
        public string FireAt(int x, int y)
        {
            return gameBoard[x][y].GetHit();
        }




    }
}
