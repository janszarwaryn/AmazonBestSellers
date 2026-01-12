using AmazonBestSellers.API.Extensions;
using AmazonBestSellers.API.Middleware;
using AmazonBestSellers.Application.Common.Constants;
using AmazonBestSellers.Infrastructure.Data;
using AspNetCoreRateLimit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using DotNetEnv;

var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}

var requiredEnvVars = new[]
{
    EnvironmentVariables.JwtSecret,
    EnvironmentVariables.JwtIssuer,
    EnvironmentVariables.JwtAudience,
    EnvironmentVariables.JwtExpireMinutes,
    EnvironmentVariables.DbHost,
    EnvironmentVariables.DbPort,
    EnvironmentVariables.DbName,
    EnvironmentVariables.DbUser,
    EnvironmentVariables.DbPassword
};

var missingVars = requiredEnvVars
    .Where(varName => string.IsNullOrEmpty(Environment.GetEnvironmentVariable(varName)))
    .ToList();

if (missingVars.Any())
{
    throw new InvalidOperationException(
        $"missing required environment variables: {string.Join(", ", missingVars)}"
    );
}

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    [ConfigurationKeys.JwtSecret] = Environment.GetEnvironmentVariable(EnvironmentVariables.JwtSecret),
    [ConfigurationKeys.JwtIssuer] = Environment.GetEnvironmentVariable(EnvironmentVariables.JwtIssuer),
    [ConfigurationKeys.JwtAudience] = Environment.GetEnvironmentVariable(EnvironmentVariables.JwtAudience),
    [ConfigurationKeys.JwtExpireMinutes] = Environment.GetEnvironmentVariable(EnvironmentVariables.JwtExpireMinutes),
    [ConfigurationKeys.RapidApiKey] = Environment.GetEnvironmentVariable(EnvironmentVariables.RapidApiKey),
    [ConfigurationKeys.RapidApiHost] = Environment.GetEnvironmentVariable(EnvironmentVariables.RapidApiHost),
    [ConfigurationKeys.RapidApiBaseUrl] = Environment.GetEnvironmentVariable(EnvironmentVariables.RapidApiBaseUrl),
    [ConfigurationKeys.CorsAllowedOrigins] = Environment.GetEnvironmentVariable(EnvironmentVariables.CorsAllowedOrigins)
});

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var connectionString = $"Server={Environment.GetEnvironmentVariable(EnvironmentVariables.DbHost)};" +
                       $"Port={Environment.GetEnvironmentVariable(EnvironmentVariables.DbPort)};" +
                       $"Database={Environment.GetEnvironmentVariable(EnvironmentVariables.DbName)};" +
                       $"User={Environment.GetEnvironmentVariable(EnvironmentVariables.DbUser)};" +
                       $"Password={Environment.GetEnvironmentVariable(EnvironmentVariables.DbPassword)};";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

builder.Services.Configure<AspNetCoreRateLimit.IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<AspNetCoreRateLimit.IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<AspNetCoreRateLimit.IRateLimitConfiguration, AspNetCoreRateLimit.RateLimitConfiguration>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSerilogRequestLogging();

app.UseMiddleware<SecurityHeadersMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
})).AllowAnonymous();

app.UseHttpsRedirection();

// Static files + cache
app.UseDefaultFiles();
app.UseStaticFiles(new Microsoft.AspNetCore.StaticFiles.StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        if (ctx.File.Name.EndsWith(".js") || ctx.File.Name.EndsWith(".css"))
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000,immutable");
        }
        else if (ctx.File.Name == "index.html")
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "no-cache,no-store,must-revalidate");
        }
    }
});

app.UseIpRateLimiting();

app.UseResponseCaching();

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AuditLoggingMiddleware>();

app.MapControllers();

// SPA fallback
app.MapFallbackToFile("index.html");

app.Run();
