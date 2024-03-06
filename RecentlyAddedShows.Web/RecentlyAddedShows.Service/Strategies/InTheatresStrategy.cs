using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;
using RecentlyAddedShows.Service.Classes;
using RecentlyAddedShows.Service.Data.Entities;
using RecentlyAddedShows.Service.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace RecentlyAddedShows.Service.Strategies
{
    public class InTheatresStrategy : IStrategy
    {
        public InTheatresStrategy()
        {

        }

        public ConcurrentBag<Show> GetShows(DateTime date)
        {
            return ApiMethod(date).Result;
        }

        public static async Task<ConcurrentBag<Show>> ApiMethod(DateTime date)
        {
            try
            {
                var listOfUrls = new List<string>();

                listOfUrls.Add($"https://api.themoviedb.org/3/movie/now_playing?api_key={Consts.ApiKey}&language=en-US&page=1");
                listOfUrls.Add($"https://api.themoviedb.org/3/movie/now_playing?api_key={Consts.ApiKey}&language=en-US&page=2");
                listOfUrls.Add($"https://api.themoviedb.org/3/movie/now_playing?api_key={Consts.ApiKey}&language=en-US&page=3");

                var shows = new ConcurrentBag<Show>();

                foreach (var url in listOfUrls)
                {
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JObject searchJson = JObject.Parse(responseBody);
                    JArray results = JArray.Parse(searchJson.GetValue("results").ToString());

                    foreach (var result in results)
                    {
                        var name = result.SelectToken("title").ToString();
                        var movieDate = result.SelectToken("release_date").ToString().Substring(0, 4);
                        var formattedName = $"{name} {movieDate}";
                        var urlValue = String.Format(Consts.TMDBUrlLink, result.SelectToken("id").ToString());
                        var imageValue = $"{Consts.TMDBCDN}{result.SelectToken("poster_path")}";
                        shows.Add(new Show(formattedName, urlValue, imageValue, ShowType.InTheatre, date));
                    }
                }

                return shows;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
