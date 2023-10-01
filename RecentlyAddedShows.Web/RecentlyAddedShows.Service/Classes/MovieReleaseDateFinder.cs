using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RecentlyAddedShows.Service.Classes
{
    public static class MovieReleaseDateFinder
    {
        const string ApiKey = "10b8b8b339f213d5a7c04a44ad8208de";
        public static async Task<DateTime?> GetDetailsAsync(string title, string year)
        {
            int nowYear = DateTime.UtcNow.Year;
            int monvieReleaseYear = int.Parse(year);

            try
            {
            var encodedTitle = HttpUtility.UrlEncode(title);
            string searchUrl = $"https://api.themoviedb.org/3/search/movie?api_key={ApiKey}&language=en-US&query={encodedTitle}&page=1";

            //Search
            HttpClient client = new HttpClient();
            HttpResponseMessage searchResponse = await client.GetAsync(searchUrl);
            searchResponse.EnsureSuccessStatusCode();
            string searchResponseBody = await searchResponse.Content.ReadAsStringAsync();
            JObject searchJson = JObject.Parse(searchResponseBody);
            int totalSearchResults = int.Parse(searchJson.GetValue("total_results").ToString());
            if (totalSearchResults == 0)
                return null;

            JArray searchResults = JArray.Parse(searchJson.GetValue("results").ToString());
            JToken firstSearchResult = searchResults.Where(x => x.SelectToken("release_date").ToString().Contains(year)).FirstOrDefault();

            var movieId = firstSearchResult.SelectToken("id").ToString();

            string releaseDatesUrl = $"https://api.themoviedb.org/3/movie/{movieId}/release_dates?api_key={ApiKey}";

            //Release Dates
            HttpResponseMessage releaseDatesResponse = await client.GetAsync(releaseDatesUrl);
            releaseDatesResponse.EnsureSuccessStatusCode();
            string releaseDatesResponseBody = await releaseDatesResponse.Content.ReadAsStringAsync();
            JObject releaseDatesJson = JObject.Parse(releaseDatesResponseBody);
            JToken releaseDatesresults = releaseDatesJson.GetValue("results");

            List<DateTime> releaseDatesPerRegion = new List<DateTime>();
            foreach (var releaseDateResult in releaseDatesresults)
            {
                try
                {
                    JToken allReleaseDatesForRegion = releaseDateResult.SelectToken("release_dates");
                    JArray arrayOfReleaseDates = JArray.Parse(allReleaseDatesForRegion.ToString());
                    var validReleaseDates = arrayOfReleaseDates.Where(x => x.SelectToken("type").ToString() == "4" || x.SelectToken("type").ToString() == "5").ToList();
                    var earliestReleaseDate = validReleaseDates.OrderBy(x => DateTime.Parse(x.SelectToken("release_date").ToString())).Select(x => DateTime.Parse(x.SelectToken("release_date").ToString())).First();
                    releaseDatesPerRegion.Add(earliestReleaseDate);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

                if (releaseDatesPerRegion.Count == 0)
                {
                    int difference = nowYear - monvieReleaseYear;
                    if ((difference) >= 2)
                    {
                        DateTime newDate = DateTime.Now.AddYears(-difference);
                        return newDate;
                    }

                    return null;
                }

            var earliestPossibleReleaseDate = releaseDatesPerRegion.OrderBy(x => x).First();
            var utcReleaseDate = TimeZoneInfo.ConvertTimeToUtc(earliestPossibleReleaseDate); 
            
            return utcReleaseDate;

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
