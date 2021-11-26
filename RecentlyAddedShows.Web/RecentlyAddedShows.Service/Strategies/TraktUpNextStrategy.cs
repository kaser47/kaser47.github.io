using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RecentlyAddedShows.Service.Classes;
using RecentlyAddedShows.Service.Data.Entities;
using RecentlyAddedShows.Service.Extensions;

namespace RecentlyAddedShows.Service.Strategies
{
    public class TraktUpNextStrategy : IStrategy
    {
        private readonly string _url;
        private const ShowType ShowType = Classes.ShowType.TVShowUpNext;
        private const string HomeUrl = "https://trakt.tv";

        public TraktUpNextStrategy(string url)
        {
            _url = url;
        }

        public ConcurrentBag<Show> GetShows(DateTime date)
        {
            using var webClient = new WebClient();

            var data = webClient.DownloadString(_url);
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

        private static string GetName(HtmlNode node)
        {
            var showTitle = node.GetText(2,1,0,4,1);
            var episode = node.GetText(2, 1, 0, 4, 2);

            var name = $"{showTitle} - {episode}";

            return name;
        }

        private static string GetUrl(HtmlNode node)
        {
            return HomeUrl + node.GetUrl(2, 1);
        }

        private static string GetImage(HtmlNode node)
        {
            return node.GetActualImage(2, 1, 0, 1);
        }

        private DateTime GetDate(HtmlNode node)
        {
            return DateTime.Parse(node.GetDate(1, 2, 17));
        }
    }
}