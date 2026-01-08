using System.Security.Claims;
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

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User not authenticated"));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FavoriteProductDto>>> GetFavorites()
    {
        try
        {
            var userId = GetUserId();
            var favorites = await _favoriteProductService.GetUserFavoritesAsync(userId);
            return Ok(favorites);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<FavoriteProductDto>> AddFavorite([FromBody] CreateFavoriteDto createDto)
    {
        try
        {
            var userId = GetUserId();
            var favorite = await _favoriteProductService.AddFavoriteAsync(userId, createDto);
            return CreatedAtAction(nameof(GetFavorites), new { id = favorite.Id }, favorite);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFavorite(int id)
    {
        try
        {
            var userId = GetUserId();
            await _favoriteProductService.RemoveFavoriteAsync(userId, id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
