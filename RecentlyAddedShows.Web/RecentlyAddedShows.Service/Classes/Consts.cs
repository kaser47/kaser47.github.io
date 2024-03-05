using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace RecentlyAddedShows.Service.Classes
{
    public static class Consts
    {
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
