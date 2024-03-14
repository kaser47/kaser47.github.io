using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RecentlyAddedShows.Service.Classes;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;


namespace RecentlyAddedShows.Web
{
    public class Program
    {
        [Obsolete]
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.MSSqlServer(
                connectionString: "Server=sql.bsite.net\\MSSQL2016;Database=recentlyaddedshows_ras;User Id=recentlyaddedshows_ras; Password=123qweasd;TrustServerCertificate=True;",
                tableName: "Logs",
                autoCreateSqlTable: true)
                .CreateLogger();
                
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
