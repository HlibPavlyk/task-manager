using TaskManager.Application.Dtos.Auth;

namespace TaskManager.Application.Abstractions.Services;

public interface IAuthService
{
    Task<Guid> RegisterAsync(RegisterDto registerDto);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto login);
}