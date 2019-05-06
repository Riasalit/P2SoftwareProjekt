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

            if (shootingTile.tile == (int)Tile.TileState.hit)
            {
                target = true;
                FillStack(point);
            }
        }
        private void FillStack(Point point)
        {
            CheckPoint(new Point(point.X, point.Y - 1));
            CheckPoint(new Point(point.X, point.Y + 1));
            CheckPoint(new Point(point.X - 1, point.Y));
            CheckPoint(new Point(point.X + 1, point.Y));
        }
        private void CheckPoint(Point point)
        {
            if (!pointsShot.Contains(point) && point.X >= 0 && point.X < Settings.boardWidth && 
                                               point.Y >= 0 && point.Y < Settings.boardWidth)
            {
                pointStack.Push(point);
                pointsShot.Add(point);
            }
        }
        private void Target()
        {
            Point point = (Point)pointStack.Pop();
            Tile shootingTile;

            shootingTile = ShootOpponent(point);

            if (shootingTile.tile == (int)Tile.TileState.hit)
            {
                FillStack(point);
            }
            if (pointStack.Count == 0)
            {
                target = false;
            }
        }
    }
}
