namespace ApiGateway.Configurations
{
    public class JwtOptions
    {
        public string Authority { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public bool RequireHttpsMetadata { get; set; } = true;
    }
}
