using System;
using RecentlyAddedShows.Web.Classes;

namespace RecentlyAddedShows.Web.Data.Entities
{
    public class Show
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public DateTime Created { get; set; }
        public int NumberViewing { get; set; }
        public bool IsUpdated { get; set; }

        public string TranslatedCreated
        {
            get
            {
                var now = DateTime.UtcNow;
                var lastUpdated = Created;

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
                    var result = TimeLeft().Hours;
                    return (result > 1) ? $"{result} hours ago" : $"{result} hour ago";
                }

                if (TimeLeft().Minutes > 0)
                {
                    var result = TimeLeft().Minutes;
                    return (result > 1) ? $"{result} minutes ago" : $"{result} minute ago";
                }

                if (TimeLeft().Seconds <= 0) return string.Empty;
                {
                    var result = TimeLeft().Seconds;
                    return (result > 1) ? $"{result} seconds ago" : $"{result} second ago";
                }

            }
        }

        public Show(string name, string url, string image, ShowType type, DateTime created, int numberViewing)
        : this(name, url, image, type, created)
        {
            NumberViewing = numberViewing;
        }

        public Show(string name, string url, string image, ShowType type, DateTime created)
        {
            Name = name;
            Url = url;
            Image = image;
            Type = type.ToString();
            Created = created;
        }

        public Show()
        {
            
        }

        public override string ToString()
        {
            var message = $"{Type}-{Name}-{Created}";
            return message;
        }
    }
}