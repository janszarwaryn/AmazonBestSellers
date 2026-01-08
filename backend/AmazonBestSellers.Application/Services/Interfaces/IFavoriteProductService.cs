using AmazonBestSellers.Application.DTOs.Favorites;

namespace AmazonBestSellers.Application.Services.Interfaces;

public interface IFavoriteProductService
{
    Task<IEnumerable<FavoriteProductDto>> GetUserFavoritesAsync(int userId);
    Task<FavoriteProductDto> AddFavoriteAsync(int userId, CreateFavoriteDto createDto);
    Task RemoveFavoriteAsync(int userId, int favoriteId);
}
