using System;
using System.Collections.Concurrent;
using RecentlyAddedShows.Service.Data.Entities;

namespace RecentlyAddedShows.Service.Strategies
{
    public interface IStrategy
    {
        ConcurrentBag<Show> GetShows(DateTime date);
    }
}