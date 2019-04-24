using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BattleshipWeb.Controllers
{
    [Route("api/[controller]")]
    public class BattleshipWebController : Controller, IUserInterface
    {
        private bool state = true;
        [HttpGet("[action]")]
        public bool TestStartGame()
        {
            state = !state;
            return !state;
        }
        public bool GameComplete(Player[] players, int playerWon)
        {
            throw new NotImplementedException();
        }

        public Ship GetShips(bool correctlyPlaced, string name)
        {
            throw new NotImplementedException();
        }

        public Player[] InitializePlayers(IUserInterface UI)
        {
            throw new NotImplementedException();
        }

        public Point MakeTargetPoint(List<Point> points, string name)
        {
            throw new NotImplementedException();
        }

        public void ReturnInformation(Point point, string info)
        {
            throw new NotImplementedException();
        }
    }
}
