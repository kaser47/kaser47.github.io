using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace RecentlyAddedShows.Web
{
    public class Show
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public DateTime Created { get; set; }

        public Show(string name, string url, string image, RecentlyAddedShows.ShowType type)
        {
            Name = name;
            Url = url;
            Image = image;
            Type = type.ToString();
            Created = DateTime.Now;
        }

        public Show()
        {
            
        }
    }

    public class Context : DbContext
    {
        private const string connectionString =
            "Server=sql.bsite.net\\MSSQL2016;Database=kaser47_RecentlyAddedShows;User Id=kaser47_RecentlyAddedShows;Password=123qwe";
        public Context() : base(connectionString)
        {

        }
        
        public DbSet<Show> Shows { get; set; }
    }

    public class RecentlyAddedModel
    {
        public IEnumerable<Show> Shows { get; set; }

        public IEnumerable<Show> Cartoons
        {
            get
            {
                return Shows.Where(x => x.Type == RecentlyAddedShows.ShowType.Cartoon.ToString()).OrderBy(x => x.Name);
            }
        }

        public IEnumerable<Show> Anime
        {
            get
            {
                return Shows.Where(x => x.Type == RecentlyAddedShows.ShowType.Anime.ToString()).OrderBy(x => x.Name);
            }
        }

        public DateTime LastUpdated => Shows.FirstOrDefault().Created;

        public string TranslatedLastUpdated => SortDates(LastUpdated);

        public RecentlyAddedModel(IEnumerable<Show> shows)
        {
            Shows = shows;
        }

        private string SortDates(DateTime lastUpdated)
        {
            var now = DateTime.Now;

            TimeSpan TimeLeft()
            {
                return (now - lastUpdated);
            }

            if (TimeLeft().Days > 0)
            {
                var result = TimeLeft().Days;
                return (result > 1) ? $"{result} days ago" : $"{result} day ago";
            }

            if (TimeLeft().Hours > 0)
            {
                int result = TimeLeft().Hours;
                return (result > 1) ? $"{result} hours ago" : $"{result} hour ago";
            }

            if (TimeLeft().Minutes > 0)
            {
                int result = TimeLeft().Minutes;
                return (result > 1) ? $"{result} minutes ago" : $"{result} minute ago";
            }

            if (TimeLeft().Seconds > 0)
            {
                int result = (lastUpdated - now).Seconds;
                return (result > 1) ? $"{result} seconds ago" : $"{result} second ago";
            }

            return String.Empty;
        }
    }
}