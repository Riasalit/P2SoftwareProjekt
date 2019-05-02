using System;
using System.Collections.Generic;
using System.Drawing;

namespace BattleshipWeb
{
    public class RandomAI : Player
    {
        List<Point> shootingPoints;

        public RandomAI(string name) : base(name)
        {
            shootingPoints = new List<Point>();
            for (int i = 0; i < Settings.boardWidth; i++)
            {
                for (int j = 0; j < Settings.boardWidth; j++)
                {
                    shootingPoints.Add(new Point(i, j));
                }
            }
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
            Point point;

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
    }
}
