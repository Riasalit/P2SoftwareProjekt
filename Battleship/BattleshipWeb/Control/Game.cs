using System;

namespace BattleshipWeb
{
    public class Game
    {
        Player[] players;
        int turn;
        public bool running;
        IUserInterface UI;

        public Game(IUserInterface UI)
        {
            players = new Player[2];
            // Random player starts
            turn = 0;
            running = true;
            this.UI = UI;
        }
        public void Start()
        {
            // Makes Players
            players = UI.InitializePlayers(UI);
            // Sets opponent
            players[0].SetOpponent(players[1]);
            players[1].SetOpponent(players[0]);
            // Places Ships
            players[0].SetShips();
            players[1].SetShips();
            // Swaps turns and shoots until all ships are gone
            while (running)
            {
                turn = (turn + 1) % 2;
                NextPlayer();
            }
            ContinueGameOrExit(UI.GameComplete(players, turn));
            Console.ReadKey();
        }
        private void NextPlayer()
        {
            // Runs the game and swaps turns between players
            players[turn].YourTurn();
            if (players[(turn + 1) % 2].board.sunkenShips == Settings.shipCount)
            {
                running = false;
            }
        }
        private void ContinueGameOrExit(bool restartChoice)
        {
            // Resets the domain for all AI players
            foreach (Player player in players)
            {
                if (player is AI)
                {
                    AI temp = player as AI;
                    if(temp.GetDomainStatus()) temp.DeleteDomain();
                }
            }
            if (restartChoice == true)
            {
                running = true;
                Start();
            }
        }
    }
}
