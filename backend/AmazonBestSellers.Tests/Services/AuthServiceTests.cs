using AmazonBestSellers.Application.DTOs.Auth;
using AmazonBestSellers.Application.Services.Implementations;
using AmazonBestSellers.Application.Services.Interfaces;
using AmazonBestSellers.Domain.Entities;
using AmazonBestSellers.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace AmazonBestSellers.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _authService = new AuthService(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenServiceMock.Object
        );
    }

    [Fact]
    public async Task RegisterAsync_WithNewUser_ReturnsAuthResponse()
    {
        var registerDto = new RegisterRequestDto
        {
            Username = "newuser",
            Password = "Password123!@#"
        };

        _userRepositoryMock.Setup(r => r.ExistsAsync(registerDto.Username))
            .ReturnsAsync(false);
        _passwordHasherMock.Setup(h => h.HashPassword(registerDto.Password))
            .Returns("hashedPassword");
        _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);
        _jwtTokenServiceMock.Setup(j => j.GenerateToken(It.IsAny<User>()))
            .Returns("jwt.token.here");

        var result = await _authService.RegisterAsync(registerDto);

        Assert.NotNull(result);
        Assert.Equal("jwt.token.here", result.Token);
        Assert.Equal(registerDto.Username, result.Username);
        _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUsername_ThrowsException()
    {
        var registerDto = new RegisterRequestDto
        {
            Username = "existinguser",
            Password = "Password123!@#"
        };

        _userRepositoryMock.Setup(r => r.ExistsAsync(registerDto.Username))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _authService.RegisterAsync(registerDto)
        );
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
    {
        var loginDto = new LoginRequestDto
        {
            Username = "testuser",
            Password = "Password123!@#"
        };

        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = "hashedPassword"
        };

        _userRepositoryMock.Setup(r => r.GetByUsernameAsync(loginDto.Username))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.VerifyPassword(loginDto.Password, user.PasswordHash))
            .Returns(true);
        _jwtTokenServiceMock.Setup(j => j.GenerateToken(user))
            .Returns("jwt.token.here");

        var result = await _authService.LoginAsync(loginDto);

        Assert.NotNull(result);
        Assert.Equal("jwt.token.here", result.Token);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.Id, result.UserId);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidUsername_ThrowsException()
    {
        var loginDto = new LoginRequestDto
        {
            Username = "nonexistent",
            Password = "Password123!@#"
        };

        _userRepositoryMock.Setup(r => r.GetByUsernameAsync(loginDto.Username))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(loginDto)
        );
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsException()
    {
        var loginDto = new LoginRequestDto
        {
            Username = "testuser",
            Password = "WrongPassword"
        };

        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = "hashedPassword"
        };

        _userRepositoryMock.Setup(r => r.GetByUsernameAsync(loginDto.Username))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.VerifyPassword(loginDto.Password, user.PasswordHash))
            .Returns(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(loginDto)
        );
    }
}
