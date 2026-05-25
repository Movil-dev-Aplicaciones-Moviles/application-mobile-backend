using BackendAwSmartstay.API.IAM.Domain.Model.Aggregates;
using BackendAwSmartstay.API.IAM.Domain.Repositories;
using BackendAwSmartstay.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using BackendAwSmartstay.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BackendAwSmartstay.API.IAM.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     The user repository
/// </summary>
/// <remarks>
///     This repository is used to manage users
/// </remarks>
public class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
{
    /// <summary>
    ///     Find a user by username
    /// </summary>
    /// <param name="username">The username to search</param>
    /// <returns>The user</returns>
    public async Task<User?> FindByUsernameAsync(string username)
    {
        return await Context.Set<User>().FirstOrDefaultAsync(user => user.Username == username);
    }

    /// <summary>
    ///     Checks asynchronously whether a user exists by username.
    /// </summary>
    /// <param name="username">The username to search</param>
    /// <returns>True if the user exists, false otherwise</returns>
    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await Context.Set<User>().AnyAsync(user => user.Username == username);
    }
}