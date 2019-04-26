using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BattleshipWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IUserInterface UI = new TempUI();
            Game game = new Game(UI);
            //CreateWebHostBuilder(args).Build().Run();
            game.Start();
        }

        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            //WebHost.CreateDefaultBuilder(args)
                //.UseStartup<Startup>();
    }
}
