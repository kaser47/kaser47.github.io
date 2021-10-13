using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using RecentlyAddedShows.Web.Data.Entities;

namespace RecentlyAddedShows.Web.Classes
{
    public interface IStrategy
    {
        ConcurrentBag<Show> GetShows(DateTime date);
    }
}