using AmazonBestSellers.Application.DTOs.Favorites;
using AmazonBestSellers.Application.Services.Interfaces;
using AmazonBestSellers.Domain.Entities;
using AmazonBestSellers.Domain.Interfaces.Repositories;

namespace AmazonBestSellers.Application.Services.Implementations;

public class FavoriteProductService : IFavoriteProductService
{
    private readonly IFavoriteProductRepository _favoriteProductRepository;

    public FavoriteProductService(IFavoriteProductRepository favoriteProductRepository)
    {
        _favoriteProductRepository = favoriteProductRepository;
    }

    public async Task<IEnumerable<FavoriteProductDto>> GetUserFavoritesAsync(int userId)
    {
        var favorites = await _favoriteProductRepository.GetByUserIdAsync(userId);

        return favorites.Select(f => new FavoriteProductDto
        {
            Id = f.Id,
            ASIN = f.ASIN,
            Title = f.Title,
            Price = f.Price,
            ImageUrl = f.ImageUrl,
            ProductUrl = f.ProductUrl,
            Rating = f.Rating,
            CreatedAt = f.CreatedAt
        });
    }

    public async Task<FavoriteProductDto> AddFavoriteAsync(int userId, CreateFavoriteDto createDto)
    {
        if (await _favoriteProductRepository.ExistsByUserAndAsinAsync(userId, createDto.ASIN))
            throw new InvalidOperationException("Product already in favorites");

        var favorite = new FavoriteProduct
        {
            UserId = userId,
            ASIN = createDto.ASIN,
            Title = createDto.Title,
            Price = createDto.Price,
            ImageUrl = createDto.ImageUrl,
            ProductUrl = createDto.ProductUrl,
            Rating = createDto.Rating
        };

        await _favoriteProductRepository.CreateAsync(favorite);

        return new FavoriteProductDto
        {
            Id = favorite.Id,
            ASIN = favorite.ASIN,
            Title = favorite.Title,
            Price = favorite.Price,
            ImageUrl = favorite.ImageUrl,
            ProductUrl = favorite.ProductUrl,
            Rating = favorite.Rating,
            CreatedAt = favorite.CreatedAt
        };
    }

    public async Task RemoveFavoriteAsync(int userId, int favoriteId)
    {
        var favorite = await _favoriteProductRepository.GetByIdAsync(favoriteId);

        if (favorite == null)
            throw new KeyNotFoundException("Favorite not found");

        if (favorite.UserId != userId)
            throw new UnauthorizedAccessException("Not authorized to remove this favorite");

        await _favoriteProductRepository.DeleteAsync(favoriteId);
    }
}
