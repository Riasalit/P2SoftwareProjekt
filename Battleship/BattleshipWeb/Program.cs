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
            Game game = new Game();
            AI man = new AI("lars");
            //CreateWebHostBuilder(args).Build().Run();
            man.InitBayesianNetwork();
            game.Start();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}