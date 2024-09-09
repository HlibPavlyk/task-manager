namespace TaskManager.Application.Dtos.Auth;

public record LoginRequestDto(string UsernameOrEmail, string Password);