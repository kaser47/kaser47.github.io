using System.Data.Entity;
using RecentlyAddedShows.Web.Data.Entities;

namespace RecentlyAddedShows.Web.Data
{
    public class Context : DbContext
    {
        public Context(string connectionString) : base(connectionString)
        {

        }
        
        public DbSet<Show> Shows { get; set; }
    }
}