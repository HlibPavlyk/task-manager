using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    // Constructor that passes the context to the base GenericRepository.
    public UserRepository(ApplicationDbContext context) : base(context) { }

    // Retrieves a user by their username or email, returns null if no match is found.
    public async Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
    {
        return await Context.Users
            .AsNoTracking() // Ensures the query does not track the entity, improving performance when tracking isn't needed.
            .SingleOrDefaultAsync(x => x.Username == usernameOrEmail || x.Email == usernameOrEmail);
    }
}
