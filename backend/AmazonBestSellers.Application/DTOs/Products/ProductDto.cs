namespace AmazonBestSellers.Application.DTOs.Products;

public record ProductDto
{
    public required string ASIN { get; init; }
    public required string Title { get; init; }
    public string? Price { get; init; }
    public string? ImageUrl { get; init; }
    public required string ProductUrl { get; init; }
    public decimal? Rating { get; init; }
}
