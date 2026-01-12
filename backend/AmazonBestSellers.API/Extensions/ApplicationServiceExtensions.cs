using AmazonBestSellers.Application.Common;
using AmazonBestSellers.Application.Services.Implementations;
using AmazonBestSellers.Application.Services.Interfaces;
using AmazonBestSellers.Application.Validators;
using AmazonBestSellers.Domain.Interfaces.Repositories;
using AmazonBestSellers.Infrastructure.ExternalServices;
using AmazonBestSellers.Infrastructure.Repositories;
using FluentValidation;

namespace AmazonBestSellers.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFavoriteProductRepository, FavoriteProductRepository>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFavoriteProductService, FavoriteProductService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddHttpClient<IAmazonApiService, AmazonApiService>((serviceProvider, client) =>
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var baseUrl = config["RapidAPI:BaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                client.BaseAddress = new Uri(baseUrl);
            }
        });

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularApp", policy =>
            {
                var allowedOriginsConfig = configuration["Cors:AllowedOrigins:0"] ?? "http://localhost:4200";
                var allowedOrigins = allowedOriginsConfig.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                policy.WithOrigins(allowedOrigins)
                      .WithMethods("GET", "POST", "DELETE")
                      .WithHeaders("Content-Type", "Authorization")
                      .AllowCredentials();
            });
        });

        return services;
    }
}
