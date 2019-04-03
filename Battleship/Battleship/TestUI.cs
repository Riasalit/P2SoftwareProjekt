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
    public class TestUI : IUserInterface
    {
        List<string> shipNames;
        List<int> shipLengths;
        public TestUI()
        {
            shipNames = new List<string>();
            shipLengths = new List<int>();
            shipNames.Add("Carrier");
            shipNames.Add("Battleship");
            shipNames.Add("Cruiser");
            shipCount = 5;
        }
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
        public Ship GetShips(bool correctlyPlaced, string playerName)
        {
            Ship returnShip = new Ship("temp", 0, new Point(1, 2),'H');
            bool gotValidShipData = false;

            if (shipCount == 5)
            {
                shipCount = 0;
                Console.Clear();
                Console.WriteLine($"{playerName}: Place ships");
            } // Lav en clear mere!

            if (!correctlyPlaced && (shipCount != 0))
            {
                shipCount--;
                Console.WriteLine($"Your {shipNames[shipCount]} could not be placed there. try again");
            }
            while (!gotValidShipData)
            {
                Console.WriteLine($"Give coordinates and direction for your {shipNames[shipCount]}");
                Console.WriteLine($"The information should be in the following format: x, y, H/V");
                string[] input = Console.ReadLine().Replace(" ", "").Split(',');
                Console.WriteLine();
                if (input.Length == 3 && (input[2] == "H" || input[2] == "V"))
                {
                    int x = int.Parse(input[0]);
                    int y = int.Parse(input[1]); //tryparse
                    char orientation = char.Parse(input[2]);
                    
                    returnShip = new Ship(shipNames[shipCount], shipLengths[shipCount], new Point(x, y), orientation);

                    gotValidShipData = true;
                    shipCount++;
                }
                else
                {
                    Console.WriteLine("Try again pls. The input wasn't valid ");
                }
            }
            return returnShip;
        }
        public Point MakeTargetPoint(List<Point> points, string name)
        {
            Point returnPoint = new Point();
            bool gotValidTarget = false;
            Console.WriteLine($"{name}'s turn");
            while (!gotValidTarget)
            {
                Console.WriteLine($"Where do you want to shoot?");
                Console.WriteLine("Give the coordinate as: x, y");
                string[] input = Console.ReadLine().Replace(" ", "").Split(',');
                if(input.Length == 2)
                {
                    returnPoint.X = int.Parse(input[0]);
                    returnPoint.Y = int.Parse(input[1]); // tryparse
                    if (!points.Contains(returnPoint))
                    {
                        //You typed something wrong
                        gotValidTarget = true;
                    }
                }
                else
                {
                    Console.WriteLine("Try again pls. The input wasn't valid :C");
                }
            }
            return returnPoint;
        }
        public void ReturnInformation(Point point, string info)
        {
            Console.WriteLine(info);
        }
    }
}
