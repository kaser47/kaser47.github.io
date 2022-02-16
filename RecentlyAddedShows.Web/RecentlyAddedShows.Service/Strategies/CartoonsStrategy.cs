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
    public class CartoonsStrategy : IStrategy
    {
        public ConcurrentBag<Show> GetShows(DateTime date)
        {
            const string baseUrl = "https://www.wcoforever.net";
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