using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Battleship
{
    class Game
    {
        Player[] players;
        int turn;
        bool running;
        IUserInterface UI;

        public Game()
        {
            players = new Player[2];
            //Random player starts
            turn = new Random().Next(0,2);
            running = true;
            UI = new TestUI();
        }
        public void Start()
        {
            //Makes Players
            players = UI.InitializePlayers(UI);
            //Sets opponent
            players[0].SetOpponent(players[1]);
            players[1].SetOpponent(players[0]);
            //Places Ships
            players[0].SetShips();
            players[1].SetShips();
            //Swaps turns and shoot until all ships are gone
            while (running)
            {
                NextPlayer();
            }
        }
        private void NextPlayer()
        {
            //Runs the game and swaps turns between players
            players[turn].YourTurn();
            if (players[(turn + 1) % 2].board.sunkenShips == 5)
            {
                running = false;
            }
            turn = (turn + 1) % 2;
        }





    }
}
