using AmazonBestSellers.API.Extensions;
using AmazonBestSellers.Application.DTOs.Favorites;
using AmazonBestSellers.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmazonBestSellers.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteProductService _favoriteProductService;

    public FavoritesController(IFavoriteProductService favoriteProductService)
    {
        _favoriteProductService = favoriteProductService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FavoriteProductDto>>> GetFavorites()
    {
        var userId = User.GetUserId();
        var favorites = await _favoriteProductService.GetUserFavoritesAsync(userId);
        return Ok(favorites);
    }

    [HttpPost]
    public async Task<ActionResult<FavoriteProductDto>> AddFavorite([FromBody] CreateFavoriteDto createDto)
    {
        var userId = User.GetUserId();
        var favorite = await _favoriteProductService.AddFavoriteAsync(userId, createDto);
        return CreatedAtAction(nameof(GetFavorites), new { id = favorite.Id }, favorite);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFavorite(int id)
    {
        var userId = User.GetUserId();
        await _favoriteProductService.RemoveFavoriteAsync(userId, id);
        return NoContent();
    }
}
