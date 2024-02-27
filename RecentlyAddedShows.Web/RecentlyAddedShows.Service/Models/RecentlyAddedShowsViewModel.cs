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

        public IEnumerable<Favourite> Favourites { get; set; }

        public IEnumerable<ErrorMessage> Errors {get; set;}
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

        public IEnumerable<Show> RecentlyUpdated
        {
            get
            {
                return Shows.Where(x => x.Type == ShowType.TVShowRecentlyAired.ToString()).OrderByDescending(x => x.Created);
            }
        }

        public IEnumerable<Show> Switch
        {
            get
            {
                return Shows.Where(x => x.Type == ShowType.GameSwitch.ToString()).OrderByDescending(x => x.Created);
            }
        }

        public IEnumerable<Show> PC
        {
            get
            {
                return Shows.Where(x => x.Type == ShowType.GamePC.ToString()).OrderByDescending(x => x.Created);
            }
        }

        public IEnumerable<Show> PS4
        {
            get
            {
                return Shows.Where(x => x.Type == ShowType.GamePS4.ToString()).OrderByDescending(x => x.Created);
            }
        }

        public IEnumerable<Show> FavouritesToWatch
        {
            get
            {
                return Shows.Where(x => x.Type == ShowType.Favourite.ToString()).OrderByDescending(x => x.Created);
            }
        }

        public IEnumerable<Show> ReleaseDates
        {
            get
            {
                return Shows.Where(x => x.Type == ShowType.ReleaseDate.ToString()).OrderByDescending(x => x.Created);
            }
        }

        public DateTime LastUpdated() {
            var result = Shows.Where(x => x.Type == ShowType.LastUpdated.ToString()).FirstOrDefault();
            if (result != null)
            {
                return result.Created;
            }
            return DateTime.UtcNow;
        }

        public string TranslatedLastUpdated()
        {
            var result = Shows.Where(x => x.Type == ShowType.LastUpdated.ToString()).FirstOrDefault();
            if (result != null)
            {
                return result.TranslatedCreated;
            }
            return string.Empty;
        }

        public RecentlyAddedShowsViewModel(IEnumerable<Show> shows, IEnumerable<ErrorMessage> errors, IEnumerable<Favourite> favourites)
        {
            Shows = shows;
            Errors = errors.OrderByDescending(x => x.Created);
            Favourites = favourites.OrderBy(x => x.Title);
        }
    }
}