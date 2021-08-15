using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.Selenium.Web
{
    public static class Consts
    {
        public static string webDriverLocation()
        {
            var runDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var location = runDir + @"\Resources\";
            return location;
        }
    }
}
