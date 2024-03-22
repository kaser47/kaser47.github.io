using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var exemptedViewPath = "/Views/Home/Index.cshtml";

        var path = context.Request.Path;
        if (path.Equals(exemptedViewPath, StringComparison.OrdinalIgnoreCase))
        {
            // Skip logging for the exempted view
            await _next(context);
        }
        else
        {
            // Log the request
            _logger.LogInformation($"Request to {context.Request.Path} received.");
            await _next(context);
        }
    }
}