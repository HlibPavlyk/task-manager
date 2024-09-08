using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
}