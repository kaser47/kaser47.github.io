using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RecentlyAddedShows.Web.Data.Entities;

namespace RecentlyAddedShows.Web.Classes
{
    public class WatchCartoonsOnlineStrategy : IStrategy
    {
        private readonly string _url;
        private readonly ShowType _showType;

        public WatchCartoonsOnlineStrategy(string url, ShowType showType)
        {
            _url = url;
            _showType = showType;
        }

        public ConcurrentBag<Show> GetShows(DateTime date)
        {
            using var web1 = new WebClient();
            string data = string.Empty;

            try 
            {
                data = web1.DownloadString(_url);
            }
            catch (Exception ex)
            {
                return new ConcurrentBag<Show>();
            }
            
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(data);

            var shows = new ConcurrentBag<Show>();
            var nodesMatchingXPath = htmlDocument.DocumentNode.SelectNodes("//*[@id='sidebar_right']/ul/li");

            Parallel.ForEach(nodesMatchingXPath, node =>
            {
                var name = node.GetText(3, 0);
                var urlValue = node.GetUrl(3, 0);
                var imageValue = node.GetImage(1, 1, 1);
                shows.Add(new Show(name, urlValue, imageValue, _showType, date));
            });

            return shows;
        }
    }

    public class CartoonsStrategy : IStrategy
    {
        public ConcurrentBag<Show> GetShows(DateTime date)
        {
            string baseUrl = "https://www.wcoforever.net";
            using var web1 = new WebClient();
            string data = string.Empty;

            try
            {
                data = web1.DownloadString(baseUrl);
            }
            catch (Exception ex)
            {
                return new ConcurrentBag<Show>();
            }

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(data);

            var shows = new ConcurrentBag<Show>();
            var selector = @"//*[@class='recent-release' and contains(.,'Cartoon')][2]/following-sibling::div[1]/ul/li";
            var nodesMatchingXPath = htmlDocument.DocumentNode.SelectNodes(selector);

            Parallel.ForEach(nodesMatchingXPath, node =>
            {
                var name = node.GetText(3, 0);
                var urlValue = baseUrl + node.GetUrl(3, 0);
                var imageValue = node.GetImage(1, 1, 0);
                shows.Add(new Show(name, urlValue, imageValue, ShowType.Cartoon, date));
            });

            return shows;
        }
    }
}