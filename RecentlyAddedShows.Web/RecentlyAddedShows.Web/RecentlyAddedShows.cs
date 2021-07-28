using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RecentlyAddedShows.Web.Controllers;

namespace RecentlyAddedShows.Web
{
    public static class RecentlyAddedShows
    {
            public static List<Show> Get()
            {
                var shows = new List<Show>();

                var _urls = new List<KeyValuePair<string, ShowType>>
                {
                    new("https://www.wco.tv/", ShowType.Cartoon),
                    new("https://www.wcoanimedub.tv/", ShowType.Anime)
                };

                foreach (var (key, value) in _urls)
                {
                    var result = GetShows(key, value);
                    shows.AddRange(result);
                }

                return shows;
            }

            private static IEnumerable<Show> GetShows(string url, ShowType type)
            {
                using var web1 = new WebClient();

                var data = web1.DownloadString(url);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(data);

                var specificNode = htmlDocument.GetElementbyId("sidebar_right");
                var nodesMatchingXPath = htmlDocument.DocumentNode.SelectNodes("//*[@id='sidebar_right']/ul/li");

                return nodesMatchingXPath.Select(a => a.ChildNodes[3].ChildNodes[0].InnerText).Select(text => new Show(text, type)).ToList();
            }

            public enum ShowType
            {
                Cartoon,
                Anime
            }
        }

        public class Show
        {
            public string Name { get; set; }
            public string Type { get; set; }

            public Show(string name, RecentlyAddedShows.ShowType type)
            {
                Name = name;
                Type = type.ToString();
            }
        }
}
