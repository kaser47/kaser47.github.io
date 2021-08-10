using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecentlyAddedShows.Web.Models;
using System.Diagnostics;
using RecentlyAddedShows.Web.Classes;

namespace RecentlyAddedShows.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = Classes.RecentlyAddedShows.GetModel();
            return View(model);
        }

        public IActionResult Rss()
        {
            var result = RssFeedCreator.CreateRssFeed();
            return result;
        }

        public IActionResult Json()
        {
            var model = Classes.RecentlyAddedShows.GetModel();
            return new JsonResult(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

