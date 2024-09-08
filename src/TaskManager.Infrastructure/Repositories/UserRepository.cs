using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
    {
        return await Context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Username == usernameOrEmail || x.Email == usernameOrEmail);
    }
}