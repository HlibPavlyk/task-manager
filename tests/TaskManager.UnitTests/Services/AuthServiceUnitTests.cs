using System.Linq.Expressions;
using System.Security.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tokenServiceMock = new Mock<ITokenService>();
        _passwordHasherMock = new Mock<IPasswordHasher<User>>();
        Mock<ILogger<AuthService>> loggerMock = new();
        _authService = new AuthService(_unitOfWorkMock.Object, _tokenServiceMock.Object, _passwordHasherMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldRegisterUser_WhenDataIsValid()
    {
        // Arrange
        var registerDto = new RegisterDto ("testuser", "testuser@example.com", "P@ssword123" );

        _unitOfWorkMock.Setup(x => x.Users.AnyAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(false);
        _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<User>(), registerDto.Password)).Returns("hashedPassword");
        _unitOfWorkMock.Setup(x => x.Users.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _unitOfWorkMock.Verify(x => x.Users.AddAsync(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task RegisterAsync_ShouldThrowAuthenticationException_WhenUserExists()
    {
        // Arrange
        var registerDto = new RegisterDto ("testuser", "testuser@example.com", "P@ssword123" );

        _unitOfWorkMock.Setup(x => x.Users.AnyAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(() => _authService.RegisterAsync(registerDto));
    }
    
    [Fact]
    public async Task RegisterAsync_ShouldThrowAuthenticationException_WhenUsernameIsInvalid()
    {
        // Arrange
        var registerDto = new RegisterDto ("testuser", "testuser@example.com", "P@ssword123" );

        _unitOfWorkMock.Setup(x => x.Users.AnyAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(() => _authService.RegisterAsync(registerDto));
    }
    
    [Fact]
    public async Task RegisterAsync_ShouldThrowAuthenticationException_WhenPasswordIsInvalid()
    {
        // Arrange
        var registerDto = new RegisterDto ("testuser", "testuser@example.com", "weakpassword" );

        _unitOfWorkMock.Setup(x => x.Users.AnyAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(() => _authService.RegisterAsync(registerDto));
    }

    [Fact]
    public async Task LoginAsync_ShouldLoginUser_WhenCredentialsAreValid()
    {
        // Arrange
        var loginRequestDto = new LoginRequestDto ("testuser", "P@ssword123" );
        var user = new User { Username = "testuser", Email = "testuser@example.com", PasswordHash = "hashedPassword" };
        var token = "jwt_token";

        _unitOfWorkMock.Setup(x => x.Users.GetUserByUsernameOrEmailAsync(loginRequestDto.UsernameOrEmail)).ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(user, user.PasswordHash, loginRequestDto.Password))
            .Returns(PasswordVerificationResult.Success);
        _tokenServiceMock.Setup(x => x.CreateToken(user)).Returns(token);

        // Act
        var result = await _authService.LoginAsync(loginRequestDto);

        // Assert
        Assert.Equal(token, result.Token);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.Email, result.Email);
    }
    
    [Fact]
    public async Task LoginAsync_ShouldThrowAuthenticationException_WhenCredentialsAreInvalid()
    {
        // Arrange
        var loginRequestDto = new LoginRequestDto ("testuser", "wrongpassword");
        var user = new User { Username = "testuser", PasswordHash = "hashedPassword" };

        _unitOfWorkMock.Setup(x => x.Users.GetUserByUsernameOrEmailAsync(loginRequestDto.UsernameOrEmail)).ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(user, user.PasswordHash, loginRequestDto.Password))
            .Returns(PasswordVerificationResult.Failed);

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(() => _authService.LoginAsync(loginRequestDto));
    }
    
    [Fact]
    public async Task LoginAsync_ShouldThrowAuthenticationException_WhenUserDoesNotExist()
    {
        // Arrange
        var loginRequestDto = new LoginRequestDto ("testuser", "P@ssword123" );

        _unitOfWorkMock.Setup(x => x.Users.GetUserByUsernameOrEmailAsync(loginRequestDto.UsernameOrEmail)).ReturnsAsync((User)null!);

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(() => _authService.LoginAsync(loginRequestDto));
    }






}