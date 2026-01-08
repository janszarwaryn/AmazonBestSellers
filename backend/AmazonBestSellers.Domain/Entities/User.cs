using AmazonBestSellers.Domain.Common;

namespace AmazonBestSellers.Domain.Entities;

public class User : BaseEntity
{
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public ICollection<FavoriteProduct> FavoriteProducts { get; set; } = new List<FavoriteProduct>();
}
