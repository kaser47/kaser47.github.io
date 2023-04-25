using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Microsoft.Extensions.Options;
using RecentlyAddedShows.Service.Data;
using RecentlyAddedShows.Service.Data.Entities;

namespace RecentlyAddedShows.Service.Classes
{
    public class RssFeedCreator
    {
        private static IEnumerable<SyndicationItem> CreateSyndicationItems(IEnumerable<Show> shows)
        {
            var items = new List<SyndicationItem>();

            var i = 0;
            foreach (var show in shows)
            {
                var description = string.Empty;

                Enum.TryParse(show.Type, out ShowType showType);
                
                switch (showType)
                {
                    case ShowType.Cartoon:
                    case ShowType.Anime:
                        description = $"{show.Type} - {show.TranslatedCreated}";
                        break;
                    case ShowType.TVShowUpNext:
                        description = $"{show.TranslatedCreated}";
                        break;
                    case ShowType.TVShowRecentlyAired:
                    case ShowType.Favourite:
                    case ShowType.GameSwitch:
                    case ShowType.GamePC:
                    case ShowType.GamePS4:
                        description = $"{show.TranslatedCreated}";
                        break;
                    case ShowType.TVShowPopular:
                    case ShowType.MoviePopular:
                            description = $"Number Viewing: {show.NumberViewing}";
                        if (show.hasReleaseDate)
                        {
                            if (show.ReleaseDate <= DateTime.UtcNow)
                            {
                                description = description + " READY";
                            }
                        }
                            break;
                    case ShowType.TVShowCollection:
                    case ShowType.MovieFavourites:
                        break;
                    default:
                        description = $"Invalid Type: {showType}";
                        break;
                }

                var content = $"{description}<img src='{show.Image}'>";

                var publishedDate = show.PublishiedDate;

                // ReSharper disable once StringLiteralTypo
                //if (show.Type == "Anime" || show.Type == "Cartoon")
                //{
                //    publishedDate = publishedDate.AddHours(-7);
                //}

                var item = new SyndicationItem
                {
                    Id = i.ToString(),
                    Title = new TextSyndicationContent($"{show.Name}"),
                    BaseUri = new Uri(show.Url),
                    Content = SyndicationContent.CreateHtmlContent(content),
                    PublishDate = new DateTimeOffset(publishedDate),
                    Links = { new SyndicationLink(new Uri(show.Url), "alternate", "Title", "text/html", 1000) },
                };

                items.Add(item);
            }

            return items;
        }

        public byte[] CreateCartoonRssFeed()
        {
            var recentlyAddedShows = new RecentlyAddedShows();
            var recentlyAddedShowsViewModel = recentlyAddedShows.GetModel();
            var shows = recentlyAddedShowsViewModel.Shows;
            var items = shows.Where(x => x.Type == "Anime" || x.Type == "Cartoon")
                .OrderByDescending(x => x.Created).ThenByDescending(x => x.Type).ThenBy(x => x.Name);

            return CreateRssFeed(items);
        }

        public byte[] CreateRssFeed(ShowType type)
        {
            var recentlyAddedShows = new RecentlyAddedShows();
            var shows = recentlyAddedShows.LoadModel().Shows;

            var items = shows.Where(x => x.Type == type.ToString())
                .OrderByDescending(x => x.Created).ThenByDescending(x => x.Type).ThenBy(x => x.Name);

            return CreateRssFeed(items);
        }

        public byte[] CreatePopularRssFeed(ShowType type)
        {
            var recentlyAddedShows = new RecentlyAddedShows();
            var shows = recentlyAddedShows.LoadModel().Shows;

            var items = shows.Where(x => x.Type == type.ToString())
                .OrderByDescending(x => x.NumberViewing);

            var i = 0;
            var date = DateTime.UtcNow;

            foreach (var item in items)
            {
                item.Created = date.AddSeconds(-i);
                i++;
            }

            return CreateRssFeed(items);
        }

        private static byte[] CreateRssFeed(IOrderedEnumerable<Show> items)
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

            return stream.ToArray();
        }
    }
}