namespace AmazonBestSellers.Application.Common.Constants;

public static class EnvironmentVariables
{
    public const string JwtSecret = "JWT_SECRET";
    public const string JwtIssuer = "JWT_ISSUER";
    public const string JwtAudience = "JWT_AUDIENCE";
    public const string JwtExpireMinutes = "JWT_EXPIRE_MINUTES";

    public const string RapidApiKey = "RAPIDAPI_KEY";
    public const string RapidApiHost = "RAPIDAPI_HOST";
    public const string RapidApiBaseUrl = "RAPIDAPI_BASE_URL";

    public const string CorsAllowedOrigins = "CORS_ALLOWED_ORIGINS";

    public const string DbHost = "DB_HOST";
    public const string DbPort = "DB_PORT";
    public const string DbName = "DB_NAME";
    public const string DbUser = "DB_USER";
    public const string DbPassword = "DB_PASSWORD";
}
