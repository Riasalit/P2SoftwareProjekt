﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BattleshipWeb;
using System.Timers;

namespace BattleshipWeb
{
    [Route("api/[controller]")]
    public class BattleshipWebController : Controller
    {
        private static Timer resetTimer;
        private static WebUI webUI;
        private static Game game;
        private static int gameRunning; //1 = not running, 2 = running;
        private static bool userDidSomething;
        [HttpGet("[action]")]
        public bool GetGameRunning()
        {
            if (gameRunning == 0) gameRunning = 1;
            if(gameRunning == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        [HttpGet("[action]")]
        public int StartData()
        {
            
            return Settings.boardWidth;
        }
        [HttpPost("[action]")]
        public void StartGame(string username)
        {
            createTimer();
            webUI = new WebUI(username);
            game = new Game(webUI);
            game.Start();
        }
        [HttpGet("[action]")]
        public IEnumerable<ShipInfo> GetShipnamesAndLengths()
        {
            createTimer();
            return Enumerable.Range(0, Settings.shipCount).Select(index => new ShipInfo
            {
                name = Settings.ships.ElementAt(index).Key,
                length = Settings.ships.ElementAt(index).Value
            });
        }
        [HttpPost("[action]")]
        public void SendShips([FromBody]ShipInfo info)
        {
            createTimer();
            Ship ship;
            ship = new Ship(info.name, info.length, new Point(info.yStart, info.xStart), info.orientation[0]);
            webUI.ShipsToUI(ship);
        }
        [HttpPost("[action]")]
        public string SendShootingCoords([FromBody]ShootingCoords coord)
        {
            createTimer();
            Point shootingPoint = new Point(coord.y, coord.x); //The board is transposed in the game
            webUI.CoordToUI(shootingPoint);
            while (!webUI.returnInformationIsReady) ;
            webUI.returnInformationIsReady = false;
            return webUI.returnInformation;
        }

        [HttpGet("[action]")]
        public IEnumerable<IEnumerable<HumanBoardAndProb>> getHumanBoardAndProb()
        {
            createTimer();
            while (!webUI.ai.probabilitiesReady) ;
            webUI.ai.probabilitiesReady = false;
            return Enumerable.Range(0, Settings.boardWidth).Select(index1 => Enumerable.Range(0, Settings.boardWidth).Select(index2 =>
                new HumanBoardAndProb
                {
                    tileShot = webUI.ai.pointsShot.Contains(new Point(index2, index1)),
                    probability = webUI.ai.probabilities.Where(p => p.Key == new Point(index2, index1))
                                          .Select(p => p.Value)
                                          .Max(),
                    x = index2,
                    y = index1
                }
            ));
        }
        [HttpGet("[action]")]
        public GameOverInfo GetGameOverInfo()
        {
            if (webUI.gameOver)
            {
                gameRunning = 1;
            }
            return new GameOverInfo
            {
                playerWhoWon = webUI.playerWhoWon,
                gameOver = webUI.gameOver
            };
        }
        [HttpPost("[action]")]
        public void RestartOrEndGame(bool restart)
        {
            webUI.restartGame = restart;
            webUI.gotRestartInfo = true;
        }

        private void createTimer()
        {
            resetTimer = new Timer(120000);
            resetTimer.Elapsed += OnTimedEvent;
            resetTimer.Enabled = true;
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            gameRunning = 1;
        }
        public class ShipInfo
        {
            public string name;
            public int length;
            public int xStart;
            public int yStart;
            public string orientation;
            public bool isPlaced;
        }
        public class ShootingCoords
        {
            public int x;
            public int y;
        }
        public class HumanBoardAndProb
        {
            public bool tileShot;
            public double probability;
            public int x;
            public int y;
        }
        public class GameOverInfo
        {
            public int playerWhoWon;
            public bool gameOver;
        }
    }
}
