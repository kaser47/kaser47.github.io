using System.Data.Entity;
using RecentlyAddedShows.Web.Data.Entities;

namespace RecentlyAddedShows.Web.Data
{
    public class Context : DbContext
    {
        private const string ConnectionString =
            "Server=sql.bsite.net\\MSSQL2016;Database=kaser47_RecentlyAddedShows;User Id=kaser47_RecentlyAddedShows;Password=123qwe";
        public Context() : base(ConnectionString)
        {

        }
        
        public DbSet<Show> Shows { get; set; }
    }
}