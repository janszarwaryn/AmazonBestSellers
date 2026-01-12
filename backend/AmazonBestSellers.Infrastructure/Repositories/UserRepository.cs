using AmazonBestSellers.Domain.Entities;
using AmazonBestSellers.Domain.Interfaces.Repositories;
using AmazonBestSellers.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AmazonBestSellers.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .AsNoTracking() // Read-only query for authentication - 30-40% performance improvement
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> ExistsAsync(string username)
    {
        return await _dbSet
            .AsNoTracking() // Read-only check for username existence
            .AnyAsync(u => u.Username == username);
    }
}
