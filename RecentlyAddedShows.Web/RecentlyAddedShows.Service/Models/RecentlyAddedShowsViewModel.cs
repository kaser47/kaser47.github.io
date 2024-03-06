using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

                if (movie != null)
                {
                    favourites.Add(movie);
                }

                if (show != null)
                {
                    favourites.Add(show);
                }

                var count = favourites.Count();

                if (count == 1)
                {
                    var item = favourites.FirstOrDefault();
                    return $"<p style=\"color: white;\">{item.Type.ToString()} - {favourites.FirstOrDefault().Name} - ADDED</p>";
                }
                else if (count > 1)
                {
                    var groupOfFavourites = new Dictionary<string, CounterWithShowType>();
                    foreach (var favourite in favourites)
                    {
                        if (favourite.Type == ShowType.Favourite.ToString())
                        {
                            if (favourite.Name.Contains("Season"))
                            {
                                var name = favourite.Name;
                                var firstPart = name.Split("Season")[0];
                                AddOrUpdateItem(groupOfFavourites, firstPart);
                            }
                            else if (favourite.Name.Contains("Episode"))
                            {
                                var name = favourite.Name;
                                var firstPart = name.Split("Episode")[0];
                                AddOrUpdateItem(groupOfFavourites, firstPart);
                            }
                            else if (favourite.Name.Contains("English"))
                            {
                                var name = favourite.Name;
                                var firstPart = name.Split("English")[0];
                                AddOrUpdateItem(groupOfFavourites, firstPart);
                            }
                            else
                            {
                                AddOrUpdateItem(groupOfFavourites, favourite.Name);
                            }
                        }
                        else if (favourite.Type == ShowType.TVShowRecentlyAired.ToString())
                        {
                            var name = favourite.Name;
                            var firstPart = name.Split("-")[0];
                            AddItem(groupOfFavourites, firstPart, ShowType.TVShowRecentlyAired);
                        }
                        else if (favourite.Type == ShowType.MoviePopular.ToString())
                        {
                            var name = favourite.Name;
                            var firstPart = name.Split("20")[0];
                            AddItem(groupOfFavourites, firstPart, ShowType.MoviePopular);
                        }

                  
                    }

                    return FormatDictionaryAsHtmlTable(groupOfFavourites);
                }
                    return null;
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

        static void AddItem(Dictionary<string, CounterWithShowType> dictionary, string item, ShowType showType)
        {
            var counter = new CounterWithShowType();
            counter.ShowType = showType;
            counter.Count = 0;
            dictionary.Add(item, counter);
        }

        static string FormatDictionaryAsHtmlTable(Dictionary<string, CounterWithShowType> dictionary)
        {
            StringBuilder html = new StringBuilder();
            html.Append(Consts.Html);
            html.Append("<table border='1'>");
            html.Append("<tr><th>New Favourites</th><th>Type</th><th>Count</th></tr>");
            foreach (var kvp in dictionary)
            {
                html.Append("<tr>");
                html.Append($"<td>{kvp.Key}</td>");
                html.Append($"<td>{kvp.Value.ShowType.ToString()}</td>");
                html.Append($"<td>{kvp.Value.Count}</td>");
                html.Append("</tr>");
            }
            html.Append("</table>");
            html.Append(Consts.HtmlEnd);
            return html.ToString();
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

    public class CounterWithShowType
    {
        public ShowType ShowType { get; set; }
        public int Count { get; set; }
    }
}