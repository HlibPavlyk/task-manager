using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    // Retrieves a user by their username or email, or returns null if not found.
    Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
}
