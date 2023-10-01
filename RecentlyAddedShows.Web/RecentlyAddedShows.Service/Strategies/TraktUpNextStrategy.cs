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
        private readonly bool _isRecentlyAired = false;
        private ShowType ShowType { get {
                if (_isRecentlyAired)
                {
                    return ShowType.TVShowRecentlyAired;
                }
                else
                {
                    return ShowType.TVShowUpNext;
                }
                    } }
        private const string HomeUrl = "https://trakt.tv";

        public TraktUpNextStrategy(string url, bool isRecentlyAired = false)
        {
            _url = url;
            _isRecentlyAired = isRecentlyAired;
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

                if (_isRecentlyAired)
                {
                    date = GetRecentlyAiredDate(node);
                }
                else
                {
                    date = GetDate(node);
                }
                shows.Add(new Show(name, urlValue, imageValue, ShowType, date));
            });

            return shows;
        }

        private static string GetName(HtmlNode node)
        {
            var showTitle = node.GetText(2,1,3,1);
            var episode = node.GetText(2, 1, 3, 2);

            var name = $"{showTitle} - {episode}";

            return name;
        }

        private static string GetUrl(HtmlNode node)
        {
            return HomeUrl + node.GetUrl(2, 1, 0);
        }

        private static string GetImage(HtmlNode node)
        {
            return node.GetActualImage(2, 1,0, 1);
        }

        private DateTime GetDate(HtmlNode node)
        {
            try
            {
                return DateTime.Parse(node.GetDate(1, 2, 17));
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        private DateTime GetRecentlyAiredDate(HtmlNode node)
        {
            try
            {
                return DateTime.Parse(node.GetDate(2, 1, 3, 0, 0));
            }
            catch (Exception)
            {
                return DateTime.Parse(node.GetDate(2, 1, 3, 1, 0));
            }
        }
    }
}