using AmazonBestSellers.Domain.Entities;

namespace AmazonBestSellers.Domain.Interfaces.Repositories;

public interface IFavoriteProductRepository : IGenericRepository<FavoriteProduct>
{
    Task<IEnumerable<FavoriteProduct>> GetByUserIdAsync(int userId);
    Task<bool> ExistsByUserAndAsinAsync(int userId, string asin);
}
