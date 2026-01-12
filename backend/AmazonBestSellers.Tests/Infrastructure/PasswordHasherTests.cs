using AmazonBestSellers.Application.Services.Implementations;
using Xunit;

namespace AmazonBestSellers.Tests.Infrastructure;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher;

    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Fact]
    public void HashPassword_ReturnsNonEmptyString()
    {
        var password = "TestPassword123!@#";

        var hash = _passwordHasher.HashPassword(password);

        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
        Assert.NotEqual(password, hash);
    }

    [Fact]
    public void HashPassword_DifferentPasswordsProduceDifferentHashes()
    {
        var password1 = "TestPassword1";
        var password2 = "TestPassword2";

        var hash1 = _passwordHasher.HashPassword(password1);
        var hash2 = _passwordHasher.HashPassword(password2);

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashPassword_SamePasswordProducesDifferentHashes()
    {
        var password = "TestPassword123!@#";

        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);

        // BCrypt includes a salt, so same password produces different hashes
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
    {
        var password = "TestPassword123!@#";
        var hash = _passwordHasher.HashPassword(password);

        var result = _passwordHasher.VerifyPassword(password, hash);

        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
    {
        var correctPassword = "TestPassword123!@#";
        var incorrectPassword = "WrongPassword";
        var hash = _passwordHasher.HashPassword(correctPassword);

        var result = _passwordHasher.VerifyPassword(incorrectPassword, hash);

        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_WithEmptyPassword_ReturnsFalse()
    {
        var password = "TestPassword123!@#";
        var hash = _passwordHasher.HashPassword(password);

        var result = _passwordHasher.VerifyPassword("", hash);

        Assert.False(result);
    }
}
