using System.Collections.Generic;
using System.Drawing;

namespace BattleshipWeb
{
    public abstract class Player
    {
        public List<Point> pointsShot;
        public Board board { get; }
        private Player opponent;
        public string playerName { get; }
        public int turnCounter { get; private set; }

        public Player(string name)
        {
            playerName = name;
            board = new Board();
            pointsShot = new List<Point>();
            turnCounter = 0;
        }
        public void SetOpponent(Player opponent)
        {
            this.opponent = opponent;
        }
        public string ShootOpponent(Point point)
        {
            turnCounter++;

            return opponent.board.ShootAt(point);
        }
        public abstract void YourTurn();
        public abstract void SetShips();
    }
}
