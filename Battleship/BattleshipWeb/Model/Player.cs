using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BattleshipWeb
{
    public abstract class Player
    {
        public List<Point> points;
        public Board board { get; }
        private Player opponent;
        public string playerName { get; }
        public int turnCounter { get; private set; }

        public Player(string name)
        {
            playerName = name;
            board = new Board();
            points = new List<Point>();
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
