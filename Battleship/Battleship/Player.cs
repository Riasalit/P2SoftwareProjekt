using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Battleship
{
    public abstract class Player
    {
        public List<Point> points;
        public Board board;
        private Player opponent;
        private string playerName { get; }
        private int hitCount;

        public Player(string name)
        {
            playerName = name;
            board = new Board();
            points = new List<Point>();
        }
        public void SetOpponent(Player opponent)
        {
            this.opponent = opponent;
        }
        public string ShootOpponent(Point point)
        {
            return opponent.board.ShootAt(point);
        }
        public abstract void YourTurn();
        public abstract void SetShips();
    }
}
