using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecentlyAddedShows.Web.Models;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using RecentlyAddedShows.Web.Classes;
using RecentlyAddedShows = RecentlyAddedShows.Web.Classes.RecentlyAddedShows;

namespace RecentlyAddedShows.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Classes.RecentlyAddedShows _recentlyAddedShows;
        private readonly Classes.RssFeedCreator _rssFeedCreator;
        private readonly IOptions<Configuration> _config;

        public HomeController(ILogger<HomeController> logger, IOptions<Configuration> config)
        {
            _logger = logger;
            _config = config;
            _recentlyAddedShows = new Classes.RecentlyAddedShows(_config);
            _rssFeedCreator = new RssFeedCreator(_config);
        }

        public IActionResult Index()
        {
            var model = _recentlyAddedShows.LoadModel();
            return View(model);
        }

        public IActionResult Rss()
        {
            var result = _rssFeedCreator.CreateCartoonRssFeed();
            return result;
        }

        public IActionResult RssUpNext()
        {
            var result = _rssFeedCreator.CreateRssFeed(ShowType.TVShowUpNext);
            return result;
        }

        public IActionResult RssPopularMovies()
        {
            var result = _rssFeedCreator.CreatePopularRssFeed(ShowType.MoviePopular);
            return result;
        }

        public IActionResult RssPopularShows()
        {
            var result = _rssFeedCreator.CreatePopularRssFeed(ShowType.TVShowPopular);
            return result;
        }

        public IActionResult RssShowCollection()
        {
            var result = _rssFeedCreator.CreateRssFeed(ShowType.TVShowCollection);
            return result;
        }

        public IActionResult RssMovieWatchlist()
        {
            var result = _rssFeedCreator.CreateRssFeed(ShowType.MovieFavourites);
            return result;
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
}

