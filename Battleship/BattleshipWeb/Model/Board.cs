﻿using System.Collections.Generic;
using System.Drawing;

namespace BattleshipWeb
{
    public class Board
    {
        private Tile[,] gameBoard;
        public List<Tile> yourShots { get; private set; } 
        public List<Ship> ships { get;}
        public int sunkenShips { get; private set; }

        public Board()
        {
            yourShots = new List<Tile>();
            ships = new List<Ship>();
            // Creates game board
            gameBoard = new Tile[Settings.boardWidth, Settings.boardWidth];
            for (int i = 0; i < Settings.boardWidth; i++)
            {
                for (int j = 0; j < Settings.boardWidth; j++)
                {
                    gameBoard[i, j] = new Tile();
                }
            }
            sunkenShips = 0;
        }
        public bool PlaceShips(Ship ship)
        {
            // Out of bounds and overlap check
            // Returns false if something fails
            if (CheckIfError(ship))
            {
                return false;
            }
            else
            {
                ships.Add(ship);
                // Places ships in their respective directions
                if (ship.orientation == 'H')
                {
                    for (int i = 0; i < ship.length; i++)
                    {
                        gameBoard[ship.shipCoord.X + i, ship.shipCoord.Y].SetShip(ship);
                    }
                }
                else if (ship.orientation == 'V')
                {
                    for (int i = 0; i < ship.length; i++)
                    {
                        gameBoard[ship.shipCoord.X, ship.shipCoord.Y + i].SetShip(ship);
                    }
                }
                return true;
            }
        }
        private bool CheckIfError(Ship ship)
        {
            // Returns true if a ship is being placed out of bounds or is being placed on an existing ship
            return (ShipOutOfBounds(ship) || ShipsOverlap(ship));
        }
        private bool ShipOutOfBounds(Ship ship)
        {
            // Returns true if a ship is out of bounds
            return ((ship.orientation == 'V' && ship.shipCoord.Y + ship.length - 1 >= Settings.boardWidth) ||
                    (ship.orientation == 'H' && ship.shipCoord.X + ship.length - 1 >= Settings.boardWidth));
        }
        private bool ShipsOverlap(Ship ship)
        {
            // Returns true if a ship already exists where the new ship is being placed
            if (ship.orientation == 'H')
            {
                for (int i = 0; i < ship.length; i++)
                {
                    if (gameBoard[ship.shipCoord.X + i, ship.shipCoord.Y].CheckShip())
                    {
                        return true; 
                    }
                }
            }
            else if (ship.orientation == 'V')
            {
                for (int i = 0; i < ship.length; i++)
                {
                    if (gameBoard[ship.shipCoord.X, ship.shipCoord.Y + i].CheckShip())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public Tile ShootAt(Point point)
        {
            Tile shootingTile = gameBoard[point.X, point.Y];
            // Shoots at x, y on the board if not already shot
            if (shootingTile.tile == (int)Tile.TileState.unknown)
            {
                yourShots.Add(shootingTile);
                shootingTile.SetTileState();
                if (shootingTile.tile == (int)Tile.TileState.sunk)
                {
                    sunkenShips++;
                }
                return shootingTile;
            }
            // Returns -1 if the tile is already hit
            return new Tile();
        }
    }
}
