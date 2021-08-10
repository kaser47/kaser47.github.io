using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecentlyAddedShows.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;

namespace RecentlyAddedShows.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private Context dbContext;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            var connectionString = _configuration.GetConnectionString("recentlyAddedShowsContext");
            dbContext = new Context();
        }

        public IActionResult Index()
        {
            var model = GetModel();

            return View(model);
        }

        private const string siteTitle = "Recently Added Shows";
        private const string siteUrl = "https://bsite.net/kaser47/";
        private const string description = "An RSS Feed created by Ash Rhodes for finding out the latest cartoons and anime to watch.";

        public IActionResult Rss()
        {
            var model = GetModel();

            List<SyndicationItem> items = new List<SyndicationItem>();

                foreach (var show in model.Shows.OrderByDescending(x => x.Created).ThenByDescending(x => x.Type).ThenBy(x => x.Name))
                {
                    string isUpdated = show.IsUpdated ? "NEW -" : string.Empty;
                    string content = $"{show.Type} - {show.TranslatedCreated}";

                SyndicationItem item = new SyndicationItem
                {
                    Title = new TextSyndicationContent($"{show.Name}"),
                    BaseUri = new Uri(show.Url),
                    Summary = new TextSyndicationContent(content),
                    PublishDate = new DateTimeOffset(show.Created),
                    Links = { new SyndicationLink(new Uri(show.Url), "alternate", "Title", "text/html", 1000)},
                };
                items.Add(item);
            }

                return CreateRSS(items);
        }
        
        private IActionResult CreateRSS(List<SyndicationItem> items)
        {
            SyndicationFeed feed = new SyndicationFeed(siteTitle, description, new Uri(siteUrl));
            feed.Items = items;
            var settings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize,
                NewLineOnAttributes = true,
                Indent = true
            };
            using (var stream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(stream, settings))
                {
                    var rssFormator = new Rss20FeedFormatter(feed, false);
                    rssFormator.WriteTo(xmlWriter);
                    xmlWriter.Flush();
                }

                return File(stream.ToArray(), "application/rss+xml;charset=utf-8");
            }
        }

        public IActionResult Json()
        {
            var model = GetModel();
            return new JsonResult(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private RecentlyAddedModel GetModel()
        {
            var results = RecentlyAddedShows.Get();
            var savedResults = dbContext.Shows.ToList();

            var itemsToRemove = savedResults.Where(x => results.All(y => y.Name != x.Name));
            var itemsToAdd = results.Where(x => savedResults.All(y => y.Name != x.Name));

            if (itemsToAdd.Any())
            {
                savedResults.ForEach(SetIsUpdatedFalse);
                itemsToAdd.ToList().ForEach(SetIsUpdatedTrue);
            }

            dbContext.Shows.RemoveRange(itemsToRemove);
            dbContext.Shows.AddRange(itemsToAdd);

            dbContext.SaveChanges();
            savedResults = dbContext.Shows.ToList();

            var model = new RecentlyAddedModel(savedResults);
            return model;
        }

        private void SetIsUpdatedTrue(Show show)
        {
            show.IsUpdated = true;
        }

        private void SetIsUpdatedFalse(Show show)
        {
            show.IsUpdated = false;
        }
    }
}

