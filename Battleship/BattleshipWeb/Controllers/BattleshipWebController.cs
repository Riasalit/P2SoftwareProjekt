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
            ship = new Ship(info.name, info.length, new Point(info.xStart, info.yStart), info.orientation[0]);
            webUI.ShipsToUI(ship);
        }
        [HttpPost("[action]")]
        public string SendShootingCoords([FromBody]ShootingCoords coord)
        {
            Point shootingPoint = new Point(coord.x, coord.y);
            webUI.CoordToUI(shootingPoint);
            while (!webUI.returnInformationIsready) ;
            return webUI.returnInformation;
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
    }
}
