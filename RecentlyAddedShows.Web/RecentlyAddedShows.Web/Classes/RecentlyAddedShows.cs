using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RecentlyAddedShows.Web.Data;
using RecentlyAddedShows.Web.Data.Entities;
using RecentlyAddedShows.Web.Models;

namespace RecentlyAddedShows.Web.Classes
{
    public class RecentlyAddedShows
    {
        private readonly IOptions<Configuration> _config;

        public RecentlyAddedShows(IOptions<Configuration> config)
        {
            _config = config;
        }

        public IList<Show> Get()
        {
            var shows = new ConcurrentBag<Show>();

            var strategies = new List<IStrategy>()
            {
               new CartoonsStrategy(),
               new WatchCartoonsOnlineStrategy("https://www.wcoanimedub.tv/", ShowType.Anime),
               new TraktUpNextStrategy("https://trakt.tv/users/kaser47/progress/watched/activity?hide_completed=true"),
               new TraktPopularStrategy("https://trakt.tv/movies/trending", ShowType.MoviePopular),
               new TraktPopularStrategy("https://trakt.tv/shows/trending", ShowType.TVShowPopular),
               new TraktGridStrategy("https://trakt.tv/users/kaser47/collection/shows/title?genres=", ShowType.TVShowCollection),
               new TraktGridStrategy("https://trakt.tv/users/kaser47/collection/shows/title?genres=&page=2", ShowType.TVShowCollection),
               new TraktGridStrategy("https://trakt.tv/users/kaser47/collection/shows/title?genres=&page=3", ShowType.TVShowCollection),
               new TraktGridStrategy("https://trakt.tv/users/kaser47/collection/shows/title?genres=&page=4", ShowType.TVShowCollection),
               new TraktGridStrategy("https://trakt.tv/users/kaser47/watchlist?display=movie&sort=added,asc", ShowType.MovieFavourites),
            }; 
            
            var date = DateTime.UtcNow;
            
            Parallel.ForEach(strategies, strategy =>
            {
                var results = strategy.GetShows(date);
                Parallel.ForEach(results, result =>
                {
                    shows.Add(result);
                });
            });

            return shows.ToList();
        }

        public RecentlyAddedShowsViewModel GetModel()
        {
            var dbContext = new Context(_config.Value.ConnectionStrings[0]);

            var results = Get();
            var savedResults = dbContext.Shows.ToList();
            var t = dbContext.Shows.ToList();
            var itemsToRemove = savedResults.Where(x => results.All(y => y.Name != x.Name | y.NumberViewing != x.NumberViewing));
            var itemsToAdd = results.Where(x => savedResults.All(y => y.Name != x.Name | y.NumberViewing != x.NumberViewing));

            if (itemsToAdd.Any())
            {
                savedResults.ForEach(SetIsUpdatedFalse);
                itemsToAdd.ToList().ForEach(SetIsUpdatedTrue);
            }

            dbContext.Shows.RemoveRange(itemsToRemove);
            dbContext.Shows.AddRange(itemsToAdd);

            dbContext.SaveChanges();
            savedResults = dbContext.Shows.ToList();

            var model = new RecentlyAddedShowsViewModel(savedResults);
            return model;
        }

        public RecentlyAddedShowsViewModel LoadModel()
        {
            var dbContext = new Context(_config.Value.ConnectionStrings[0]);
            var savedResults = dbContext.Shows.ToList();
            var model = new RecentlyAddedShowsViewModel(savedResults);
            return model;
        }

        private static void SetIsUpdatedTrue(Show show)
        {
            show.IsUpdated = true;
        }

        private static void SetIsUpdatedFalse(Show show)
        {
            show.IsUpdated = false;
        }
    }
}
