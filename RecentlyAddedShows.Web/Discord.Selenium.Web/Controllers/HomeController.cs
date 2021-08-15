using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Selenium.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            var result = new AutomatedDiscord(Consts.webDriverLocation());

            return new JsonResult($"Running Cron Jobs - {result.Display()}");
        }
    }
}
