using System.Linq;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using RepositoryContracts.ExceptionHandling;

namespace EfcRepositories;

public class EfcUserRepository : IUserRepository
{
    private readonly AppContext ctx;  // EF DbContext

    public EfcUserRepository(AppContext ctx)
    {
        this.ctx = ctx;      // injected context
    }

    // ADD USER (DB)
    public async Task<User> AddAsync(User user)
    {
        // validate fields
        if (string.IsNullOrWhiteSpace(user.Username))
            throw new ValidationException("Username cannot be empty.");
        if (string.IsNullOrWhiteSpace(user.Password))
            throw new ValidationException("Password cannot be empty.");

        // check duplicate username
        bool exists = await ctx.Users
            .AnyAsync(u => u.Username.ToLower() == user.Username.ToLower()); // DB check

        if (exists)
            throw new ValidationException($"Username '{user.Username}' already exists.");

        await ctx.Users.AddAsync(user); // track new entity
        await ctx.SaveChangesAsync();  // write to SQLite
        return user;                  // Id now set by DB
    }

    // UPDATE USER (DB)
    public async Task UpdateAsync(User user)
    {
        // user exists?
        bool exists = await ctx.Users
            .AnyAsync(u => u.Id == user.Id);      // PK lookup

        if (!exists)
            throw new NotFoundException($"User with ID {user.Id} not found.");

        // duplicate username on update
        bool duplicate = await ctx.Users
            .AnyAsync(u => u.Id != user.Id &&
                           u.Username.ToLower() == user.Username.ToLower()); // DB uniqueness

        if (duplicate)
            throw new ValidationException($"Username '{user.Username}' already exists.");

        ctx.Users.Update(user);         // mark as modified
        await ctx.SaveChangesAsync();   // commit changes
    }

    // DELETE USER (DB)
    public async Task DeleteAsync(int id)
    {
        var existing = await ctx.Users
            .SingleOrDefaultAsync(u => u.Id == id);   // single row query

        if (existing is null)
            throw new NotFoundException($"User with ID {id} not found.");

        ctx.Users.Remove(existing);    // mark for delete
        await ctx.SaveChangesAsync(); // commit delete
    }

    // GET SINGLE USER (DB)
    public async Task<User> GetSingleAsync(int id)
    {
        var user = await ctx.Users
            .SingleOrDefaultAsync(u => u.Id == id);   // PK lookup

        if (user is null)
            throw new NotFoundException($"User with ID {id} not found.");

        return user;     // tracked entity
    }

    // GET MANY USERS (DB)
    public IQueryable<User> GetManyAsync()
        => ctx.Users.AsQueryable();  
}
