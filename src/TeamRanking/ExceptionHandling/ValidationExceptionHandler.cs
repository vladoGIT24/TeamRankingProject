using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TeamRanking.ExceptionHandling.Interface;

namespace TeamRanking.ExceptionHandling
{
    public class ValidationExceptionHandler : IExceptionHandler
    {
        private readonly IExceptionHandler _next;

        public ValidationExceptionHandler(IExceptionHandler next)
        {
            _next = next;
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception ex, RequestDelegate nextDelegate)
        {
            if (ex is ValidationException)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, "Validation error");
            }
            else
            {
                await _next.HandleExceptionAsync(context, ex, nextDelegate);
            }
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
