using System.Security.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.Helpers;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<AuthService> _logger;

    // Constructor injects dependencies, including ILogger for logging purposes.
    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IPasswordHasher<User> passwordHasher, ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    // Registers a new user if the username or email is not already in use.
    public async Task<Guid> RegisterAsync(RegisterDto registerDto)
    {
        _logger.LogInformation("Starting registration for {Username}", registerDto.Username);

        if (await _unitOfWork.Users.AnyAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
        {
            _logger.LogWarning("Registration failed. User with username {Username} or email {Email} already exists", registerDto.Username, registerDto.Email);
            throw new AuthenticationException("User with this username or email already exists");
        }

        if (UserCredentialsValidator.IsSingleWordUsername(registerDto.Username) == false || UserCredentialsValidator.IsCredentialEmail(registerDto.Email) == false)
        {
            _logger.LogWarning("Registration failed for {Username} or {Email}: Invalid username or email", registerDto.Username, registerDto.Email);
            throw new AuthenticationException("Invalid username or email");
        }

        var newUser = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
        };

        if (UserCredentialsValidator.IsPasswordComplex(registerDto.Password) == false)
        {
            _logger.LogWarning("Registration failed for {Username}: Password not complex enough", registerDto.Username);
            throw new AuthenticationException("Password is not complex enough");
        }

        // Hash the user's password before saving.
        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, registerDto.Password);

        await _unitOfWork.Users.AddAsync(newUser);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User {Username} registered successfully with ID {UserId}", registerDto.Username, newUser.Id);

        return newUser.Id;
    }

    // Logs in the user by verifying the credentials and generating a JWT token.
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto login)
    {
        _logger.LogInformation("Login attempt for {UsernameOrEmail}", login.UsernameOrEmail);

        var user = await _unitOfWork.Users.GetUserByUsernameOrEmailAsync(login.UsernameOrEmail);

        if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, login.Password) == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Login failed for {UsernameOrEmail}: Invalid credentials", login.UsernameOrEmail);
            throw new AuthenticationException("Invalid username, email or password");
        }

        // Generate a JWT token for the authenticated user.
        var token = _tokenService.CreateToken(user);

        _logger.LogInformation("Login successful for {UsernameOrEmail}. Token generated", login.UsernameOrEmail);

        return new LoginResponseDto(user.Username, user.Email, token);
    }
}
