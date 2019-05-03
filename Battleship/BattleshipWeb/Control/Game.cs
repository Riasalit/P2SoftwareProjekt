using System;

namespace BattleshipWeb
{
    public class Game
    {
        Player[] players;
        public bool running;
        IUserInterface UI;

        public Game(IUserInterface UI)
        {
            players = new Player[2];
            // Random player starts
            running = true;
            this.UI = UI;
        }
        public void Start()
        {
            // Makes Players
            players = UI.InitializePlayers(UI);
            // Sets opponent
            players[0].SetOpponent(players[0]);
            // Places Ships
            players[0].SetShips();
            // Swaps turns and shoots until all ships are gone
            while (running && !players[0].turnFailed)
            {
                NextPlayer();
            }
            ContinueGameOrExit(UI.GameComplete(players, 0));
        }
        private void NextPlayer()
        {
            // Runs the game and swaps turns between players
            players[0].YourTurn();
            if (players[0].board.sunkenShips == Settings.shipCount)
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
                    (player as AI).DeleteDomain();
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
