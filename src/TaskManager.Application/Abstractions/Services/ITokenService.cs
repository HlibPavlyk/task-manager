using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions.Services;

public interface ITokenService
{
    // Creates a JWT token based on the provided user information.
    string CreateToken(User user);
}
