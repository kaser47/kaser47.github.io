using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RecentlyAddedShows.Web.Data.Entities;

namespace RecentlyAddedShows.Web.Classes
{
    public class TraktUpNextStrategy : IStrategy
    {
        private readonly string _url;
        private const ShowType ShowType = Classes.ShowType.TVShowUpNext;
        private const string homeUrl = "https://trakt.tv";

        public TraktUpNextStrategy(string url)
        {
            _url = url;
        }

        public ConcurrentBag<Show> GetShows(DateTime date)
        {
            using var web1 = new WebClient();

            var data = web1.DownloadString(_url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(data);

            var shows = new ConcurrentBag<Show>();
            var nodesMatchingXPath = htmlDocument.DocumentNode.SelectNodes("//*[@id='progress-wrapper']/div/div[@class='row posters fanarts twenty-four-cols grid-item no-overlays']");

            Parallel.ForEach(nodesMatchingXPath, node =>
            {
                var name = GetName(node);
                var urlValue = GetUrl(node);
                var imageValue = GetImage(node);
                date = GetDate(node);

                shows.Add(new Show(name, urlValue, imageValue, ShowType, date));
            });

            return shows;
        }

        private string GetName(HtmlNode node)
        {
            var name = string.Empty;

            var showTitle = node.GetText(2,1,0,4,1);
            var episode = node.GetText(2, 1, 0, 4, 2);

            name = $"{showTitle} - {episode}";

            return name;
        }

        private string GetUrl(HtmlNode node)
        {
            return homeUrl + node.GetUrl(2, 1);
        }

        private string GetImage(HtmlNode node)
        {
            return node.GetActualImage(2, 1, 0, 1);
        }

        private DateTime GetDate(HtmlNode node)
        {
            return DateTime.Parse(node.GetDate(1, 2, 17));
        }
    }
}