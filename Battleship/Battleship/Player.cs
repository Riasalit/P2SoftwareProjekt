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
        public Board board;
        private Player opponent;
        private string playerName { get; }
        private int hitCount;

        public Player(string name)
        {
            playerName = name;
            board = new Board();
        }
        public void SetOpponent(Player opponent) 
        {
            this.opponent = opponent;
        }
        public abstract void YourTurn();
        public abstract void SetShips();
        
    }
}
