using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RecentlyAddedShows.Service.Classes;
using RecentlyAddedShows.Service.Data.Entities;
using RecentlyAddedShows.Service.Extensions;

namespace RecentlyAddedShows.Service.Strategies
{
    public class TraktGridStrategy : IStrategy
    {
        private readonly string _url;
        private readonly ShowType _showType;
        private const string HomeUrl = "https://trakt.tv";

        public TraktGridStrategy(string url, ShowType showType)
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
            var nodesMatchingXPath = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class,'row posters')]/div[@class='grid-item col-xs-6 col-md-2 col-sm-3']");

            Parallel.ForEach(nodesMatchingXPath, node =>
            {
                var name = GetName(node);
                var urlValue = GetUrl(node);
                var imageValue = GetImage(node);

                if (DateExists(node))
                {
                    date = GetDate(node);
                }

                shows.Add(new Show(name, urlValue, imageValue, _showType, date));
            });

            return shows;
        }

        private string GetName(HtmlNode node)
        {
            var i = node.ChildNodes.Count == 5 ? 3 : 4;
            return node.GetText(i, 0, 0);
        }

        private string GetUrl(HtmlNode node)
        {
            var i = node.ChildNodes.Count == 5 ? 1 : 2;
            var url = node.GetUrl(i);
            return HomeUrl + url;
        }

        private string GetImage(HtmlNode node)
        {
            var i = node.ChildNodes.Count == 5 ? 1 : 2;
            return node.GetActualImage(i, 0, 1);
        }

        private DateTime GetDate(HtmlNode node)
        {
            return DateTime.Parse(node.GetDateAdded());
        }

        private bool DateExists(HtmlNode node)
        {
            var result = node
                .Attributes.Any(x => x.Name == "data-added");

            return result;
        }
    }

    public class MetacriticStrategy : IStrategy
    {
        private readonly string _url;
        private readonly ShowType _showType;
        private const string HomeUrl = "https://www.metacritic.com";

        public MetacriticStrategy(ShowType showType)
        {
            var gameType = string.Empty;

            switch (showType)
            {
                case ShowType.GameSwitch:
                    gameType = "switch";
                    break;
                case ShowType.GamePC:
                    gameType = "pc";
                    break;
                case ShowType.GamePS4:
                    gameType = "ps4";
                    break;
                default:
                    break;
            }

            _url = $"https://www.metacritic.com/browse/games/release-date/new-releases/{gameType}/date";
            _showType = showType;
        }

        public ConcurrentBag<Show> GetShows(DateTime date)
        {
            using var web1 = new WebClient();

            var data = web1.DownloadString(_url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(data);

            var shows = new ConcurrentBag<Show>();
            var nodesMatchingXPath = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class,'title_bump')]/div[contains(@class,'browse_list_wrapper')]/table/tr[not(contains(@class, 'spacer'))]");

            Parallel.ForEach(nodesMatchingXPath, node =>
            {
                var name = GetName(node);
                var urlValue = GetUrl(node);
                var imageValue = GetImage(node);

                date = GetDate(node);
             
                shows.Add(new Show(name, urlValue, imageValue, _showType, date));
            });

            return shows;
        }

        private string GetName(HtmlNode node)
        {
            return node.GetText(3, 5, 0);
        }

        private string GetUrl(HtmlNode node)
        {
            var url = node.GetUrl(3, 5);
            return HomeUrl + url;
        }

        private string GetImage(HtmlNode node)
        {
            return node.GetImage(1, 1, 0);
        }

        private DateTime GetDate(HtmlNode node)
        {
            return DateTime.Parse(node.GetText(3, 7, 3));
        }

        private bool DateExists(HtmlNode node)
        {
            var result = node
                .Attributes.Any(x => x.Name == "data-added");

            return result;
        }
    }
}