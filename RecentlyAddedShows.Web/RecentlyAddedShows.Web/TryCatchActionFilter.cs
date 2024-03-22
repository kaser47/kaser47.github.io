using Microsoft.AspNetCore.Mvc.Filters;
using RecentlyAddedShows.Service.Data.Entities;
using RecentlyAddedShows.Service.Data;
using System;
using System.Text;

public class TryCatchActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Executed before the action method
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Executed after the action method

        if (context.Exception != null)
        {
            // Exception occurred during the action method execution
            // Handle the exception here
            context.Exception.SaveErrorDetails();

            // Optionally, you can modify the result or re-throw the exception
            // Example: context.Result = new StatusCodeResult(500); // Set a custom status code
            // Or: throw context.Exception; // Re-throw the exception
        }
    }
}

public static class ExceptionExtensions
{
    public static ErrorMessage FindInnerException(this Exception ex)
    {
        if (ex.InnerException != null)
        {

            return ex.InnerException.FindInnerException();
        }

        return new ErrorMessage(ex.Message, ex.StackTrace);
    }

    public static void SaveErrorDetails(this Exception ex)
    {
        var dbContext = new Context();
        ErrorDetails error = new ErrorDetails(ex);

        dbContext.ErrorDetails.Add(error);
        dbContext.SaveChanges();
    }

    public static string GetAllErrors(Exception ex)
    {
        StringBuilder sb = new StringBuilder();

        while (ex != null)
        {
            sb.AppendLine("Error Message: " + ex.Message);
            sb.AppendLine("Stack Trace: " + ex.StackTrace);
            sb.AppendLine();

            ex = ex.InnerException;
        }

        return sb.ToString();
    }
}