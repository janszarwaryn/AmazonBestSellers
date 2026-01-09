using AmazonBestSellers.Application.Common;
using AmazonBestSellers.Application.Services.Implementations;
using AmazonBestSellers.Domain.Entities;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace AmazonBestSellers.Tests.Services;

public class JwtTokenServiceTests
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public JwtTokenServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            Secret = "TestSecretKeyThatIsLongEnoughForHS256AlgorithmToWork123456",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpireMinutes = 60
        };

        var options = Options.Create(_jwtSettings);
        _jwtTokenService = new JwtTokenService(options);
    }

    [Fact]
    public void GenerateToken_ValidUser_ReturnsValidToken()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = "hashedpassword"
        };

        // Act
        var token = _jwtTokenService.GenerateToken(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        // Verify token structure
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal(_jwtSettings.Issuer, jwtToken.Issuer);
        Assert.Contains(jwtToken.Audiences, a => a == _jwtSettings.Audience);
        // JWT uses short claim type names in tokens
        Assert.Contains(jwtToken.Claims, c => c.Type == "nameid" && c.Value == "1");
        Assert.Contains(jwtToken.Claims, c => c.Type == "unique_name" && c.Value == "testuser");
    }

    [Fact]
    public void GenerateToken_MultipleUsers_GeneratesUniqueTokens()
    {
        // Arrange
        var user1 = new User { Id = 1, Username = "user1", PasswordHash = "hash1" };
        var user2 = new User { Id = 2, Username = "user2", PasswordHash = "hash2" };

        // Act
        var token1 = _jwtTokenService.GenerateToken(user1);
        var token2 = _jwtTokenService.GenerateToken(user2);

        // Assert
        Assert.NotEqual(token1, token2);
    }
}
