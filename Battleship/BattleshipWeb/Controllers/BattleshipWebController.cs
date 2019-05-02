using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BattleshipWeb;

namespace BattleshipWeb
{
    [Route("api/[controller]")]
    public class BattleshipWebController : Controller
    {
        private static WebUI webUI;
        private static Game game;
        [HttpGet("[action]")]
        public int StartData()
        {
            return Settings.boardWidth;
        }
        [HttpPost("[action]")]
        public void StartGame(string username)
        {
            webUI = new WebUI(username);
            game = new Game(webUI);
            game.Start();
        }
        [HttpGet("[action]")]
        public IEnumerable<ShipInfo> GetShipnamesAndLengths()
        {
            
            return Enumerable.Range(0, Settings.shipCount).Select(index => new ShipInfo
            {
                name = Settings.ships.ElementAt(index).Key,
                length = Settings.ships.ElementAt(index).Value
            });
        }
        [HttpPost("[action]")]
        public void SendShips([FromBody]ShipInfo info)
        {
            Ship ship;
            ship = new Ship(info.name, info.length, new Point(info.yStart, info.xStart), info.orientation[0]);
            webUI.ShipsToUI(ship);
        }
        [HttpPost("[action]")]
        public string SendShootingCoords([FromBody]ShootingCoords coord)
        {
            Point shootingPoint = new Point(coord.y, coord.x); //The board is transposed in the game
            webUI.CoordToUI(shootingPoint);
            while (!webUI.returnInformationIsReady) ;
            webUI.returnInformationIsReady = false;
            return webUI.returnInformation;
        }

        [HttpGet("[action]")]
        public IEnumerable<IEnumerable<HumanBoardAndProb>> getHumanBoardAndProb()
        {
            while (!webUI.ai.probabilitiesReady) ;
            webUI.ai.probabilitiesReady = false;
            return Enumerable.Range(0, Settings.boardWidth).Select(index1 => Enumerable.Range(0, Settings.boardWidth).Select(index2 =>
                new HumanBoardAndProb
                {
                    tilesShot = webUI.ai.pointsShot.Contains(new Point(index2, index1)) ? true : false,
                    probability = webUI.ai.probabilities.Where(p => p.Key == new Point(index2, index1))
                                          .Select(p => p.Value)
                                          .Max()
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
            public bool tilesShot;
            public double probability;
        }
        public class GameOverInfo
        {
            public int playerWhoWon;
            public bool gameOver;
        }
    }
}
