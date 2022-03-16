using Microsoft.EntityFrameworkCore;
using RecentlyAddedShows.Service.Data.Entities;

namespace RecentlyAddedShows.Service.Data
{
    public class Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Configuration.ConnectionString);
        }

        public DbSet<Show> Shows { get; set; }
        public DbSet<ErrorMessage> ErrorMessages { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
    }
}