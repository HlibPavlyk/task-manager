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

    // Constructor to initialize dependencies for token service, password hashing, and logging.
    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IPasswordHasher<User> passwordHasher, ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    // Registers a new user after validating the username, email, and password.
    public async Task<Guid> RegisterAsync(RegisterDto registerDto)
    {
        _logger.LogInformation("Starting registration for {Username}", registerDto.Username);

        // Check if a user with the same username or email already exists.
        if (await _unitOfWork.Users.AnyAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
            throw new AuthenticationException($"Registration failed: User with username '{registerDto.Username}' or email '{registerDto.Email}' already exists");

        // Validate the username and email.
        if (!UserCredentialsValidator.IsSingleWordUsername(registerDto.Username))
            throw new AuthenticationException($"Registration failed: Username '{registerDto.Username}' is not valid. It must be a single word.");

        if (!UserCredentialsValidator.IsCredentialEmail(registerDto.Email))
            throw new AuthenticationException($"Registration failed: Email '{registerDto.Email}' is not valid.");

        // Create a new user and validate password complexity.
        var newUser = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
        };

        if (!UserCredentialsValidator.IsPasswordComplex(registerDto.Password))
            throw new AuthenticationException("Registration failed: Password does not meet complexity requirements (minimum 8 characters, at least one uppercase letter, one number, and one special character).");

        // Hash the password before saving it.
        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, registerDto.Password);

        // Save the new user to the database.
        await _unitOfWork.Users.AddAsync(newUser);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User {Username} registered successfully with ID {UserId}", registerDto.Username, newUser.Id);

        return newUser.Id;
    }

    // Authenticates the user using username/email and password, and generates a JWT token.
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto login)
    {
        _logger.LogInformation("Login attempt for {UsernameOrEmail}", login.UsernameOrEmail);

        // Retrieve the user by username or email.
        var user = await _unitOfWork.Users.GetUserByUsernameOrEmailAsync(login.UsernameOrEmail);

        // Validate the credentials by checking the password hash.
        if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, login.Password) == PasswordVerificationResult.Failed)
            throw new AuthenticationException($"Login failed: Invalid credentials for '{login.UsernameOrEmail}'");

        // Generate the JWT token for the authenticated user.
        var token = _tokenService.CreateToken(user);

        _logger.LogInformation("Login successful for {UsernameOrEmail}. Token generated", login.UsernameOrEmail);

        return new LoginResponseDto(user.Username, user.Email, token);
    }
}