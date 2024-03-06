using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace RecentlyAddedShows.Service.Classes
{
    public static class Consts
    {
        public const string ApiKey = "10b8b8b339f213d5a7c04a44ad8208de";
        public const string WebsiteAddress = "http://recentlyaddedshows.somee.com/";
        public const string TMDBCDN = "https://media.themoviedb.org/t/p/w260_and_h390_bestv2";
        public const string TMDBUrlLink = "https://www.themoviedb.org/movie/{0}?language=en-GB";

        public static string Html {  get
            {
                return "<html>\r\n<head>\r\n    <title>White Table with White Text</title>\r\n    <style>\r\n        table {\r\n            border-collapse: collapse;\r\n            width: 100%;\r\n            color: white;  /* Text color */\r\n        }\r\n        th {\r\n            background-color: #000;  /* Header background color */\r\n        }\r\n    </style>\r\n</head>";
            } }

        public static string HtmlEnd
        {
            get
            {
                return "</body></html>";
            }
        }
    }
}
