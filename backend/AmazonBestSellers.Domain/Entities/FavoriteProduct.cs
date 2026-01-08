using AmazonBestSellers.Domain.Common;

namespace AmazonBestSellers.Domain.Entities;

public class FavoriteProduct : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public required string ASIN { get; set; }
    public required string Title { get; set; }
    public string? Price { get; set; }
    public string? ImageUrl { get; set; }
    public required string ProductUrl { get; set; }
    public decimal? Rating { get; set; }
}
