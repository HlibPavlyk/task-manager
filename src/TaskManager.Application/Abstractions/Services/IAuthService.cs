using TaskManager.Application.Dtos.Auth;

namespace TaskManager.Application.Abstractions.Services;

public interface IAuthService
{
    // Registers a new user and returns the user's unique ID (GUID).
    Task<Guid> RegisterAsync(RegisterDto registerDto);

    // Authenticates a user using their credentials and returns a JWT token if successful.
    Task<LoginResponseDto> LoginAsync(LoginRequestDto login);
}