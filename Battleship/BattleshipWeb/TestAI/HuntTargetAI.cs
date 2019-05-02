using System;
using System.Collections.Generic;
using System.Drawing;

namespace BattleshipWeb
{
    public class HuntTargetAI : Player
    {
        List<Point> shootingPoints;
        bool target = false;
        Point huntPoint;
        int huntDirection;
        Point originalPoint;

        public HuntTargetAI(string name) : base(name)
        {
            shootingPoints = new List<Point>();
            for (int i = 0; i < Settings.boardWidth; i++)
            {
                for (int j = 0; j < Settings.boardWidth; j++)
                {
                    shootingPoints.Add(new Point(i, j));
                }
            }
            SetShips();
            target = false;
            huntDirection = 0;
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
            Tile shootTile;
            if (shootingPoints.Count < (Settings.boardWidth * Settings.boardWidth) / Settings.dimension)
            {
                do
                {
                    point = new Point
                    {
                        X = new Random().Next(0, Settings.boardWidth),
                        Y = new Random().Next(0, Settings.boardWidth)
                    };
                }
                while (!shootingPoints.Contains(point));

                ShootOpponent(point);
                shootingPoints.Remove(point);

            }
            else
            {
                do
                {
                    point = new Point
                    {
                        X = new Random().Next(0, Settings.boardWidth),
                        Y = new Random().Next(0, Settings.boardWidth)
                    };
                }
                while (!shootingPoints.Contains(point) && (((point.Y % 2 == 0) && (point.X % 2 == 1)) || ((point.Y % 2 == 1) && (point.X % 2 == 0))));

                shootTile = ShootOpponent(point);
                shootingPoints.Remove(point);
                if (shootTile.tile == (int)Tile.TileState.hit)
                {
                    target = true;
                    huntPoint = point;
                    originalPoint = point;
                }
            }

        }
        private void Target()
        {
            switch (huntDirection)
            {
                case 0:
                    TargetHelper(new Point(huntPoint.X, huntPoint.Y - 1));
                    break;
                case 1:
                    TargetHelper(new Point(huntPoint.X, huntPoint.Y + 1));
                    break;
                case 2:
                    TargetHelper(new Point(huntPoint.X - 1, huntPoint.Y));
                    break;
                case 3:
                    TargetHelper(new Point(huntPoint.X + 1, huntPoint.Y));
                    break;
                case 4:
                    if (huntPoint == originalPoint)
                    {
                        target = false;
                    }
                    else
                    {
                        huntPoint = originalPoint;
                        huntDirection = 0;
                        Target();
                    }
                        break;
            }


        }
        private void TargetHelper(Point point)
        {
            if (point.X >= 0 && point.X <= Settings.boardWidth && point.Y >= 0 && point.Y <= Settings.boardWidth && shootingPoints.Contains(point))
            {
                Tile tile = ShootOpponent(point);
                shootingPoints.Remove(point);
                if (tile.tile == (int)Tile.TileState.hit)
                {
                    huntPoint = point;
                    huntDirection = 0;
                }
                else if (tile.tile == (int)Tile.TileState.sunk)
                {
                    target = false;
                    huntDirection = 0;
                }
                else if (huntDirection > 3)
                {
                    huntPoint = originalPoint;
                    huntDirection = 0;
                }
                else
                {
                    huntDirection++;
                }
            }
            else
            {
                huntDirection++;
                Target();
            }

        }
    }
}

