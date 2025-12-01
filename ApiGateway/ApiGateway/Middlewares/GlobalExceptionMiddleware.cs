using Microsoft.AspNetCore.Mvc;
using Serilog;
using CorrelationId;
using Correlate;

namespace ApiGateway.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var correlationId = context.Features.Get<ICorrelationContextAccessor>()
                    ?.CorrelationContext?.CorrelationId
                    ?? context.TraceIdentifier;

                Log.Error(ex,
                "Unhandled exception occurred. CorrelationId: {CorrelationId}, Path: {Path}",
                correlationId,
                context.Request.Path);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = 500,
                    Title = "Internal Server Error",
                    Detail = ex.Message
                });
            }
        }
    }

}
