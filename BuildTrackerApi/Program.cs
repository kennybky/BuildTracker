using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildTrackerApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
            .UseContentRoot(Directory.GetCurrentDirectory())
            .Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder.ConfigureKestrel(serverOptions =>
                 {
                 })
                 .UseStartup<Startup>();
             });
    }
}
