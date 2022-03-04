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
    public class TraktPopularStrategy : IStrategy
    {
        private readonly string _url;
        private readonly ShowType _showType;
        private const string HomeUrl = "https://trakt.tv";

        public TraktPopularStrategy(string url, ShowType showType)
        {
            _url = url;
            _showType = showType;
        }

        public ConcurrentBag<Show> GetShows(DateTime date)
        {
            using var web1 = new WebClient();

            var data = web1.DownloadString(_url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(data);

            var shows = new ConcurrentBag<Show>();
            var nodesMatchingXPath = htmlDocument.DocumentNode.SelectNodes("//div[@class='row fanarts']/div[@data-show-id]|//div[@class='row fanarts']/div[@data-movie-id]");

            Parallel.ForEach(nodesMatchingXPath, node =>
            {
                var name = GetName(node);
                var urlValue = GetUrl(node);
                var imageValue = GetImage(node);
                var numberOfViewers = GetNumberOfViewers(node);
                shows.Add(new Show(name, urlValue, imageValue, _showType, date, numberOfViewers));
            });

            return shows;
        }

        private static string GetName(HtmlNode node)
        {
            return node.GetText(1, 3, 1);
        }

        private static string GetUrl(HtmlNode node)
        {
            return HomeUrl + node.GetUrl(1, 0);
        }

        private static string GetImage(HtmlNode node)
        {
            return node.GetActualImage(1, 0, 1);
        }

        private static int GetNumberOfViewers(HtmlNode node)
        {
            var result = node.GetText(1, 3, 0);
            result = result.Replace(" people watching", "");
            var number = int.Parse(result);
            return number;
        }
    }
}