using System;
using System.Collections.Generic;
using RecentlyAddedShows.Web.Data.Entities;

namespace RecentlyAddedShows.Web.Classes
{
    public interface IStrategy
    {
        IList<Show> GetShows(DateTime date);
    }
}