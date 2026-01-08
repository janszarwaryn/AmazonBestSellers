using AmazonBestSellers.Domain.Entities;

namespace AmazonBestSellers.Domain.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> ExistsAsync(string username);
}
