using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BattleshipWeb.Controllers
{
    [Route("api/[controller]")]
    public class BattleshipWebController : Controller
    {
        [HttpGet("[action]")]
        public int StartData()
        {
            return Settings.boardWidth;
        }
    }
}
