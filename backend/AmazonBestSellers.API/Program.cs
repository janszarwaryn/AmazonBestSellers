using AmazonBestSellers.API.Extensions;
using AmazonBestSellers.API.Middleware;
using AmazonBestSellers.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using DotNetEnv;

var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Jwt:Secret"] = Environment.GetEnvironmentVariable("JWT_SECRET"),
    ["Jwt:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER"),
    ["Jwt:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
    ["Jwt:ExpireMinutes"] = Environment.GetEnvironmentVariable("JWT_EXPIRE_MINUTES"),
    ["RapidAPI:Key"] = Environment.GetEnvironmentVariable("RAPIDAPI_KEY"),
    ["RapidAPI:Host"] = Environment.GetEnvironmentVariable("RAPIDAPI_HOST"),
    ["RapidAPI:BaseUrl"] = Environment.GetEnvironmentVariable("RAPIDAPI_BASE_URL"),
    ["Cors:AllowedOrigins:0"] = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS")
});

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var connectionString = $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                       $"User={Environment.GetEnvironmentVariable("DB_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

// Response Caching for RapidAPI proxy (10 min TTL to reduce API calls)
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Response Caching middleware (must be before UseCors)
app.UseResponseCaching();

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AuditLoggingMiddleware>();

app.MapControllers();

app.Run();
