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

        public IActionResult Rss()
        {
            var model = GetModel();

            SyndicationFeed feed = null;
            string siteTitle, description, siteUrl;
            siteTitle = "Recently Added Shows";
            siteUrl = "https://bsite.net/kaser47/";
            description = "Welcome to the dotnetawesome.com! We are providing step by step tutorial of ASP.NET Web Forms, ASP.NET MVC, Jquery in ASP.NET and about lots of control available in asp.net framework &amp; topic  like GridView, webgrid, mvc4, DropDownList, AJAX, Microsoft, Reports, .rdlc,    mvc, DetailsView, winforms, windows forms, windows application, code, .net code, examples, WCF, tutorial, WebService, LINQ and more. This is the best site for beginners as well as for advanced learner.";

     
                List<SyndicationItem> items = new List<SyndicationItem>();

                foreach (var show in model.Shows.OrderByDescending(x => x.IsUpdated).ThenBy(x => x.Type).ThenBy(x => x.Name))
                {
                    string isUpdated = show.IsUpdated ? "NEW - " : string.Empty;
                    string content = $"{isUpdated}{show.Type} - {model.TranslatedLastUpdated}";

                SyndicationItem item = new SyndicationItem
                {
                    Title = new TextSyndicationContent($"{show.Name}"),
                    Content = new TextSyndicationContent(content), //here content may be Html content so we should use plain text
                };
                item.Links.Add(new SyndicationLink(new Uri(show.Url)));
                items.Add(item);
            }

                feed = new SyndicationFeed(siteTitle, description, new Uri(siteUrl));
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

            var websiteNames = results.Select(x => x.Name).OrderBy(x => x).ToList();
            var savedNames = savedResults.Select(x => x.Name).OrderBy(x => x).ToList();
            if (!websiteNames.SequenceEqual(savedNames))
            {
                var recentlyUpdated = results.Where(p => savedResults.All(l => p.Name != l.Name)).ToList();

                foreach (var show in dbContext.Shows)
                {
                    dbContext.Shows.Remove(show);
                }

                foreach (var show in recentlyUpdated)
                {
                    var showToAdd = results.FirstOrDefault(x => x.Name == show.Name);
                    showToAdd.IsUpdated = true;
                }

                dbContext.Shows.AddRange(results);

                dbContext.SaveChanges();
                savedResults = dbContext.Shows.ToList();
            }

            var model = new RecentlyAddedModel(savedResults);
            return model;
        }

    }
}

