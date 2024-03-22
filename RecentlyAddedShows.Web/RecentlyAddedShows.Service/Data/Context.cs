using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using RecentlyAddedShows.Service.Data.Entities;
using RecentlyAddedShows.Service.Strategies;
using System;

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
        public DbSet<ErrorDetails> ErrorDetails { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<ExcludedFavourite> ExcludedFavourites { get; set; }
    }
}