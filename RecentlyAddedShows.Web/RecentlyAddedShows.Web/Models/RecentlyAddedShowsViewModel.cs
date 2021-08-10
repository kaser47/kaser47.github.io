using System;
using System.Collections.Generic;
using System.Linq;
using RecentlyAddedShows.Web.Data.Entities;

namespace RecentlyAddedShows.Web.Models
{
    public class RecentlyAddedShowsViewModel
    {
        public IEnumerable<Show> Shows { get; set; }

        public IEnumerable<Show> Cartoons
        {
            get
            {
                return Shows.Where(x => x.Type == Classes.RecentlyAddedShows.ShowType.Cartoon.ToString()).OrderBy(x => x.Name);
            }
        }

        public IEnumerable<Show> Anime
        {
            get
            {
                return Shows.Where(x => x.Type == Classes.RecentlyAddedShows.ShowType.Anime.ToString()).OrderBy(x => x.Name);
            }
        }

        public DateTime LastUpdated => Shows.OrderByDescending(x => x.Created).FirstOrDefault().Created;

        public string TranslatedLastUpdated =>
            Shows.OrderByDescending(x => x.Created).FirstOrDefault().TranslatedCreated;

        public RecentlyAddedShowsViewModel(IEnumerable<Show> shows)
        {
            Shows = shows;
        }
    }
}