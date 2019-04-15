using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
/**************************************
 * JEG ER EN TEST CLASS FOR TESTING :3*
 * ************************************/
namespace BattleshipWeb
{
    public class TestUI : IUserInterface
    {
        List<string> shipNames;
        List<int> shipLengths;
        private int shipCount;
        public TestUI()
        {
            shipNames = new List<string>();
            shipLengths = new List<int>();
            foreach (string name in Settings.shipNames) shipNames.Add(name);
            foreach (int integer in Settings.shipLengths) shipLengths.Add(integer);
            shipCount = Settings.shipCount;
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
            Ship returnShip = new Ship("temp", 0, new Point(1, 2), 'H');
            bool gotValidShipData = false;
            int x = 0;
            int y = 0;
            char orientation = 's';

            if (shipCount == Settings.shipCount)
            {
                shipCount = 0;
                Console.WriteLine($"{playerName}: Place ships");
            }
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
                //Validation
                while (input.Length != 3 || !int.TryParse(input[0], out x) || !int.TryParse(input[1], out y) ||
                           !(input[2] == "H" || input[2] == "V"))
                {
                    Console.WriteLine("Wrong Input");
                    Console.WriteLine($"Give coordinates and direction for your {shipNames[shipCount]}");
                    Console.WriteLine($"The information should be in the following format: x, y, 'H'/'V'");
                    input = Console.ReadLine().Replace(" ", "").Split(',');
                    Console.WriteLine();
                }
                orientation = char.Parse(input[2]);
                returnShip = new Ship(shipNames[shipCount], shipLengths[shipCount], 
                             new Point(x-1, y-1), orientation);
                gotValidShipData = true;
                shipCount++;
                if (shipCount == Settings.shipCount)
                {
                    Console.Clear();
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
                if (input.Length == 2)
                {
                    returnPoint.X = int.Parse(input[0]) - 1;
                    returnPoint.Y = int.Parse(input[1]) - 1; 
                    //Validation
                    if (!points.Contains(returnPoint) && (returnPoint.X >= 0 && returnPoint.X < Settings.boardWidth) 
                                                      && (returnPoint.Y >= 0 && returnPoint.Y < Settings.boardWidth))
                    {
                        gotValidTarget = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Coordinates. Please try again: ");
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
            Console.WriteLine();
        }
        public void GameComplete(Player[] players, int playerWon)
        {
            Console.WriteLine($"Congratulations {players[playerWon].playerName}, " +
                              $"you won the game with {players[playerWon].turnCounter} turns!");
            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine();
                Console.WriteLine($"{players[i].playerName} sunk: ");
                foreach (Ship ship in players[i+1%2].board.ships)
                {
                    if (ship.IsSunken())
                    {
                        Console.WriteLine($"{ship.name} on ({ship.shipCoord.X},{ship.shipCoord.Y}) " +
                                          $"with orientation: {ship.orientation}");
                    }
                }
            }
        } 
    }
}
