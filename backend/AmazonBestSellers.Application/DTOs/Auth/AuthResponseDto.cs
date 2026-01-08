namespace AmazonBestSellers.Application.DTOs.Auth;

public record AuthResponseDto
{
    public required string Token { get; init; }
    public required string Username { get; init; }
    public required int UserId { get; init; }
}
