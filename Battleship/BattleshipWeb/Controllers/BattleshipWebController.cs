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
        private WebUI webUI;
        private Game game;
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
                name = Settings.shipNames[index],
                length = Settings.shipLengths[index]
            });
        }
        public class ShipInfo
        {
            public string name;
            public int length;
        }
    }
}
