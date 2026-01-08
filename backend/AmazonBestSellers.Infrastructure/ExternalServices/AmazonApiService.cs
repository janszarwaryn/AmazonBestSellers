using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AmazonBestSellers.Application.DTOs.Products;
using AmazonBestSellers.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AmazonBestSellers.Infrastructure.ExternalServices;

public class AmazonApiService : IAmazonApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _apiHost;

    public AmazonApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["RapidAPI:Key"] ?? throw new InvalidOperationException("RapidAPI Key not configured");
        _apiHost = configuration["RapidAPI:Host"] ?? throw new InvalidOperationException("RapidAPI Host not configured");
    }

    public async Task<IEnumerable<ProductDto>> GetBestSellersAsync()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"https://{_apiHost}/best-sellers?category=software&country=PL&type=BEST_SELLERS"),
            Headers =
            {
                { "X-RapidAPI-Key", _apiKey },
                { "X-RapidAPI-Host", _apiHost }
            }
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var apiResponse = await response.Content.ReadFromJsonAsync<AmazonApiResponse>();

        if (apiResponse?.Data?.BestSellers == null)
            return Enumerable.Empty<ProductDto>();

        return apiResponse.Data.BestSellers.Select(p => new ProductDto
        {
            ASIN = p.Asin ?? string.Empty,
            Title = p.Title ?? string.Empty,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            ProductUrl = p.ProductUrl ?? string.Empty,
            Rating = ParseRating(p.Rating)
        });
    }

    private static decimal? ParseRating(string? rating)
    {
        if (string.IsNullOrEmpty(rating))
            return null;

        if (decimal.TryParse(rating, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var result))
            return result;

        return null;
    }

    private record AmazonApiResponse
    {
        [JsonPropertyName("data")]
        public ResponseData? Data { get; init; }
    }

    private record ResponseData
    {
        [JsonPropertyName("best_sellers")]
        public List<ApiProduct>? BestSellers { get; init; }
    }

    private record ApiProduct
    {
        [JsonPropertyName("asin")]
        public string? Asin { get; init; }

        [JsonPropertyName("product_title")]
        public string? Title { get; init; }

        [JsonPropertyName("product_price")]
        public string? Price { get; init; }

        [JsonPropertyName("product_photo")]
        public string? ImageUrl { get; init; }

        [JsonPropertyName("product_url")]
        public string? ProductUrl { get; init; }

        [JsonPropertyName("product_star_rating")]
        public string? Rating { get; init; }

        [JsonPropertyName("product_num_ratings")]
        public int? NumRatings { get; init; }

        [JsonPropertyName("rank")]
        public int? Rank { get; init; }
    }
}
