﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
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

            var data = web1.DownloadString(_url);
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
}