using System.Data.Entity;
using RecentlyAddedShows.Service.Data.Entities;

namespace RecentlyAddedShows.Service.Data
{
    public class Context : DbContext
    {
        public Context(string connectionString) : base(connectionString)
        {
        }

        public Context()
        {
            
        }

        public DbSet<Show> Shows { get; set; }
        public DbSet<ErrorMessage> ErrorMessages { get; set; }
    }
}