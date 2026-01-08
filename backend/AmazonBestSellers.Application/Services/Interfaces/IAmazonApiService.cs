using AmazonBestSellers.Application.DTOs.Products;

namespace AmazonBestSellers.Application.Services.Interfaces;

public interface IAmazonApiService
{
    Task<IEnumerable<ProductDto>> GetBestSellersAsync();
}
