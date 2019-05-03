using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace BattleshipWeb
{
    public class HuntTargetAI : Player
    {
        List<Point> shootingPoints;
        Stack pointsToShoot;
        bool target;

        public HuntTargetAI(string name) : base(name)
        {
            pointsToShoot = new Stack();
            shootingPoints = new List<Point>();
            for (int i = 0; i < Settings.boardWidth; i++)
            {
                for (int j = 0; j < Settings.boardWidth; j++)
                {
                    shootingPoints.Add(new Point(i, j));
                }
            }
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
                        X = new Random().Next(0, Settings.boardWidth),
                        Y = new Random().Next(0, Settings.boardWidth)
                    };
                    orientation = new Random().Next(0, 2);
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
                if (turnCounter > Settings.boardSize)
                {
                    turnFailed = true;
                }
                point = new Point
                {
                    X = new Random().Next(0, Settings.boardWidth),
                    Y = new Random().Next(0, Settings.boardWidth)
                };
            } while (!shootingPoints.Contains(point) && point.X % 2 == 0 && point.Y % 2 == 0 || 
                                                        point.X % 2 == 1 && point.Y % 2 == 1);

            shootingTile = ShootOpponent(point);
            pointsShot.Add(point);
            shootingPoints.Remove(point);

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
            if (point.X >= 0 && point.X < 10 && point.Y >= 0 && point.Y < 10 && shootingPoints.Contains(point))
            {
                pointsToShoot.Push(point);
                shootingPoints.Remove(point);
            }
        }
        private void Target()
        {
            Point point = (Point)pointsToShoot.Pop();
            Tile shootingTile;
            
            shootingTile = ShootOpponent(point);
            pointsShot.Add(point);

            if (shootingTile.tile == (int)Tile.TileState.hit)
            {
                FillStack(point);
            }
            if (pointsToShoot.Count == 0)
            {
                target = false;
            }
        }
    }
}


