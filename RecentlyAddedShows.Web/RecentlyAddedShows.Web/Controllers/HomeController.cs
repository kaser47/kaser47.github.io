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
            var model = _recentlyAddedShows.LoadModel();
            string TranslatedLastUpdated = model.TranslatedLastUpdated();
            string LastUpdated = model.LastUpdated().ToString();
            var data = new { TranslatedLastUpdated, LastUpdated };
            return Json(data);
        }

        public IActionResult Clear()
        {
            _recentlyAddedShows.ClearErrors();
            return RedirectToAction("Index");
        }

        public IActionResult Refresh()
        {
            var model = _recentlyAddedShows.GetModel();
            return View("Index", model);
        }

        public IActionResult Update()
        {

            ContentResult result = new ContentResult
            {
                Content = "Success",
                ContentType = "text/plain"
            };

            var viewModel = _recentlyAddedShows.GetModel();


            if (viewModel.ShowInHtml != null)
            {
                result.Content = viewModel.ShowInHtml.ToString();
            }

            return result;
        }

        public IActionResult RssAnimation()
        {
            var result = _rssFeedCreator.CreateCartoonRssFeed();

            return result.ToRss();
        }

        public IActionResult RssUpNext()
        {
            var result = _rssFeedCreator.CreateRssFeed(ShowType.TVShowUpNext);
            return result.ToRss();
        }

        public IActionResult RssRecentlyAired()
        {
            var result = _rssFeedCreator.CreateRssFeed(ShowType.TVShowRecentlyAired);
            return result.ToRss();
        }


        public IActionResult RssPopularMovies()
        {
            var result = _rssFeedCreator.CreatePopularRssFeed(ShowType.MoviePopular);
            return result.ToRss();
        }


        public IActionResult RssPopularShows()
        {
            var result = _rssFeedCreator.CreatePopularRssFeed(ShowType.TVShowPopular);
            return result.ToRss();
        }

        public IActionResult RssReleaseDates()
        {
            var result = _rssFeedCreator.CreateRssFeed(ShowType.ReleaseDate);
            return result.ToRss();
        }

        public IActionResult RssShowCollection()
        {
            var result = _rssFeedCreator.CreateRssFeed(ShowType.TVShowCollection);
            return result.ToRss();
        }

        public IActionResult RssMovieWatchlist()
        {
            var result = _rssFeedCreator.CreateRssFeed(ShowType.MovieFavourites);
            return result.ToRss();
        }

        public IActionResult RssSwitch()
        {
            var result = _rssFeedCreator.CreateRssFeed(ShowType.GameSwitch);
            return result.ToRss();
        }

        public IActionResult RssPC()
        {
            var result = _rssFeedCreator.CreateRssFeed(ShowType.GamePC);
            return result.ToRss();
        }

        public IActionResult RssPS4()
        {
            var result = _rssFeedCreator.CreateRssFeed(ShowType.GamePS4);
            return result.ToRss();
        }

        public IActionResult RssFavourites()
        {
            var result = _rssFeedCreator.CreateRssFeed(ShowType.Favourite);
            return result.ToRss();
        }

        public IActionResult Json()
        {
            var model = _recentlyAddedShows.GetModel();
            return new JsonResult(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
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

