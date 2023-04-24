using System;
using RecentlyAddedShows.Service.Classes;

namespace RecentlyAddedShows.Service.Data.Entities
{
    public class Show
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public DateTime Created { get; set; }
        public virtual DateTime PublishiedDate { get {
                var utc = Created;
                var localTime = TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.Local);
                var englishTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                var currentEnglishTime = TimeZoneInfo.ConvertTimeFromUtc(utc, englishTimeZoneInfo);
                if (!TimeZoneInfo.Local.IsDaylightSavingTime(currentEnglishTime) && englishTimeZoneInfo.IsDaylightSavingTime(currentEnglishTime))
                {
                    localTime = localTime.AddHours(1);
                }
                
                return localTime; } }

        public int NumberViewing { get; set; }
        public bool IsUpdated { get; set; }

        public string TranslatedCreated
        {
            get
            {
                var now = DateTime.UtcNow;

                TimeSpan TimeLeft()
                {
                    return (now - Created);
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

                if (TimeLeft().Seconds <= 0)
                {
                    var result = TimeLeft().Seconds;
                    return (result > 1) ? $"{result} seconds ago" : $"{result} second ago";
                }

                return string.Empty;
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

            var utcTime  = TimeZoneInfo.ConvertTimeToUtc(created);
            Created = utcTime;
        }

        public Show(Show show)
        {
            Name = show.Name;
            Url = show.Url;
            Image = show.Image;
            Type = ShowType.Favourite.ToString();
            Created = show.Created;
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

    public class ErrorMessage
    {
        public int id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime Created { get; set; }

        public ErrorMessage(string message, string stackTrace)
        {
            this.Message = message;
            this.StackTrace = stackTrace;
            Created = DateTime.UtcNow;
        }
    }

    public class Favourite
    {
        public int id { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }

        public Favourite(string title)
        {
            this.Title = title;
            Created = DateTime.UtcNow;
        }

        public Favourite()
        {

        }
    }
}