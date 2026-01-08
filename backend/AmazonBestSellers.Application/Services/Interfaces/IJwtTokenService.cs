using AmazonBestSellers.Domain.Entities;

namespace AmazonBestSellers.Application.Services.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    int? ValidateToken(string token);
}
