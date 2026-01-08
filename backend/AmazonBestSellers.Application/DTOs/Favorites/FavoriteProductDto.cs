namespace AmazonBestSellers.Application.DTOs.Favorites;

public record FavoriteProductDto
{
    public int Id { get; init; }
    public required string ASIN { get; init; }
    public required string Title { get; init; }
    public string? Price { get; init; }
    public string? ImageUrl { get; init; }
    public required string ProductUrl { get; init; }
    public decimal? Rating { get; init; }
    public DateTime CreatedAt { get; init; }
}
