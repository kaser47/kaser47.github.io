using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RecentlyAddedShows.Service.Data;
using RecentlyAddedShows.Service.Data.Entities;
using RecentlyAddedShows.Service.Models;
using RecentlyAddedShows.Service.Strategies;

namespace RecentlyAddedShows.Service.Classes
{
    public class RecentlyAddedShows
    {
        public IList<Show> Get()
        {
            var shows = new ConcurrentBag<Show>();

            var strategies = new List<IStrategy>()
            {
               new CartoonsStrategy(),
               new WatchCartoonsOnlineStrategy("https://www.wcoanimedub.tv/", ShowType.Anime),
               new TraktUpNextStrategy("https://trakt.tv/users/kaser47/progress/watched/recently-aired?hide_completed=true&page=1", true),
               new TraktUpNextStrategy("https://trakt.tv/users/kaser47/progress/watched/recently-aired?hide_completed=true&page=2", true),
               new TraktUpNextStrategy("https://trakt.tv/users/kaser47/progress/watched/recently-aired?hide_completed=true&page=3", true),
               new TraktUpNextStrategy("https://trakt.tv/users/kaser47/progress/watched/activity?hide_completed=true&page=1"),
               new TraktUpNextStrategy("https://trakt.tv/users/kaser47/progress/watched/activity?hide_completed=true&page=2"),
               new TraktUpNextStrategy("https://trakt.tv/users/kaser47/progress/watched/activity?hide_completed=true&page=3"),
               new TraktPopularStrategy("https://trakt.tv/movies/trending", ShowType.MoviePopular),
               new TraktPopularStrategy("https://trakt.tv/shows/trending", ShowType.TVShowPopular),
               new TraktGridStrategy("https://trakt.tv/users/kaser47/collection/shows/title?genres=", ShowType.TVShowCollection),
               new TraktGridStrategy("https://trakt.tv/users/kaser47/collection/shows/title?genres=&page=2", ShowType.TVShowCollection),
               new TraktGridStrategy("https://trakt.tv/users/kaser47/collection/shows/title?genres=&page=3", ShowType.TVShowCollection),
               new TraktGridStrategy("https://trakt.tv/users/kaser47/collection/shows/title?genres=&page=4", ShowType.TVShowCollection),
               new TraktGridStrategy("https://trakt.tv/users/kaser47/watchlist?display=movie&sort=added,asc", ShowType.MovieFavourites),
               new MetacriticStrategy(ShowType.GameSwitch),
               new MetacriticStrategy(ShowType.GamePC),
               new MetacriticStrategy(ShowType.GamePS4)
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
            var dbContext = new Context(Configuration.ConnectionString);

            var results = Get();
            var savedResults = dbContext.Shows.ToList();
            int resultsCount = results.Count();
            int savedReultsCount = savedResults.Count();
            var t = dbContext.Shows.ToList();
            var itemsToRemove = savedResults.Where(x => results.All(y => y.Name != x.Name | y.NumberViewing != x.NumberViewing));
            var savedResultKeyPairs = savedResults.Select(x => new KeyValuePair<string, string>(x.Name, x.Type));
            var resultKeyPairs = results.Select(x => new KeyValuePair<string, string>(x.Name, x.Type));
            var moreItemsToAddKeyPairs = resultKeyPairs.Where(x => !savedResultKeyPairs.Contains(x));
            var extraItemsToAdd = new List<Show>();
            foreach (var item in moreItemsToAddKeyPairs)
            {
                try
                {
                    var itemValue = results.Where(x => x.Name == item.Key && x.Type == item.Value).FirstOrDefault();
                    if (itemValue != null)
                    {
                        extraItemsToAdd.Add(itemValue);
                    }
                }
                catch (Exception)
                {

                }
            }
            int extraItemsCount = extraItemsToAdd.Count();
            var itemsToAdd = results.Where(x => savedResults.All(y => (y.Name != x.Name && y.Type != x.Type) | y.NumberViewing != x.NumberViewing));
            var ultimateItemsToAdd = itemsToAdd.Concat(extraItemsToAdd).Distinct();
            int ultimateItemsToAddCount = ultimateItemsToAdd.Count();


            if (ultimateItemsToAdd.Any())
            {
                savedResults.ForEach(SetIsUpdatedFalse);
                ultimateItemsToAdd.ToList().ForEach(SetIsUpdatedTrue);
            }

            dbContext.Shows.RemoveRange(itemsToRemove);
            dbContext.Shows.AddRange(ultimateItemsToAdd);

            dbContext.SaveChanges();
            savedResults = dbContext.Shows.ToList();

            var model = new RecentlyAddedShowsViewModel(savedResults);
            return model;
        }

        public RecentlyAddedShowsViewModel LoadModel()
        {
            var dbContext = new Context(Configuration.ConnectionString);
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
