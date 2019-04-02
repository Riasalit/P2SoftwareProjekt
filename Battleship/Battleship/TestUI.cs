using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
/**************************************
 * JEG ER EN TEST CLASS FOR TESTING :3*
 * ************************************/
namespace Battleship
{
    class TestUI : IUserInterface
    {

        int increment = 0;
        public Player[] InitializePlayers(IUserInterface UI)
        {
            Player[] players = new Player[2];

            for (int i = 0; i < players.Length; i++)
            {
                Console.WriteLine("AI or Human (0 or 1)");
                int answer = int.Parse(Console.ReadLine());
                Console.WriteLine("What is your name");
                string playername = Console.ReadLine();

                if (answer == 1)
                {
                    players[i] = new Human(playername, UI);
                }
                else if (answer == 0)
                {
                    players[i] = new AI(playername);
                }
                else
                {
                    Console.WriteLine("Type again - you dun goofed capri son");
                    i--;
                }
            }
            return players;
        }
        public Ship GetShips()
        {
            Point point = new Point();
            point.Y = 0;
            point.X = increment++;
            return new Ship("Carrier", 5, point, 'V');
        }
    }
}
