using AmazonBestSellers.Application.DTOs.Products;
using AmazonBestSellers.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AmazonBestSellers.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IAmazonApiService _amazonApiService;

    public ProductsController(IAmazonApiService amazonApiService)
    {
        _amazonApiService = amazonApiService;
    }

    /// <summary>
    /// Get best-selling software products from Amazon (cached for 10 minutes)
    /// This endpoint acts as a caching proxy to reduce RapidAPI calls
    /// </summary>
    [HttpGet("bestsellers")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByHeader = "Origin")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetBestSellers()
    {
        try
        {
            var products = await _amazonApiService.GetBestSellersAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to fetch bestsellers", error = ex.Message });
        }
    }
}
