using Serilog.Context;

namespace ApiGateway.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private const string CorrelationHeader = "X-Correlation-ID";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(CorrelationHeader, out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                context.Response.Headers.Add(CorrelationHeader, correlationId);
            }

            LogContext.PushProperty("CorrelationId", correlationId.ToString());

            await _next(context);
        }
    }
}
