using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using RecentlyAddedShows.Service.Classes;
using RecentlyAddedShows.Service.Data.Entities;

namespace RecentlyAddedShows.Service.Models
{
    public class RecentlyAddedShowsViewModel
    {
        public IEnumerable<Show> Shows { get; set; }

        public IEnumerable<Favourite> Favourites { get; set; }

        public IEnumerable<ErrorMessage> Errors {get; set;}

        public IEnumerable<ErrorDetails> ErrorDetails { get; set; }

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
                return Shows.Where(x => x.Type == ShowType.TVShowRecentlyAired.ToString() && !x.hasDeletedDate).OrderByDescending(x => x.Created);
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

        public IEnumerable<Show> CheckedItems
        {
            get
            {
                return Shows.Where(x => x.IsChecked).OrderByDescending(x => x.Created);
            }
        }

        public IEnumerable<Show> ShowInHtmlItems
        {
            get
            {
                return Shows.Where(x => x.ShowInHtml).OrderByDescending(x => x.Created);
            }
        }

        public string RandomSingleHtmlItem
        {
            get
            {
                Random random = new Random();
                int randomNumber = random.Next(1, 1001);
                var shows = Shows.Where(x => x.Type.Contains(ShowType.MoviePopular.ToString()) 
                || x.Type.Contains(ShowType.TVShowRecentlyAired.ToString())
                || x.Type.Contains(ShowType.Anime.ToString())
                || x.Type.Contains(ShowType.Cartoon.ToString())
                ).ToList();
                var item = string.Empty;
                


                try
                {
                   var show = shows[randomNumber];
  
                   item = $"{show.Type} ----- {show.Name.UrlDecode()} ----- {show.Url}";
                }
                catch (Exception)
                {

                }

                return item; 
            }
        }

        public string RandomMultipleHtmlItems
        {
            get
            {
                Random random = new Random();
                var shows = Shows.Where(x => x.Type.Contains(ShowType.MoviePopular.ToString())
                || x.Type.Contains(ShowType.TVShowRecentlyAired.ToString())
                || x.Type.Contains(ShowType.Anime.ToString())
                || x.Type.Contains(ShowType.Cartoon.ToString())
                ).ToList();
                var item = string.Empty;
                int randomNumber = random.Next(3, 12);

                for (int i = 0; i < randomNumber; i++)
                {
                    try
                    {
                        var show = shows[random.Next(1, 1001)];
                        item += $"{show.Type} ----- {show.Name.UrlDecode()} ----- {show.Url}/////";
                    }
                    catch (Exception)
                    {

                    }
                }

                item = item.Substring(0, item.Length - 5);

                return item;
            }
        }

        public string ShowInHtml
        {
            get
            {
                //Get Movie
                var movies = Shows.Where(x => x.Type == ShowType.MoviePopular.ToString() && x.ShowInHtml).ToList();
                var movieCount = movies.Count();
                if (movieCount > 1)
                {
                    throw new Exception("More than one movie returned, this shouldn't happen");
                }
                var movie = movies.FirstOrDefault();

                //Get Show
                var shows = Shows.Where(x => x.Type == ShowType.TVShowRecentlyAired.ToString() && x.ShowInHtml).ToList();
                var showCount = shows.Count();
                if (showCount > 1)
                {
                    throw new Exception("More than one show returned, this shouldn't happen");
                }
                var show = shows.FirstOrDefault();

                var favourites = Shows.Where(x => x.Type == ShowType.Favourite.ToString() && x.ShowInHtml).OrderByDescending(x => x.Created).ToList();

                var item = string.Empty;



                if (movie != null)
                {
                    item += $"{movie.Type} ----- {movie.Name.UrlDecode()} ----- {movie.Url}/////";
                }

                if (show != null)
                {
                    item += $"{show.Type} ----- {show.Name.UrlDecode()} ----- {show.Url}/////";
                }

                var count = favourites.Count();

                if (count == 1)
                {
                    var favourite = favourites.FirstOrDefault();
                    item += $"{favourite.Type} ----- {favourite.Name.UrlDecode()} ----- {favourite.Url}/////";
                }
                else if (count > 1)
                {
                    foreach (var favourite in favourites)
                    {
                        item += $"{favourite.Type} ----- {favourite.Name.UrlDecode()} ----- {favourite.Url}/////";
                    }
                }

                if (item != string.Empty)
                {
                    item = item.Substring(0, item.Length - 5);
                }
     
                return item;
               } 
            } 
        

         static void AddOrUpdateItem(Dictionary<string, CounterWithShowType> dictionary, string item, ShowType showType = ShowType.Favourite)
        {
            if (dictionary.ContainsKey(item))
            {
                // If item exists, increment the count
                var value = dictionary[item];
                value.Count++;
                value.ShowType = showType;
            }
            else
            {
                // If item doesn't exist, add it with count 1
                var counter = new CounterWithShowType();
                counter.ShowType = showType;
                counter.Count = 1;
                dictionary.Add(item, counter);

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

        

        public RecentlyAddedShowsViewModel(IEnumerable<Show> shows, IEnumerable<ErrorMessage> errors, IEnumerable<Favourite> favourites, IEnumerable<ErrorDetails> errorDetails)
        {
            Shows = shows;
            Errors = errors.OrderByDescending(x => x.Created);
            Favourites = favourites.OrderBy(x => x.Title);
            ErrorDetails = errorDetails.OrderByDescending(x => x.Created);
        }
    }

    public class CounterWithShowType
    {
        public ShowType ShowType { get; set; }
        public int Count { get; set; }
    }

    public static class StringExtentions
    {
        public static string UrlDecode(this string text)
        {
            var result = HttpUtility.HtmlDecode(text);
            return result;
        }
    }
}