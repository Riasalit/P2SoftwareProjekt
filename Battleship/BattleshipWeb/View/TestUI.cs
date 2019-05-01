using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BattleshipWeb
{
    public class TestUI : IUserInterface
    {
        public bool GameComplete(Player[] players, int playerWon)
        {
            throw new NotImplementedException();
        }

        public Ship GetShips(bool correctlyPlaced, string name)
        {
            throw new NotImplementedException();
        }

        public Player[] InitializePlayers(IUserInterface UI)
        {
            Player[] players = new Player[1];
            for (int i = 0; i < players.Length; i++)
            {
                Console.WriteLine("AI, Random or Hunt/target (0, 1 or 2)");
                int answer = int.Parse(Console.ReadLine());
                Console.WriteLine("What is your name");
                string playername = Console.ReadLine();

                if (answer == 0)
                {
                    players[i] = new AI(playername);
                }
                else if (answer == 1)
                {
                    players[i] = new RandomAI(playername);
                }
                else if (answer == 2)
                {
                    players[i] = new HuntTargetAI(playername);
                }
                else
                {
                    Console.WriteLine("Type again - you dun goofed capri son");
                    i--;
                }
            }
            return players;
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
