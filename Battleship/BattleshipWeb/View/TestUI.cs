using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace BattleshipWeb
{
    public class TestUI : IUserInterface
    {
        private int numberOfGames = 1000;
        private int counter = 0;
        private int choice;
        private string playerName;
 
        public Player[] InitializePlayers(IUserInterface UI)
        {
            if (counter == 0)
            {
                Console.WriteLine("AI, Random or Hunt/target (0, 1 or 2)");
                choice = int.Parse(Console.ReadLine());
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
                Console.WriteLine("Type again - you dun goofed capri son");
            }
            return players;
        }
        public Ship GetShips(bool correctlyPlaced, string name)
        {
            throw new NotImplementedException();
        }

        
        public bool GameComplete(Player[] players, int playerWon)
        {
            if (counter == 0)
            {
                File.WriteAllText("BattleshipResult.txt", string.Empty);
                File.AppendAllText("BattleshipResult.txt", players[0].playerName + Environment.NewLine);
            }
            //Pass the filepath and filename to the StreamWriter Constructor
            if (counter < numberOfGames)
            {
                //Write a second line of text
                File.AppendAllText("BattleshipResult.txt", players[0].turnCounter + Environment.NewLine);
                counter++;
                return true;
            }
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
    }
}
