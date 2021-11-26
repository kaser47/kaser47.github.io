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
}