using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace AuthService.Api.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ProblemDetailsFactory _problemFactory;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ProblemDetailsFactory problemFactory, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _problemFactory = problemFactory;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in AuthService");
                var pd = _problemFactory.CreateProblemDetails(context, statusCode: 500, title: "Internal Server Error", detail: ex.Message);
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(pd);
            }
        }
    }
}
