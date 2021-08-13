using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace RecentlyAddedShows.Test
{
    [TestFixture]
    public class RecentlyAddedShowsTests
    {
        [Test]
        public void Discord()
        {
            var discord = new Discord.Class1();
            discord.Run();
        }

        [Test]
        public void GetShows()
        {
            var recentlyAddedShows = new Web.Classes.RecentlyAddedShows(null);
            recentlyAddedShows.Get();
            Assert.IsTrue(true);
        }

        List<string> messages = new List<string>();

        [Test]
        public void Dates()
        {
            var rand = new Random();
            var number = rand.Next(1, 59);

            var now = DateTime.Now;

            var Dates = new List<DateTime>()
            {
                now.AddSeconds(-number),
                now.AddMinutes(-number),
                now.AddHours(-number),
                now.AddDays(-number),
                now.AddMonths(-number),
                now.AddYears(-number)
            };

            foreach (var date in Dates)
            {
                SortDates(date);
            }
        }

        private void SortDates(DateTime lastUpdated)
        {
            var now = DateTime.Now;

            TimeSpan TimeLeft()
            {
               return (now - lastUpdated);
            }

            if (TimeLeft().Days > 0)
            {
                int result = TimeLeft().Days;
                messages.Add($"{result} Days ago");
            }

            if (TimeLeft().Hours > 0)
            {
                int result = TimeLeft().Hours;
                messages.Add($"{result} Hours ago");
            }

            if (TimeLeft().Minutes > 0)
            {
                int result = TimeLeft().Minutes;
                messages.Add($"{result} Minutes ago");
            }

            if (TimeLeft().Seconds > 0)
            {
                int result = (lastUpdated - now).Seconds;
                messages.Add($"{result} Seconds ago");
            }
        }
    }
}