﻿using Microsoft.EntityFrameworkCore;
using RecentlyAddedShows.Service.Data;
using RecentlyAddedShows.Service.Data.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace RecentlyAddedShows.Service.Strategies
{
    public class StrategyWrapper : IStrategy
    {
        private readonly IStrategy strategy;

        public StrategyWrapper(IStrategy strategy)
        {
            this.strategy = strategy;
        }
        public ConcurrentBag<Show> GetShows(DateTime date)
        {
            try
            {
                return strategy.GetShows(date);
            }
            catch (Exception ex)
            {
                //var optionsBuilder = new DbContextOptionsBuilder<Context>();
                //optionsBuilder.UseSqlServer(Configuration.ConnectionString);
                //var dbContext = new Context(optionsBuilder.Options);
                var dbContext = new Context();
                var errorMessage = new ErrorMessage(ex.Message, ex.StackTrace);

                dbContext.ErrorMessages.Add(errorMessage);
                dbContext.SaveChanges();

                return new ConcurrentBag<Show>();
            }
        }
    }
}