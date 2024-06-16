namespace TeamRanking.ExceptionHandling.Interface
{
    public interface IExceptionHandler
    {
        Task HandleExceptionAsync(HttpContext context, Exception ex, RequestDelegate next);
    }
}
