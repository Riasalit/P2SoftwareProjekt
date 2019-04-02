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
        List<string> shipNames;
        List<int> shipLengths;
        public Player[] InitializePlayers(IUserInterface UI)
        {
            shipNames = new List<string>();
            shipLengths = new List<int>();
            shipNames.Add("Carrier");
            shipNames.Add("Battleship");
            shipNames.Add("Cruiser");
            shipNames.Add("Submarine");
            shipNames.Add("Destroyer");
            shipLengths.Add(5);
            shipLengths.Add(4);
            shipLengths.Add(3);
            shipLengths.Add(3);
            shipLengths.Add(2);

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
        public Ship GetShips(bool correctlyPlaced)
        {
            Ship returnShip = new Ship("temp", 0, new Point(1, 2),'H');
            bool gotValidShipData = false;
            if (correctlyPlaced)
            {
                shipNames.RemoveAt(0);
                shipLengths.RemoveAt(0);
            }else if (shipLengths.Count != 5)
            {
                Console.WriteLine($"Your {shipNames[0]} could not be placed there. try again");
            }
            while (!gotValidShipData)
            {
                Console.WriteLine($"Give coordinates and direction for your {shipNames[0]}");
                Console.WriteLine($"The information should be in the following format: x, y, H/V");
                string[] input = Console.ReadLine().Replace(" ", "").Split(',');
                if (input.Length == 3)
                {
                    int x = int.Parse(input[0]);
                    int y = int.Parse(input[1]);
                    char orientation = char.Parse(input[2]);
                    returnShip = new Ship(shipNames[0], shipLengths[0], new Point(x, y), orientation);

                    gotValidShipData = true;
                }
                else
                {
                    Console.WriteLine("Try again pls. The input wasn't valid :C");
                }
            }
            return returnShip;
        }
        public Point MakeTargetPoint(List<Point> points)
        {
            Point returnPoint = new Point();
            bool gotValidTarget = false;
            while (!gotValidTarget)
            {
                Console.WriteLine("Where do you want to shoot?");
                Console.WriteLine("Give the coordinate as: x, y");
                string[] input = Console.ReadLine().Trim().Split(',');
                if(input.Length == 2)
                {
                    returnPoint.X = int.Parse(input[0]);
                    returnPoint.Y = int.Parse(input[1]);
                    if (!points.Contains(returnPoint))
                    {
                        gotValidTarget = true;
                    }
                }
                else
                {
                    Console.WriteLine("Try again pl´s. The input wasn't valid :C");
                }
            }
            return returnPoint;
        }
        public void ReturnInformation(string info)
        {
            Console.WriteLine(info);
        }
    }
}
