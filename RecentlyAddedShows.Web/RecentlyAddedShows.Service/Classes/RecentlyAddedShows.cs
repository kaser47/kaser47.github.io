using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
               new WatchCartoonsOnlineStrategy("https://www.wco.tv/#dubbed", ShowType.Anime),
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
               new MetacriticStrategy(ShowType.GamePS4),
               new InTheatresStrategy("https://www.themoviedb.org/movie/now-playing?page=1&language=en-GB"),
               new InTheatresStrategy("https://www.themoviedb.org/movie/now-playing?page=2&language=en-GB"),
               new InTheatresStrategy("https://www.themoviedb.org/movie/now-playing?page=3&language=en-GB")
            }; 
            
            var date = DateTime.UtcNow;
            
            Parallel.ForEach(strategies, strategy =>
            {
                StrategyWrapper strategyWrapper = new StrategyWrapper(strategy);
                var results = strategyWrapper.GetShows(date);
                Parallel.ForEach(results, result =>
                {
                    shows.Add(result);
                });
            });

            return shows.ToList();
        }

        public void ClearErrors()
        {
            var dbContext = new Context();

            foreach (var item in dbContext.ErrorMessages)
            {
                dbContext.ErrorMessages.Remove(item);
            }

            dbContext.SaveChanges();
        }

        public RecentlyAddedShowsViewModel GetModel()
        {
            var dbContext = new Context();
            var results = Get();
            var savedResults = dbContext.Shows.ToList();
            var errors = new List<ErrorMessage>();

            try
            {
                errors = dbContext.ErrorMessages.ToList();
            }
            catch (Exception)
            {

            }
            int resultsCount = results.Count();
            int savedReultsCount = savedResults.Count();
            var t = dbContext.Shows.ToList();
            var itemsToRemove = savedResults.Where(x => results.All(y => y.Name != x.Name 
            || y.NumberViewing != x.NumberViewing 
            || x.hasReleaseDate != y.hasReleaseDate 
            || (x.ReleaseDate.HasValue && y.ReleaseDate.HasValue && x.ReleaseDate != y.ReleaseDate)
            || (x.Type == ShowType.TVShowUpNext.ToString() && y.Created != x.Created) 
            ))
                .Where(x => x.Type != ShowType.Favourite.ToString() && x.Type != ShowType.ReleaseDate.ToString()).ToList();
            var addtionalItemsToRemove = savedResults.Where(x => x.Type == ShowType.ReleaseDate.ToString() && x.Created <= DateTime.UtcNow.AddMonths(-6)).ToList();

            var listPopularMovies = savedResults.Where((x) => x.Type == ShowType.MoviePopular.ToString() || x.Type == ShowType.InTheatre.ToString());
            var releaseDateMovies = savedResults.Where(x => x.Type.ToString() == ShowType.ReleaseDate.ToString());

            //Fix incorrect release dates when they get updated.
            foreach (var popularMovie in listPopularMovies)
            {
                foreach (var releaseDate in releaseDateMovies)
                {
                    if (popularMovie.Name == releaseDate.Name && popularMovie.hasReleaseDate && popularMovie.ReleaseDate.Value != releaseDate.Created)
                    {
                        releaseDate.Created = popularMovie.ReleaseDate.Value;
                    }
                }
            }

           itemsToRemove.AddRange(addtionalItemsToRemove);
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

            //var favouritesToAdd = new List<Show>();
            //var existingFavouriteInstances = dbContext.Shows.Where(x => x.Type == ShowType.Favourite.ToString()).ToList();
            //var favouriteNames = dbContext.Favourites.Select(x => x.Title).ToList();
            //var sortedItemsToAdd = ultimateItemsToAdd.Where(x => x.Type == ShowType.Anime.ToString() || x.Type == ShowType.Cartoon.ToString());

            //foreach (var item in sortedItemsToAdd)
            //{
            //    foreach (var favourite in favouriteNames)
            //    {
            //        if (item.Name.ToLower().Trim().Contains(favourite.ToLower().Trim()))
            //        {
            //            var existingItem = existingFavouriteInstances.Where(x => x.Name == item.Name).FirstOrDefault();
            //            if(existingItem == null)
            //            {
            //                favouritesToAdd.Add(new Show(item));
            //            }
            //        }
            //    }
            //}

            //var sortedFavourites =   favouritesToAdd.GroupBy(x => x.Name)
            //                                        .Select(g => g.First())
            //                                        .ToList();

            //var finishedItemsToAdd = ultimateItemsToAdd.Concat(sortedFavourites).Distinct();
            var finishedItemsToAdd = ultimateItemsToAdd.Distinct();

            int ultimateItemsToAddCount = finishedItemsToAdd.Count();

            var releaseDates = new List<Show>();
            if (finishedItemsToAdd.Any())
            {
                savedResults.ForEach(SetIsUpdatedFalse);
                finishedItemsToAdd.ToList().ForEach(SetIsUpdatedTrue);
                var popularMovies = finishedItemsToAdd.Where(x => x.Type == ShowType.MoviePopular.ToString() || x.Type == ShowType.InTheatre.ToString());
                foreach (var item in popularMovies)
                {
                    var savedItem = savedResults.Where(x => x.Name == item.Name && x.Type == item.Type).FirstOrDefault();
                    if (savedItem != null && savedItem.hasReleaseDate && !item.hasReleaseDate)
                    {
                        item.ReleaseDate = savedItem.ReleaseDate;
                    }

                    if (item.hasReleaseDate && item.ReleaseDate.Value > DateTime.UtcNow.AddMonths(-6))
                    {
                        var savedReleaseDate = savedResults.Where(x => x.Name == item.Name && x.Type == ShowType.ReleaseDate.ToString()).FirstOrDefault();
                        if (savedReleaseDate == null)
                        {
                            var releaseDate = new Show(item, ShowType.ReleaseDate.ToString());
                            releaseDates.Add(releaseDate);
                        }
                    }
                }
            }
            releaseDates = releaseDates
                            .GroupBy(p => p.Name)
                            .Select(g => g.First())
                            .ToList();
            var releaseDatesFiltered = releaseDates.Where(rd => !savedResults.Any(sr => sr.Name == rd.Name && sr.Type == rd.Type));

            foreach (var item in releaseDatesFiltered)
            {
                    finishedItemsToAdd = finishedItemsToAdd.Append(item);
            }
            
            dbContext.Shows.RemoveRange(itemsToRemove);
            dbContext.Shows.AddRange(finishedItemsToAdd);

            dbContext.SaveChanges();

            RefreshFavourites();
            savedResults = dbContext.Shows.ToList();

            var model = new RecentlyAddedShowsViewModel(savedResults, errors);
            return model;
        }

        public RecentlyAddedShowsViewModel LoadModel()
        {
            var dbContext = new Context();
            var savedResults = dbContext.Shows.ToList();
            var errors = new List<ErrorMessage>();

            try
            {
                errors = dbContext.ErrorMessages.ToList();
            }
            catch (Exception)
            {

            }
            var model = new RecentlyAddedShowsViewModel(savedResults, errors);
            return model;
        }

        public void RefreshFavourites()
        {
            var dbContext = new Context();
            var favouritesToAdd = new List<Show>();
            var existingFavouriteInstances = dbContext.Shows.Where(x => x.Type == ShowType.Favourite.ToString()).ToList();
            var favouriteNames = dbContext.Favourites.Select(x => x.Title).ToList();
            var cartoonsAndAnime = dbContext.Shows.Where(x => x.Type == ShowType.Anime.ToString() || x.Type == ShowType.Cartoon.ToString()).ToList();

            foreach (var cartoon in cartoonsAndAnime)
            {
                foreach (var favourite in favouriteNames)
                {
                    if (checkTitle(favourite, cartoon.Name))
                    {
                       var newFavourite = new Show(cartoon);
                       var existingItem = existingFavouriteInstances.Where(x => x.Name == newFavourite.Name).FirstOrDefault();
                       if (existingItem == null)
                       {
                            favouritesToAdd.Add(newFavourite);
                        }
                       else if (existingItem.hasDeletedDate)
                        {
                            existingItem.DeletedDate = null;
                        }
                    }
                }
            }
            var sortedFavourites =   favouritesToAdd.GroupBy(x => x.Name)
                                                    .Select(g => g.First())
                                                    .ToList();
            dbContext.Shows.AddRange(sortedFavourites);
            dbContext.SaveChanges();

            DeleteFavourites();
        }

        private void DeleteFavourites()
        {
            var dbContext = new Context();
            var existingFavouriteInstances = dbContext.Shows.Where(x => x.Type == ShowType.Favourite.ToString()).ToList();
            var favouriteNames = dbContext.Favourites.Select(x => x.Title).ToList();

            foreach (var favourite in existingFavouriteInstances)
            {
                bool found = false;
                foreach (var favouriteName in favouriteNames)
                {
                    if (checkTitle(favouriteName, favourite.Name))
                    {
                        found = true; break;    
                    }
                }

                if (!found)
                {
                    favourite.DeletedDate = DateTime.UtcNow;
                }
            }

            dbContext.SaveChanges();
        }

        private bool checkTitle(string title, string cartoonTitle)
        {
            title = title.ToLower().Replace(" ", "");
            cartoonTitle = cartoonTitle.ToLower().Replace(" ", "");

            if (cartoonTitle.Contains(title))
            {
                return true;
            }

            return false;
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
