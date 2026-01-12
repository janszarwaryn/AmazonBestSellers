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

    [HttpGet("bestsellers")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByHeader = "Origin")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetBestSellers()
    {
        var products = await _amazonApiService.GetBestSellersAsync();
        return Ok(products);
    }
}
