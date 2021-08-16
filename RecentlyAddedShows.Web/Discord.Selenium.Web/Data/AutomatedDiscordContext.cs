using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Discord.Selenium.Web.Data
{
    public class AutomatedDiscordContext : DbContext
    {
        private readonly string _connectionString;

        public AutomatedDiscordContext()
        {
            _connectionString = Consts.ConnectionString;
        }

        public AutomatedDiscordContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public DbSet<Log> logs { get; set; }
    }

    public class Log
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string Message { get; set; }

        public Log()
        {

        }

        public Log(string message)
        {
            Message = message;
            Created = DateTime.UtcNow;
        }
    }

    public static class Consts
    {
        public static string ConnectionString =
            "workstation id=automatedDiscord.mssql.somee.com;packet size=4096;user id=kaser477_SQLLogin_1;pwd=ndrauk2q23;data source=automatedDiscord.mssql.somee.com;persist security info=False;initial catalog=automatedDiscord";
    }

    public static class DbInitializer
    {
        public static void Initialize(AutomatedDiscordContext automatedDiscordContext)
        {
            automatedDiscordContext.Database.EnsureCreated();

            if (automatedDiscordContext.logs.Any())
            {
                return;
            }

            var log = new Log("Test");
            automatedDiscordContext.logs.Add(log);
            automatedDiscordContext.SaveChanges();
        }
    }
}
