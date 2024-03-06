using Microsoft.EntityFrameworkCore;
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
                var dbContext = new Context();
                ErrorDetails error = new ErrorDetails(ex); 

                dbContext.ErrorDetails.Add(error);
                dbContext.SaveChanges();

                return new ConcurrentBag<Show>();
            }
        }
    }

    public static class ExceptionExtensions
    {
        public static ErrorMessage FindInnerException(this Exception ex)
        {
            if (ex.InnerException != null)
            {

                return ex.InnerException.FindInnerException();
            }

            return new ErrorMessage(ex.Message, ex.StackTrace);
        }

        public static string GetAllErrors(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            while (ex != null)
            {
                sb.AppendLine("Error Message: " + ex.Message);
                sb.AppendLine("Stack Trace: " + ex.StackTrace);
                sb.AppendLine();

                ex = ex.InnerException;
            }

            return sb.ToString();
        }
    }
}
