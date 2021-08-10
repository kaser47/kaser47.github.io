using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using RecentlyAddedShows.Web.Data;
using RecentlyAddedShows.Web.Data.Entities;
using RecentlyAddedShows.Web.Models;

namespace RecentlyAddedShows.Web.Classes
{
    public static class RecentlyAddedShows
    {
            public static List<Show> Get()
            {
                var shows = new List<Show>();

                var urls = new List<KeyValuePair<string, ShowType>>
                {
                    new("https://www.wco.tv/", ShowType.Cartoon),
                    new("https://www.wcoanimedub.tv/", ShowType.Anime)
                };

                foreach (var (key, value) in urls)
                {
                    var result = GetShowsFromUrl(key, value);
                    shows.AddRange(result);
                }

                return shows;
            }

            private static IEnumerable<Show> GetShowsFromUrl(string url, ShowType type)
            {
                using var web1 = new WebClient();

                var data = web1.DownloadString(url);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(data);

                var shows = new List<Show>();
                var nodesMatchingXPath = htmlDocument.DocumentNode.SelectNodes("//*[@id='sidebar_right']/ul/li");

                foreach (var node in nodesMatchingXPath)
                {
                    var name = node.ChildNodes[3].ChildNodes[0].InnerText;
                    var urlValue = node.ChildNodes[3].ChildNodes[0].Attributes["href"].Value;
                    var imageValue = node.ChildNodes[1].ChildNodes[1].ChildNodes[1].Attributes["src"].Value;
                    shows.Add(new Show(name, urlValue, imageValue, type));
                }

                return shows;
            }

            public enum ShowType
            {
                Cartoon,
                Anime
            }

            public static RecentlyAddedShowsViewModel GetModel()
            {
                var dbContext = new Context();

                var results = Get();
                var savedResults = dbContext.Shows.ToList();

                var itemsToRemove = savedResults.Where(x => results.All(y => y.Name != x.Name));
                var itemsToAdd = results.Where(x => savedResults.All(y => y.Name != x.Name));

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
