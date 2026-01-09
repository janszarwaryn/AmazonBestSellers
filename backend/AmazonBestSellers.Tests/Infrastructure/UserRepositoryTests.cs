using AmazonBestSellers.Domain.Entities;
using AmazonBestSellers.Infrastructure.Data;
using AmazonBestSellers.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AmazonBestSellers.Tests.Infrastructure;

public class UserRepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task GetByUsernameAsync_ExistingUser_ReturnsUser()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            PasswordHash = "hashedpassword"
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUsernameAsync("testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
        Assert.Equal("hashedpassword", result.PasswordHash);
    }

    [Fact]
    public async Task GetByUsernameAsync_NonExistingUser_ReturnsNull()
    {
        // Arrange - Empty database

        // Act
        var result = await _repository.GetByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }
}
