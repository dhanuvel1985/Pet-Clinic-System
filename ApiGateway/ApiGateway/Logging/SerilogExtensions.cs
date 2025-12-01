using Serilog;

namespace ApiGateway.Logging
{
    public static class SerilogExtensions
    {
        public static void ConfigureSerilog(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog(static (context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithCorrelationId()
                    .WriteTo.Console()
                    .WriteTo.Seq("http://localhost:5341");
            });
        }
    }

}
