using AmazonBestSellers.Application.DTOs.Favorites;
using AmazonBestSellers.Application.Services.Implementations;
using AmazonBestSellers.Domain.Entities;
using AmazonBestSellers.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace AmazonBestSellers.Tests.Services;

public class FavoriteProductServiceTests
{
    private readonly Mock<IFavoriteProductRepository> _favoriteRepositoryMock;
    private readonly FavoriteProductService _favoriteService;

    public FavoriteProductServiceTests()
    {
        _favoriteRepositoryMock = new Mock<IFavoriteProductRepository>();
        _favoriteService = new FavoriteProductService(_favoriteRepositoryMock.Object);
    }

    [Fact]
    public async Task AddFavoriteAsync_WithNewProduct_ReturnsDto()
    {
        // Arrange
        var userId = 1;
        var createDto = new CreateFavoriteDto
        {
            ASIN = "B001TEST",
            Title = "Test Product",
            Price = "$29.99",
            ImageUrl = "http://example.com/image.jpg",
            ProductUrl = "http://example.com/product",
            Rating = 4.5m
        };

        _favoriteRepositoryMock.Setup(r => r.ExistsByUserAndAsinAsync(userId, createDto.ASIN))
            .ReturnsAsync(false);
        _favoriteRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<FavoriteProduct>()))
            .ReturnsAsync((FavoriteProduct fp) => fp);

        // Act
        var result = await _favoriteService.AddFavoriteAsync(userId, createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createDto.ASIN, result.ASIN);
        Assert.Equal(createDto.Title, result.Title);
        _favoriteRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<FavoriteProduct>()), Times.Once);
    }

    [Fact]
    public async Task AddFavoriteAsync_WithExistingProduct_ThrowsException()
    {
        // Arrange
        var userId = 1;
        var createDto = new CreateFavoriteDto
        {
            ASIN = "B001TEST",
            Title = "Test Product",
            Price = "$29.99",
            ProductUrl = "http://example.com/product"
        };

        _favoriteRepositoryMock.Setup(r => r.ExistsByUserAndAsinAsync(userId, createDto.ASIN))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _favoriteService.AddFavoriteAsync(userId, createDto)
        );
    }

    [Fact]
    public async Task RemoveFavoriteAsync_WithExistingProduct_Succeeds()
    {
        // Arrange
        var userId = 1;
        var favoriteId = 1;

        var favoriteProduct = new FavoriteProduct
        {
            Id = favoriteId,
            UserId = userId,
            ASIN = "B001TEST",
            Title = "Test Product",
            Price = "$29.99",
            ProductUrl = "http://example.com/product"
        };

        _favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteId))
            .ReturnsAsync(favoriteProduct);
        _favoriteRepositoryMock.Setup(r => r.DeleteAsync(favoriteId))
            .Returns(Task.CompletedTask);

        // Act
        await _favoriteService.RemoveFavoriteAsync(userId, favoriteId);

        // Assert
        _favoriteRepositoryMock.Verify(r => r.DeleteAsync(favoriteId), Times.Once);
    }

    [Fact]
    public async Task RemoveFavoriteAsync_WithNonExistentProduct_ThrowsException()
    {
        // Arrange
        var userId = 1;
        var favoriteId = 999;

        _favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteId))
            .ReturnsAsync((FavoriteProduct?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _favoriteService.RemoveFavoriteAsync(userId, favoriteId)
        );
    }

    [Fact]
    public async Task RemoveFavoriteAsync_WithDifferentUser_ThrowsException()
    {
        // Arrange
        var userId = 1;
        var favoriteId = 1;

        var favoriteProduct = new FavoriteProduct
        {
            Id = favoriteId,
            UserId = 2, // Different user
            ASIN = "B001TEST",
            Title = "Test Product",
            Price = "$29.99",
            ProductUrl = "http://example.com/product"
        };

        _favoriteRepositoryMock.Setup(r => r.GetByIdAsync(favoriteId))
            .ReturnsAsync(favoriteProduct);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _favoriteService.RemoveFavoriteAsync(userId, favoriteId)
        );
    }

    [Fact]
    public async Task GetUserFavoritesAsync_ReturnsAllFavorites()
    {
        // Arrange
        var userId = 1;
        var favorites = new List<FavoriteProduct>
        {
            new FavoriteProduct
            {
                Id = 1,
                UserId = userId,
                ASIN = "B001TEST1",
                Title = "Test Product 1",
                Price = "$29.99",
                ImageUrl = "http://example.com/image1.jpg",
                ProductUrl = "http://example.com/product1",
                Rating = 4.5m,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new FavoriteProduct
            {
                Id = 2,
                UserId = userId,
                ASIN = "B001TEST2",
                Title = "Test Product 2",
                Price = "$39.99",
                ImageUrl = "http://example.com/image2.jpg",
                ProductUrl = "http://example.com/product2",
                Rating = 4.8m,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        _favoriteRepositoryMock.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(favorites);

        // Act
        var result = await _favoriteService.GetUserFavoritesAsync(userId);

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Contains(resultList, p => p.ASIN == "B001TEST1");
        Assert.Contains(resultList, p => p.ASIN == "B001TEST2");
    }

    [Fact]
    public async Task GetUserFavoritesAsync_WithNoFavorites_ReturnsEmptyList()
    {
        // Arrange
        var userId = 1;
        _favoriteRepositoryMock.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<FavoriteProduct>());

        // Act
        var result = await _favoriteService.GetUserFavoritesAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
