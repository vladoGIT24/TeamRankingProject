using Microsoft.AspNetCore.Mvc;
using System.Net;
using TeamRanking.ExceptionHandling.Interface;

namespace TeamRanking.ExceptionHandling
{
    public class GenericExceptionHandler : IExceptionHandler
    {
        public async Task HandleExceptionAsync(HttpContext context, Exception ex, RequestDelegate nextDelegate)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, "Internal server error");
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode statusCode, string title)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = ex.Message,
            };

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
