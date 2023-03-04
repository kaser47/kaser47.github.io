using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RecentlyAddedShows.Service.Classes;
using RecentlyAddedShows.Service.Data.Entities;
using RecentlyAddedShows.Service.Extensions;

namespace RecentlyAddedShows.Service.Strategies
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
            string baseUrl = _url;
            String data;
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl);
                request.UserAgent = $"{Guid.NewGuid()} {Guid.NewGuid()} {Guid.NewGuid()} {Guid.NewGuid()} {Guid.NewGuid()}";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    data = reader.ReadToEnd();
                }

            }
            catch (Exception)
            {
                return new ConcurrentBag<Show>();
            }

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(data);

            var shows = new ConcurrentBag<Show>();
            var nodesMatchingXPath = htmlDocument.DocumentNode.SelectNodes("//*[@id='sidebar_right']/ul/li");

            Parallel.ForEach(nodesMatchingXPath, node =>
            {
                var name = node.GetText(3, 1);
                var urlValue = node.GetUrl(3, 1);
                var imageValue = node.GetImage(1, 1, 1);
                if (name.Contains("Dubbed"))
                { shows.Add(new Show(name, urlValue, imageValue, _showType, date)); }
            });

            return shows;
        }
    }
}