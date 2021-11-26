using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using RecentlyAddedShows.Service;
using RecentlyAddedShows.Service.Classes;
using RecentlyAddedShows.Service.Models;
using RssFeedCreator = RecentlyAddedShows.Service.Classes.RssFeedCreator;

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

        public IActionResult Rss()
        {
            var result = _rssFeedCreator.CreateCartoonRssFeed();

            return result.ToRss();
        }

        public IActionResult RssUpNext()
        {
            var result = _rssFeedCreator.CreateRssFeed(ShowType.TVShowUpNext);
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

