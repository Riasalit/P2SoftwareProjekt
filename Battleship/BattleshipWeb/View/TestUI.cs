using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace BattleshipWeb
{
    public class TestUI : IUserInterface
    {
        private int numberOfGames = 5000;
        private int counter = 0;
        private int choice;
        private string playerName;
 
        public Player[] InitializePlayers(IUserInterface UI)
        {
            // Takes user input the first time the class is run
            // Decides which Player type is executed
            if (counter == 0)
            {
                Console.WriteLine("AI, Random or Hunt/target (0, 1 or 2)");
                choice = int.Parse(playerName + Console.ReadLine());
                Console.WriteLine("What is your name");
                playerName = Console.ReadLine();
            }
            Player[] players = new Player[1];
            if (choice == 0)
            {
                players[0] = new AI(playerName);
            }
            else if (choice == 1)
            {
                players[0] = new RandomAI(playerName);
            }
            else if (choice == 2)
            {
                players[0] = new HuntTargetAI(playerName);
            }
            else
            {
                Console.WriteLine("Invalid input");
            }
            return players;
        }
        // Saves results when a game is complete
        public bool GameComplete(Player[] players, int playerWon)
        {
            // Creates a text file and saves results
            if (counter == 0)
            {
                File.WriteAllText("BattleshipResult.txt", string.Empty);
                File.AppendAllText("BattleshipResult.txt", players[0].playerName + Environment.NewLine);
            }
            // Runs if a game failed
            if(players[0].turnCounter > Settings.boardSize)
            {
                return true;
            }
            // Saves results to the file
            if (counter < numberOfGames)
            {
                File.AppendAllText("BattleshipResult.txt", players[0].turnCounter + Environment.NewLine);
                counter++;
                return true;
            }
            // Saves results for the last game
            File.AppendAllText("BattleshipResult.txt", players[0].turnCounter + Environment.NewLine);
            return false;
        }
        public Point MakeTargetPoint(List<Point> points, string name)
        {
            throw new NotImplementedException();
        }
        public void ReturnInformation(Point point, Tile info)
        {
            throw new NotImplementedException();
        }
        public Ship GetShips(bool correctlyPlaced, string name)
        {
            throw new NotImplementedException();
        }
    }
}
