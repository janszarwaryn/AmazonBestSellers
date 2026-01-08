namespace AmazonBestSellers.Application.DTOs.Auth;

public record LoginRequestDto
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}
