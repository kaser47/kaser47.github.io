using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using RecentlyAddedShows.Web.Data;
using RecentlyAddedShows.Web.Models;

namespace RecentlyAddedShows.Web.Classes
{
    public static class RssFeedCreator
    {
        private static IEnumerable<SyndicationItem> CreateSyndicationItems(RecentlyAddedShowsViewModel showsViewModel)
        {
            List<SyndicationItem> items = new List<SyndicationItem>();

            foreach (var show in showsViewModel.Shows.OrderByDescending(x => x.Created).ThenByDescending(x => x.Type)
                .ThenBy(x => x.Name))
            {
                string isUpdated = show.IsUpdated ? "NEW -" : string.Empty;
                string content = $"{show.Type} - {show.TranslatedCreated}";

                SyndicationItem item = new SyndicationItem
                {
                    Title = new TextSyndicationContent($"{show.Name}"),
                    BaseUri = new Uri(show.Url),
                    Summary = new TextSyndicationContent(content),
                    PublishDate = new DateTimeOffset(show.Created),
                    Links = { new SyndicationLink(new Uri(show.Url), "alternate", "Title", "text/html", 1000) },
                };
                items.Add(item);
            }

            return items;
        }

        public static IActionResult CreateRssFeed()
        {
            var syndicationItems = CreateSyndicationItems(RecentlyAddedShows.GetModel());

            var feed = new SyndicationFeed(ConstantValues.SiteTitle,
                    ConstantValues.Description, new Uri(ConstantValues.SiteUrl))
                { Items = syndicationItems };
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