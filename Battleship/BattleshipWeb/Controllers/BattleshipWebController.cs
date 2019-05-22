using System;
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
        private static int gameRunning; // 0 = null, 1 = not running, 2 = running;
        [HttpGet("[action]")]
        public bool GetGameRunning() // Returns true if the server is busy and false if not
        {
            if (gameRunning != 1 && gameRunning != 2) gameRunning = 1;
            if(gameRunning == 1)
            {
                gameRunning = 2; // If a user asked then he is now using the game, therefore it is running
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
            CreateTimer(); // Creates a reset timer
            webUI = new WebUI(username);
            game = new Game(webUI);
            game.Start();
        }

        [HttpGet("[action]")]
        public IEnumerable<ShipInfo> GetShipNamesAndLengths()
        {
            CreateTimer(); // Overwrites the reset timer
            return Enumerable.Range(0, Settings.shipCount).Select(index => new ShipInfo //enumerates from 0 to Settings.shipCount and uses the number as "index"
            {
                // Grabs the names and length from the dictionary in settings
                name = Settings.ships.ElementAt(index).Key, 
                length = Settings.ships.ElementAt(index).Value
            });
        }

        [HttpPost("[action]")]
        public void SendShips([FromBody]ShipInfo info)
        {
            while (webUI == null) ; // As the web ui takes time to be created because it contains an AI we need to wait for it to be completed
            CreateTimer();
            Ship ship = new Ship(info.name, info.length, new Point(info.yStart, info.xStart), info.orientation[0]); //creates a ship from the info provided by the client
            webUI.ShipsToUI(ship); // Sends the created ship to the ui to be placed on the gameBoard
        }

        [HttpPost("[action]")]
        public string SendShootingCoords([FromBody]ShootingCoords coord)
        {
            CreateTimer();
            Point shootingPoint = new Point(coord.y, coord.x); // The board is transposed in the game
            webUI.CoordToUI(shootingPoint); // Sends point to ui
            while (!webUI.returnInformationIsReady) ; // Waits for ai to get information to return
            webUI.returnInformationIsReady = false;
            return webUI.returnInformation;
        }

        [HttpGet("[action]")]
        public IEnumerable<IEnumerable<HumanBoardAndProb>> getHumanBoardAndProb()
        {
            while (webUI == null) ;
            while (webUI.ai == null) ;
            CreateTimer();
            while (!webUI.ai.probabilitiesReady) ;
            webUI.ai.probabilitiesReady = false;
            // Works like 2 nested for loops and thereby returns a 2d array of board positions and the corresponding probabilities
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
            if (restart)
            {
                CreateTimer();
            }
        }

        private void CreateTimer()
        {
            if(resetTimer != null) resetTimer.Enabled = false;
            resetTimer = new Timer(120000); // 120000 millisec = 2 min
            resetTimer.Elapsed += OnTimedEvent; // Add an event to run when the timer runs out
            resetTimer.Enabled = true;
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            gameRunning = 1;
            if (webUI != null)
            {
                game.running = false;
                webUI.gameTimedOut = true;
                resetTimer.Enabled = false;
            }
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
