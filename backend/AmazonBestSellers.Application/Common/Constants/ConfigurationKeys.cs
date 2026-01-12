namespace AmazonBestSellers.Application.Common.Constants;

public static class ConfigurationKeys
{
    public const string JwtSecret = "Jwt:Secret";
    public const string JwtIssuer = "Jwt:Issuer";
    public const string JwtAudience = "Jwt:Audience";
    public const string JwtExpireMinutes = "Jwt:ExpireMinutes";

    public const string RapidApiKey = "RapidAPI:Key";
    public const string RapidApiHost = "RapidAPI:Host";
    public const string RapidApiBaseUrl = "RapidAPI:BaseUrl";

    public const string CorsAllowedOrigins = "Cors:AllowedOrigins:0";
}
