using TeamRanking.ExceptionHandling;
using TeamRanking.ExceptionHandling.Interface;

namespace TeamRanking.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IExceptionHandler _handlerChain;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Setup the chain of responsibility
            _handlerChain = new ValidationExceptionHandler(
                new BadRequestExceptionHandler(
                    new KeyNotFoundExceptionHandler(
                        new InvalidOperationExceptionHandler(
                            new GenericExceptionHandler()))));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unhandled exception: {ex.Message}");
                await _handlerChain.HandleExceptionAsync(context, ex, _next);
            }
        }
    }
}