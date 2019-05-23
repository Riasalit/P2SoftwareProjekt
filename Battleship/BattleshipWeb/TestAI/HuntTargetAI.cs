using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace BattleshipWeb
{
    public class HuntTargetAI : Player
    {
        Stack pointStack;
        bool target;

        public HuntTargetAI(string name) : base(name)
        {
            pointStack = new Stack();
            target = false;
        }
        // Places ships randomly
        public override void SetShips()
        {
            int orientation;
            char orientationLetter;
            bool correctlyPlaced;

            foreach (KeyValuePair<string, int> ship in Settings.ships)
            {
                correctlyPlaced = false;
                while (!correctlyPlaced)
                {
                    Point point = new Point
                    {
                        X = random.Next(0, Settings.boardWidth),
                        Y = random.Next(0, Settings.boardWidth)
                    };
                    orientation = random.Next(0, 2);
                    orientationLetter = orientation == 0 ? 'H' : 'V';
                    correctlyPlaced = board.PlaceShips(new Ship(ship.Key, ship.Value, point, orientationLetter));
                }
            }
        }
        // Switches between hunt and target mode
        public override void YourTurn()
        {
            if (!target)
            {
                Hunt();
            }
            else
            {
                Target();
            }
        }
        private void Hunt()
        {
            Point point;
            Tile shootingTile;
            // Finds a random point in checkerboard pattern
            do
            {
                point = new Point
                {
                    X = random.Next(0, Settings.boardWidth),
                    Y = random.Next(0, Settings.boardWidth)
                };
            } while (pointsShot.Contains(point) || (point.X % 2 == 1 && point.Y % 2 == 0) ||
                                                   (point.X % 2 == 0 && point.Y % 2 == 1));

            shootingTile = ShootOpponent(point);
            pointsShot.Add(point);
            // Runs if a ship is hit
            if (shootingTile.tile == (int)Tile.TileState.hit)
            {
                target = true;
                FillStack(point);
            }
        }
        // Fills a stack with the hit point's surrounding points
        private void FillStack(Point point)
        {
            CheckPoint(new Point(point.X, point.Y - 1));
            CheckPoint(new Point(point.X, point.Y + 1));
            CheckPoint(new Point(point.X - 1, point.Y));
            CheckPoint(new Point(point.X + 1, point.Y));
        }
        // Checks if the point is valid
        private void CheckPoint(Point point)
        {
            // Runs if the point is not already shot at and if it's not out of bounds
            if (!pointsShot.Contains(point) && point.X >= 0 && point.X < Settings.boardWidth && 
                                               point.Y >= 0 && point.Y < Settings.boardWidth)
            {
                // Adds point to the top of the stack
                pointStack.Push(point);
                // Adds the point to points shot
                pointsShot.Add(point);
            }
        }
        // Shoots all points in the stack
        private void Target()
        {
            Point point = (Point)pointStack.Pop();
            Tile shootingTile;

            shootingTile = ShootOpponent(point);
            // Runs if a ship is hit
            if (shootingTile.tile == (int)Tile.TileState.hit)
            {
                FillStack(point);
            }
            // Runs if the stack is empty
            if (pointStack.Count == 0)
            {
                target = false;
            }
        }
    }
}
