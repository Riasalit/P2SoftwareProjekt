﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using BattleshipWeb;

namespace BattleshipTest.ModelTest
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
            players[0] = new Human("TestHuman", UI);
            players[1] = new AI("TestAI");
            return players;
        }
        public Ship GetShips(bool correctlyPlaced, string name)
        {
            if (shipCount == Settings.shipCount)
            {
                shipCount = 0;
            }
            if (!correctlyPlaced && (shipCount != 0))
            {
                shipCount--;
            }
            int random = new Random().Next(1, Settings.boardWidth / 2);
            int randomShipPoint = new Random().Next(0, Settings.boardWidth-1);
            Ship returnShip = new Ship("temp", random, new Point(0 , randomShipPoint), 'H');
            shipCount++;
            return returnShip;
        }
        public Point MakeTargetPoint(List<Point> points, string name)
        {
            return new Point(new Random().Next(0, Settings.boardWidth), new Random().Next(0, Settings.boardWidth));
        }
        public void ReturnInformation(Point point, string info)
        {
            
        }
        public bool GameComplete(Player[] players, int playerWon)
        {
            //always end
            return false;
        }
    }
}
