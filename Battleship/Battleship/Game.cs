using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    class Game
    {
        Player[] players;
        int turn;
        bool running;

        public Game()
        {
            players = new Player[2];
            //Random player starts
            turn = new Random().Next(0,2);
            running = true;
        }
        public void Start()
        {
            //Makes Players
            GetPlayers(1); GetPlayers(2);
            players[0].SetOpponent(players[1]);
            players[1].SetOpponent(players[0]);
            //Places Ships
            players[0].SetShips(); players[1].SetShips();
            //Swaps turns and shoot until all ships are gone
            while (running)
            {
                RunningGame();
            }

        }
        private void RunningGame()
        {
            //Runs the game and swaps turns between players
            players[turn].YourTurn();
            if (players[(turn + 1) % 2].board.sunkenShips == 5)
            {
                running = false;
            }
            turn = (turn + 1) % 2;
        }
        
        private void GetPlayers(int player)
        {
            /*
             * FIX MEEEEEE, NO USERINTERACTION IN GAME CODE
             */
            Console.WriteLine($"Is player{player} a human or an AI?");
            Console.WriteLine();
            Console.WriteLine("Press 0 for human");
            Console.WriteLine("Press 1 for AI");
            int playerRace = int.Parse(Console.ReadLine());
            Console.WriteLine("What's your name?");
            string playerName = Console.ReadLine();
            if (playerRace == 1)
            {
                players[player - 1] = new AI(playerName);
            }
            else if (playerRace == 0)
            {
                players[player - 1] = new Human(playerName);
            }
            else
            {
                Console.WriteLine("Something went wrong, try again");
                GetPlayers(player);
            }

            
        }
    }
}
