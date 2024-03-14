using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using RecentlyAddedShows.Service;
using RecentlyAddedShows.Service.Classes;
using RecentlyAddedShows.Service.Models;
using RssFeedCreator = RecentlyAddedShows.Service.Classes.RssFeedCreator;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net;
using System.Linq;
using System;
using System.Reflection;

namespace RecentlyAddedShows.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Service.Classes.RecentlyAddedShows _recentlyAddedShows;
        private readonly RssFeedCreator _rssFeedCreator;
        
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _recentlyAddedShows = new Service.Classes.RecentlyAddedShows();
            _rssFeedCreator = new RssFeedCreator();
        }

        public IActionResult Index()
        {
            var model = _recentlyAddedShows.LoadModel();
            return View(model);
        }

        public IActionResult GetLastUpdated()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            var model = _recentlyAddedShows.LoadModel();
            string TranslatedLastUpdated = model.TranslatedLastUpdated();
            string LastUpdated = model.LastUpdated().ToString();
            var data = new { TranslatedLastUpdated, LastUpdated };


            _logger.LogWarning($"{this.GetType().Name}/{MethodBase.GetCurrentMethod().Name} Result: {data}");
            return Json(data);
        }

        public IActionResult Clear()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            _recentlyAddedShows.ClearErrors();
            return RedirectToAction("Index");
        }

        public IActionResult ClearChecked()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            _recentlyAddedShows.ClearChecked();
            return RedirectToAction("Index");
        }

        public IActionResult Refresh()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            var model = _recentlyAddedShows.GetModel();
            return View("Index", model);
        }

        public IActionResult Update()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            ContentResult result = new ContentResult
            {
                Content = "Success",
                ContentType = "text/plain"
            };

            var viewModel = _recentlyAddedShows.GetModel();


            if (viewModel.ShowInHtml != String.Empty)
            {
                result.Content = viewModel.ShowInHtml.ToString();
            }

            _logger.LogWarning($"{this.GetType().Name}/{MethodBase.GetCurrentMethod().Name} Result: {result.Content}");
            _logger.LogError($"{this.GetType().Name}/{MethodBase.GetCurrentMethod().Name} Result: {result.Content}");
            return result;
        }

        public IActionResult TestLogClear()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            _recentlyAddedShows.ClearDownLogs();

            ContentResult result = new ContentResult
            {
                Content = "Success",
                ContentType = "text/plain"
            };
            _logger.LogWarning($"{this.GetType().Name}/{MethodBase.GetCurrentMethod().Name} Result: {result.Content}");
            return result;
        }

        public IActionResult TestRandomSingle(string showType = null)
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            ContentResult result = new ContentResult
            {
                Content = "Success",
                ContentType = "text/plain"
            };

            var viewModel = _recentlyAddedShows.LoadModel();

            var item = viewModel.GetRandomSingleHtmlItem(showType);

            if (item != "")
            {
                result.Content = item;
            }

            _logger.LogWarning($"Result: {result.Content}");
            return result;
        }

        public IActionResult TestRandomMultiple(string showType = null)
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            ContentResult result = new ContentResult
            {
                Content = "Success",
                ContentType = "text/plain"
            };

            var viewModel = _recentlyAddedShows.LoadModel();

            var item = viewModel.GetRandomMultipleHtmlItems(showType);

            if (item != "")
            {
                result.Content = item;
            }

            _logger.LogWarning($"{this.GetType().Name}/{MethodBase.GetCurrentMethod().Name} Result: {result.Content}");
            return result;
        }

        public IActionResult RssAnimation()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            var result = _rssFeedCreator.CreateCartoonRssFeed();

            return result.ToRss();
        }

        public IActionResult RssUpNext()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            var result = _rssFeedCreator.CreateRssFeed(ShowType.TVShowUpNext);
            return result.ToRss();
        }

        public IActionResult RssRecentlyAired()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            var result = _rssFeedCreator.CreateRssFeed(ShowType.TVShowRecentlyAired);
            return result.ToRss();
        }


        public IActionResult RssPopularMovies()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            var result = _rssFeedCreator.CreatePopularRssFeed(ShowType.MoviePopular);
            return result.ToRss();
        }


        public IActionResult RssPopularShows()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            var result = _rssFeedCreator.CreatePopularRssFeed(ShowType.TVShowPopular);
            return result.ToRss();
        }

        public IActionResult RssReleaseDates()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            var result = _rssFeedCreator.CreateRssFeed(ShowType.ReleaseDate);
            return result.ToRss();
        }

        public IActionResult RssShowCollection()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            var result = _rssFeedCreator.CreateRssFeed(ShowType.TVShowCollection);
            return result.ToRss();
        }

        public IActionResult RssMovieWatchlist()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            var result = _rssFeedCreator.CreateRssFeed(ShowType.MovieFavourites);
            return result.ToRss();
        }

        public IActionResult RssSwitch()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            var result = _rssFeedCreator.CreateRssFeed(ShowType.GameSwitch);
            return result.ToRss();
        }

        public IActionResult RssPC()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            var result = _rssFeedCreator.CreateRssFeed(ShowType.GamePC);
            return result.ToRss();
        }

        public IActionResult RssPS4()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            var result = _rssFeedCreator.CreateRssFeed(ShowType.GamePS4);
            return result.ToRss();
        }

        public IActionResult RssFavourites()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            var result = _rssFeedCreator.CreateRssFeed(ShowType.Favourite);
            return result.ToRss();
        }

        public IActionResult Json()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            var model = _recentlyAddedShows.GetModel();
            return new JsonResult(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogWarning($"Home/{MethodBase.GetCurrentMethod().Name} was called");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }

    public static class Extensions
    {
        public static FileContentResult ToRss(this byte[] result)
        {
            var file = new FileContentResult(result, "application/rss+xml;charset=utf-8");

            return file;
        }
    }
}

