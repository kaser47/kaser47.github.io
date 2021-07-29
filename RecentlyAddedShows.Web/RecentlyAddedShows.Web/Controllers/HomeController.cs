using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecentlyAddedShows.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
            var results = RecentlyAddedShows.Get();

            var websiteNames = results.Select(x => x.Name).OrderBy(x => x).ToList();
            var savedNames = dbContext.Shows.Select(x => x.Name).OrderBy(x => x).ToList();

            if (!websiteNames.SequenceEqual(savedNames))
            {
                foreach (var show in dbContext.Shows)
                {
                    dbContext.Shows.Remove(show);
                }

                dbContext.Shows.AddRange(results);
                dbContext.SaveChanges();
            }

            results = dbContext.Shows.ToList();

            var model = new RecentlyAddedModel(results);

            return View(model);
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
    }
}
