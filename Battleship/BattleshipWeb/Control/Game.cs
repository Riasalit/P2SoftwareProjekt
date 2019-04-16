using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BattleshipWeb
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
            //Swaps turns and shoots until all ships are gone
            bool restartChoice = true;
            while (running)
            {
                restartChoice = NextPlayer();
            }
            ContinueGameOrExit(restartChoice);
            Console.ReadKey();
        }
        private bool NextPlayer()
        {
            bool restartChoice = true;
            //Runs the game and swaps turns between players
            players[turn].YourTurn();
            if (players[(turn + 1) % 2].board.sunkenShips == Settings.shipCount)
            {
                running = false;
                restartChoice = UI.GameComplete(players, turn);
            }
            turn = (turn + 1) % 2;
            return restartChoice;
        }
        private void ContinueGameOrExit(bool restartChoice)
        {
            //restart domain for each player
            if (restartChoice == true)
            {
                foreach(Player player in players)
                {
                    if (player is AI)
                    {
                        (player as AI).DeleteDomain();
                    }
                }
                running = true;
                Start();
            }
            else
            {
                foreach (Player player in players)
                {
                    if (player is AI)
                    {
                        (player as AI).DeleteDomain();
                    }
                }
            }


        }
    }
}
