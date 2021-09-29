using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RecentlyAddedShows.Web.Data;
using RecentlyAddedShows.Web.Data.Entities;
using RecentlyAddedShows.Web.Models;

namespace RecentlyAddedShows.Web.Classes
{
    public class RssFeedCreator
    {
        private readonly IOptions<Configuration> _config;

        public RssFeedCreator(IOptions<Configuration> config)
        {
            _config = config;
        }

        private static IEnumerable<SyndicationItem> CreateSyndicationItems(IEnumerable<Show> shows)
        {
            var items = new List<SyndicationItem>();

            
            foreach (var show in shows)
            {
                var content = $"{show.Type} - {show.TranslatedCreated} <img src='{show.Image}'>";

                var item = new SyndicationItem
                {
                    Title = new TextSyndicationContent($"{show.Name}"),
                    BaseUri = new Uri(show.Url),
                    Content = SyndicationContent.CreateHtmlContent(content),
                    PublishDate = new DateTimeOffset(show.Created.AddHours(-7)),
                    Links = { new SyndicationLink(new Uri(show.Url), "alternate", "Title", "text/html", 1000) },
                };

                items.Add(item);
            }

            return items;
        }

        public IActionResult CreateCartoonRssFeed()
        {
            var recentlyAddedShows = new RecentlyAddedShows(_config);
            var shows = recentlyAddedShows.GetModel().Shows;

            var items = shows.Where(x => x.Type == "Anime" || x.Type == "Cartoon")
                .OrderByDescending(x => x.Created).ThenByDescending(x => x.Type).ThenBy(x => x.Name);

            return CreateRssFeed(items);
        }

        public IActionResult CreateRssFeed(ShowType type)
        {
            var recentlyAddedShows = new RecentlyAddedShows(_config);
            var shows = recentlyAddedShows.GetModel().Shows;

            var items = shows.Where(x => x.Type == type.ToString())
                .OrderByDescending(x => x.Created).ThenByDescending(x => x.Type).ThenBy(x => x.Name);

            return CreateRssFeed(items);
        }

        public IActionResult CreatePopularRssFeed(ShowType type)
        {
            var recentlyAddedShows = new RecentlyAddedShows(_config);
            var shows = recentlyAddedShows.GetModel().Shows;

            var items = shows.Where(x => x.Type == type.ToString())
                .OrderByDescending(x => x.NumberViewing)
                .ThenByDescending(x => x.Created)
                .ThenByDescending(x => x.Type).ThenBy(x => x.Name);

            return CreateRssFeed(items);
        }

        private static IActionResult CreateRssFeed(IOrderedEnumerable<Show> items)
        {
            var syndicationItems = CreateSyndicationItems(items);

            var feed = new SyndicationFeed(ConstantValues.SiteTitle,
                    ConstantValues.Description, new Uri(ConstantValues.SiteUrl))
                {Items = syndicationItems};
            var settings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize,
                NewLineOnAttributes = true,
                Indent = true
            };
            using var stream = new MemoryStream();
            using (var xmlWriter = XmlWriter.Create(stream, settings))
            {
                var feedFormatter = new Rss20FeedFormatter(feed, false);
                feedFormatter.WriteTo(xmlWriter);
                xmlWriter.Flush();
            }

            var file = new FileContentResult(stream.ToArray(), "application/rss+xml;charset=utf-8");

            return file;
        }
    }
}