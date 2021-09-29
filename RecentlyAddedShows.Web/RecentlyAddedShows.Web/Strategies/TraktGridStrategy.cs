using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using RecentlyAddedShows.Web.Data.Entities;

namespace RecentlyAddedShows.Web.Classes
{
    public class TraktGridStrategy : IStrategy
    {
        private readonly string _url;
        private readonly ShowType _showType;
        private const string homeUrl = "https://trakt.tv";

        public TraktGridStrategy(string url, ShowType showType)
        {
            _url = url;
            _showType = showType;
        }

        public IList<Show> GetShows(DateTime date)
        {
            using var web1 = new WebClient();

            var data = web1.DownloadString(_url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(data);

            var shows = new List<Show>();
            var nodesMatchingXPath = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class,'row posters')]/div[@class='grid-item col-xs-6 col-md-2 col-sm-3']");

            foreach (var node in nodesMatchingXPath)
            {
                var name = GetName(node);
                var urlValue = GetUrl(node);
                var imageValue = GetImage(node);

                if (DateExists(node))
                {
                    date = GetDate(node);
                }

                shows.Add(new Show(name, urlValue, imageValue, _showType, date));
            }

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
            return homeUrl + url;
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