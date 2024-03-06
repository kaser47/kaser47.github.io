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
        public DateTime? DeletedDate { get; set; }
        public bool IsChecked { get; set; }
        public bool ShowInHtml { get; set; }

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

        public DateTime? ReleaseDate { get; set; }

        public virtual bool hasReleaseDate { get { return ReleaseDate.HasValue; } }
        public virtual bool hasDeletedDate { get { return DeletedDate.HasValue; } }

        public string TranslatedReleaseDate
        {
            get {
                if (ReleaseDate.HasValue)
                {
                    var now = DateTime.UtcNow;
                    var timeleft = TimeSpanToDateParts(now, ReleaseDate.Value);
                    return timeleft.ToString();
                }
                return null;
            }
        }

        public static TimeDifference TimeSpanToDateParts(DateTime d1, DateTime d2)
        {
            int years, months, days, hours, minutes, seconds;
            bool isFuture = false;
            if (d1 < d2)
            {
                var d3 = d2;
                d2 = d1;
                d1 = d3;
                isFuture = true;
            }

            var span = d1 - d2;

            months = 12 * (d1.Year - d2.Year) + (d1.Month - d2.Month);

            //month may need to be decremented because the above calculates the ceiling of the months, not the floor.
            //to do so we increase d2 by the same number of months and compare.
            //(500ms fudge factor because datetimes are not precise enough to compare exactly)
            if (d1.CompareTo(d2.AddMonths(months).AddMilliseconds(-500)) <= 0)
            {
                --months;
            }

            years = months / 12;
            months -= years * 12;

            if (months == 0 && years == 0)
            {
                days = span.Days;
            }
            else
            {
                var md1 = new DateTime(d1.Year, d1.Month, d1.Day);
                var md2 = new DateTime(d2.Year, d2.Month, d2.Day);

                days = (int)(md1.AddMonths(-(months)).AddYears(-((years))) - md2).TotalDays;
            }
            hours = span.Hours;
            minutes = span.Minutes;
            seconds = span.Seconds;

            return new TimeDifference(years, months, days, hours, minutes, seconds, isFuture);
        }



    public string TranslatedCreated
        {
            get
            {
                var now = DateTime.UtcNow;
                var timeleft = TimeSpanToDateParts(now, Created);
                return timeleft.ToString();
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

            if ((Type == ShowType.MoviePopular.ToString() || Type == ShowType.InTheatre.ToString()) && Name != null)
            {
                var searchableName = Name.Substring(0, Name.Length - 5);
                var date = Name.Substring(Name.Length - 4, 4);
                ReleaseDate = MovieReleaseDateFinder.GetDetailsAsync(searchableName, date).Result;
            }
        }

        public Show(Show show, string showType = "Favourite")
        {
            Name = show.Name;
            Url = show.Url;
            Image = show.Image;
            Type = showType;

            if (showType == ShowType.Favourite.ToString())
            {
                Created = DateTime.UtcNow;
            }
            else if (showType == ShowType.ReleaseDate.ToString())
            {
                Created = show.ReleaseDate.Value;
            }
            else
            {
                Created = show.Created;
            }
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

    public class TimeDifference
    {
        public int Years { get; set; }
        public int Months { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public bool IsFuture { get; set; }
        public TimeDifference(int years, int months, int days, int hours, int minutes, int seconds, bool isFuture)
        {
            Years = years;
            Months = months;
            Days = days;
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            IsFuture = isFuture;
        }

        public override string ToString()
        {
            string result = "";
            if (Years > 0)
            {
                result = $"{Years} year(s), {Months} month(s), {Days} day(s) ago";
            }
            else if (Months > 0)
            {
                result = $"{Months} month(s), {Days} day(s) ago";
            }
            else if (Days > 0)
            {
                result = $"{Days} day(s), {Hours} hour(s), {Minutes} minute(s) ago";
            }
            else if (Hours > 0)
            {
                result = $"{Hours} hour(s), {Minutes} minute(s) ago";
            }
            else if (Minutes > 0)
            {
                result = $"{Minutes} minutes(s), {Seconds} second(s) ago";
            }
            else if (Seconds > 0)
            {
                result = $"{Seconds} second(s) ago";
            }
            else
            {
                result = "Less than a second ago";
            }

            if (IsFuture)
            {
                result = result.Replace("ago", "until release");
            }

            return result;
        }
    }
}