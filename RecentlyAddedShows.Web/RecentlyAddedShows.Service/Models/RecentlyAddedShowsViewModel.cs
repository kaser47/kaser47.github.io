using System;
using System.Collections.Generic;
using System.Linq;
using RecentlyAddedShows.Service.Classes;
using RecentlyAddedShows.Service.Data.Entities;

namespace RecentlyAddedShows.Service.Models
{
    public class RecentlyAddedShowsViewModel
    {
        public IEnumerable<Show> Shows { get; set; }

        public IEnumerable<Show> Cartoons
        {
            get
            {
                return Shows.Where(x => x.Type == ShowType.Cartoon.ToString()).OrderBy(x => x.Name);
            }
        }

        public IEnumerable<Show> Anime
        {
            get
            {
                return Shows.Where(x => x.Type == ShowType.Anime.ToString()).OrderBy(x => x.Name);
            }
        }

        public IEnumerable<Show> NextUp
        {
            get
            {
                return Shows.Where(x => x.Type == ShowType.TVShowUpNext.ToString()).OrderByDescending(x => x.Created);
            }
        }

        public IEnumerable<Show> TVCollection
        {
            get
            {
                return Shows.Where(x => x.Type == ShowType.TVShowCollection.ToString()).OrderBy(x => x.Name);
            }
        }

        public IEnumerable<Show> TVPopular
        {
            get
            {
                return Shows.Where(x => x.Type == ShowType.TVShowPopular.ToString()).OrderByDescending(x => x.NumberViewing);
            }
        }

        public IEnumerable<Show> MovieWatchlist
        {
            get
            {
                return Shows.Where(x => x.Type == ShowType.MovieFavourites.ToString()).OrderByDescending(x => x.Created);
            }
        }

        public IEnumerable<Show> MoviePopular
        {
            get
            {
                return Shows.Where(x => x.Type == ShowType.MoviePopular.ToString()).OrderByDescending(x => x.NumberViewing);
            }
        }

        public DateTime LastUpdated() {
            var result = Shows.OrderByDescending(x => x.Created).FirstOrDefault();
            if (result != null)
            {
                return result.Created;
            }
            return DateTime.UtcNow;
        }

        public string TranslatedLastUpdated()
        {
            var result = Shows.OrderByDescending(x => x.Created).FirstOrDefault();
            if (result != null)
            {
                return result.TranslatedCreated;
            }
            return string.Empty;
        }

        public RecentlyAddedShowsViewModel(IEnumerable<Show> shows)
        {
            Shows = shows;
        }
    }
}