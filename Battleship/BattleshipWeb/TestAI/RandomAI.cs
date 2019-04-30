using System;
using System.Collections.Generic;
using System.Drawing;

namespace BattleshipWeb
{
    public class RandomAI : Player
    {
        AI bayesianAI = new AI("Bayesian");
        List<Point> shootingPoints;

        public RandomAI() : base("Random")
        {
            bayesianAI = new AI("Bayesian");
            shootingPoints = new List<Point>();
            for (int i = 0; i < Settings.boardWidth; i++)
            {
                for (int j = 0; j < Settings.boardWidth; j++)
                {
                    shootingPoints.Add(new Point(i + 1, j + 1));
                }
            }
            SetShips();
        }

        public override void SetShips()
        {
            bayesianAI.SetShips();
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
