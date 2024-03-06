using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecentlyAddedShows.Service.Data;
using RecentlyAddedShows.Service.Data.Entities;
using RecentlyAddedShows.Service.Extensions;
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
               new InTheatresStrategy()
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

        public void ClearChecked()
        {
            var dbContext = new Context();

            foreach (var item in dbContext.Shows)
            {
                item.IsChecked = false;
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

            var errorDetails = new List<ErrorDetails>();

            try
            {
                errorDetails = dbContext.ErrorDetails.ToList();
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
                //DO NOT DELETE FAVOURITES, RELEASEDATES, CARTOONS, ANIME, LASTUPDATED
                .Where(x => x.Type != ShowType.Favourite.ToString() && x.Type != ShowType.ReleaseDate.ToString() && x.Type != ShowType.Cartoon.ToString() && x.Type != ShowType.Anime.ToString() && x.Type != ShowType.LastUpdated.ToString()).ToList();
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


            var lastUpdated = dbContext.Shows.Where(x => x.Type == ShowType.LastUpdated.ToString()).FirstOrDefault();
            if (lastUpdated != null)
            {
                lastUpdated.Created = DateTime.UtcNow;
            }
            else
            {
                lastUpdated = new Show("LastUpdated", "", "", ShowType.LastUpdated, DateTime.UtcNow);
                finishedItemsToAdd = finishedItemsToAdd.Append(lastUpdated);
            }


            var addItems = ClearHTMLFlags(finishedItemsToAdd);

            dbContext.Shows.RemoveRange(itemsToRemove);
            dbContext.Shows.AddRange(addItems);

            dbContext.SaveChanges();

            //ShowMovieInHTML();
            //ShowTVShowInHTML();
            //RefreshFavourites();
            //ReorderCartoonsAndAnime();
            savedResults = dbContext.Shows.ToList();
            var favourites = dbContext.Favourites.ToList();

            var model = new RecentlyAddedShowsViewModel(savedResults, errors, favourites, errorDetails);
            return model;
        }

        public RecentlyAddedShowsViewModel LoadModel()
        {
            var dbContext = new Context();
            var savedResults = dbContext.Shows.ToList();
            var favourites = dbContext.Favourites.ToList();
            var errors = new List<ErrorMessage>();

            try
            {
                errors = dbContext.ErrorMessages.ToList();
            }
            catch (Exception)
            {

            }

            var errorDetails = new List<ErrorDetails>();

            try
            {
                errorDetails = dbContext.ErrorDetails.ToList();
            }
            catch (Exception)
            {

            }
            var model = new RecentlyAddedShowsViewModel(savedResults, errors, favourites, errorDetails);
            return model;
        }

        public void ReorderCartoonsAndAnime()
        {
            var dbContext = new Context();
            var cartoons = dbContext.Shows.Where(x => x.Type == ShowType.Cartoon.ToString() && x.DeletedDate == null).OrderByDescending(x => x.Created).ToList();
            var animes = dbContext.Shows.Where(x => x.Type == ShowType.Anime.ToString() && x.DeletedDate == null).OrderByDescending(x => x.Created).ToList();
            if (cartoons.Count() > 16)
            {
                var i = 0;
                foreach (Show cartoon in cartoons)
                {
                    i++;
                    if (i > 16)
                    {
                        cartoon.DeletedDate = DateTime.UtcNow;
                    }
                }
            }

            if (animes.Count() > 16)
            {
                var i = 0;
                foreach (Show anime in animes)
                {
                    i++;
                    if (i > 16)
                    {
                        anime.DeletedDate = DateTime.UtcNow;
                    }
                }
            }


            dbContext.SaveChanges();
        }

        public IEnumerable<Show> ClearHTMLFlags(IEnumerable<Show> items)
        {
            foreach (Show item in items)
            {
                item.ShowInHtml = false;
            }

            return items;
        }

        public void ShowMovieInHTML()
        {
            var dbContext = new Context();
            var topMovie = dbContext.Shows.Where(x => x.Type == ShowType.MoviePopular.ToString()).OrderByDescending(x => x.NumberViewing).FirstOrDefault();

            if (topMovie.hasReleaseDate && topMovie.ReleaseDate < DateTime.UtcNow.AddDays(15) && topMovie.ReleaseDate > DateTime.UtcNow.AddMonths(-2)) {
                if (!topMovie.IsChecked)
                {
                    topMovie.IsChecked = true;
                    topMovie.ShowInHtml = true;
                }
            }
                dbContext.SaveChanges();
        }

        public void ShowTVShowInHTML()
        {
            var dbContext = new Context();
            var upNextTvShow = dbContext.Shows.Where(x => x.Type == ShowType.TVShowUpNext.ToString()).OrderByDescending(x => x.Created).FirstOrDefault();
            var recentlyAiredTvShow = dbContext.Shows.Where(x => x.Type == ShowType.TVShowRecentlyAired.ToString()).OrderByDescending(x => x.Created).FirstOrDefault();

            if (!recentlyAiredTvShow.IsChecked)
            {
                recentlyAiredTvShow.IsChecked = true;

                if (!checkTitle(recentlyAiredTvShow.Name, upNextTvShow.Name)) {
                    recentlyAiredTvShow.ShowInHtml = true;
                }

                dbContext.SaveChanges();
            }
        }

        public void RefreshFavourites()
        {
            var dbContext = new Context();
            var favouritesToAdd = new List<Show>();
            var existingFavouriteInstances = dbContext.Shows.Where(x => x.Type == ShowType.Favourite.ToString()).ToList();
            var deletedFavouriteInstances = dbContext.Shows.Where(x => x.Type == ShowType.Favourite.ToString() && x.DeletedDate != null).ToList();
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

            foreach (var deletedFavouriteInstance in deletedFavouriteInstances)
            {
                foreach (var favourite in favouriteNames)
                {
                    if (checkTitle(favourite, deletedFavouriteInstance.Name))
                    {
                        deletedFavouriteInstance.DeletedDate = null;
                        deletedFavouriteInstance.ShowInHtml = true;
                    }
                }
            }

            var sortedFavourites =   favouritesToAdd.GroupBy(x => x.Name)
                                                    .Select(g => g.First())
                                                    .ToList();

            foreach (var favouriteShow in existingFavouriteInstances)
            {
                bool remove = true;
                foreach (var name in favouriteNames)
                {
                    if (checkTitle(name, favouriteShow.Name))
                    {
                        remove = false;
                    }
                }

                if (remove && !favouriteShow.hasDeletedDate)
                { 
                    favouriteShow.DeletedDate = DateTime.UtcNow;
                }
            }
            
            foreach (var favourite in sortedFavourites)
            {
                favourite.ShowInHtml = true;
            }
            
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
            title = title.FormatTitle();
            cartoonTitle = cartoonTitle.FormatTitle();

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
