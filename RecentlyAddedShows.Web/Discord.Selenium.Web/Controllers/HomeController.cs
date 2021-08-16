using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Selenium.Web.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Discord.Selenium.Web.Controllers
{
    [ApiController]
    [Route("")]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            var context = new AutomatedDiscordContext();
            return new JsonResult(context.logs.OrderByDescending(x => x.Created));
        }

        [Route("Clear")]
        public IActionResult Clear()
        {
            var context = new AutomatedDiscordContext();
            var logs = context.logs;
            context.RemoveRange(logs);
            context.SaveChanges();
            
            return new JsonResult("Successfully cleared logs");
        }
    }
}
