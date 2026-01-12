using AmazonBestSellers.Domain.Entities;
using AmazonBestSellers.Domain.Interfaces.Repositories;
using AmazonBestSellers.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AmazonBestSellers.Infrastructure.Repositories;

public class FavoriteProductRepository : GenericRepository<FavoriteProduct>, IFavoriteProductRepository
{
    public FavoriteProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<FavoriteProduct>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .AsNoTracking() // Read-only query for listing favorites - 30-40% performance improvement
            .Where(fp => fp.UserId == userId)
            .OrderByDescending(fp => fp.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> ExistsByUserAndAsinAsync(int userId, string asin)
    {
        return await _dbSet
            .AsNoTracking() // Read-only check for favorite existence
            .AnyAsync(fp => fp.UserId == userId && fp.ASIN == asin);
    }
}
