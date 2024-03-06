using HtmlAgilityPack;
using RecentlyAddedShows.Service.Classes;
using RecentlyAddedShows.Service.Data.Entities;
using RecentlyAddedShows.Service.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RecentlyAddedShows.Service.Strategies
{
    public class InTheatresStrategy : IStrategy
    {
        string _url;

        public InTheatresStrategy(string url)
        {
                _url = url;
        }

        public ConcurrentBag<Show> GetShows(DateTime date)
        {
            const string baseUrl = "https://www.themoviedb.org/";
            String data;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
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
            var selector = @"//*[@class='page_wrapper']/div[@class='card style_1']";
            var nodesMatchingXPath = htmlDocument.DocumentNode.SelectNodes(selector);

            if (nodesMatchingXPath.Count > 0) {
                Parallel.ForEach(nodesMatchingXPath, node =>
                {
                    if (node != null)
                    {
                        var name = node.GetText(3, 3);
                        var formattedName = string.Empty;
                        var urlValue = string.Empty;
                        try
                        {
                            var movieDate = node.GetText(3, 5).Substring(node.GetText(3, 5).Length - 4, 4);
                            formattedName = $"{name} {movieDate}";
                        }
                        catch (Exception)
                        {
                            formattedName = $"{name}";
                        }

                        try
                        {
                            urlValue = baseUrl + node.GetUrl(3, 3, 0);
                        }
                        catch (Exception)
                        {
                            urlValue = string.Empty;
                        }

                        var imageValue = string.Empty;
                        shows.Add(new Show(formattedName, urlValue, imageValue, ShowType.InTheatre, date));
                    }
                });
            }

            return shows;
        }
    }
}
