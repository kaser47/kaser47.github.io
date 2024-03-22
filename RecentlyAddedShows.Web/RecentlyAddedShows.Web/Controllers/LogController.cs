using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using RecentlyAddedShows.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Reflection;

public class LogController : Controller
{
    private readonly string _connectionString;
    private readonly ILogger<FavouritesController> _logger;

    public LogController(ILogger<FavouritesController> logger)
    {
        _logger = logger;
        // Set your SQL Server connection string here
        _connectionString = "Server=sql.bsite.net\\MSSQL2016;Database=recentlyaddedshows_ras;User Id=recentlyaddedshows_ras; Password=123qweasd;TrustServerCertificate=True;";
    }


    public IActionResult Index(DateTime? startDate, DateTime? endDate, string logLevel, string keyword)
    {
        var logs = AllLogs();
        return View(logs);
    }

    public IActionResult Search(DateTime? startDate, DateTime? endDate, string logLevel, string keyword)
    {
        var logs = SearchLogs(startDate, endDate, logLevel, keyword);
        return View("Index", logs);
    }

    private IEnumerable<string> AllLogs()
    {
        _logger.LogWarning($"Log/{MethodBase.GetCurrentMethod().Name} was called");
        var logs = new List<string>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var sql = "SELECT [Timestamp], [Level], [Message], [Properties] " +
                      "FROM [Logs] " +
                      "ORDER BY 1 DESC";

            using (var command = new SqlCommand(sql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Read log data from the database
                        // Adjust this based on your table structure
                        var logEntry = $"{reader["Timestamp"]} ---- {reader["Level"]} ---- {reader["Message"]}";
                        logs.Add(logEntry);
                    }
                }
            }
        }

        return logs;
    }

    private IEnumerable<string> SearchLogs(DateTime? startDate, DateTime? endDate, string logLevel, string keyword)
    {
        _logger.LogWarning($"Log/{MethodBase.GetCurrentMethod().Name} was called");
        var logs = new List<string>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var sql = "SELECT [Timestamp], [Level], [Message], [Properties] " +
                      "FROM [Logs] " +
                      "WHERE 1=1 ";

            if (startDate.HasValue)
            {
                sql += "AND [Timestamp] >= @StartDate ";
            }

            if (endDate.HasValue)
            {
                sql += "AND [Timestamp] <= @EndDate ";
            }

            if (!string.IsNullOrEmpty(logLevel))
            {
                sql += "AND [Level] = @LogLevel ";
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                sql += "AND [MessageTemplate] LIKE '%' + @Keyword + '%' ";
            }

            sql += "ORDER BY 1 DESC";

            using (var command = new SqlCommand(sql, connection))
            {
                if (startDate.HasValue)
                {
                    command.Parameters.AddWithValue("@StartDate", startDate.Value);
                }

                if (endDate.HasValue)
                {
                    command.Parameters.AddWithValue("@EndDate", endDate.Value);
                }

                if (!string.IsNullOrEmpty(logLevel))
                {
                    command.Parameters.AddWithValue("@LogLevel", logLevel);
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    command.Parameters.AddWithValue("@Keyword", keyword);
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Read log data from the database
                        // Adjust this based on your table structure
                        var logEntry = $"{reader["Timestamp"]} ---- {reader["Level"]} ---- {reader["Message"]}";
                        logs.Add(logEntry);
                    }
                }
            }
        }



        return logs;
    }
}