using System.Linq;
using Entities;
using RepositoryContracts;
using RepositoryContracts.ExceptionHandling; // NotFoundException, ValidationException

namespace FileRepositories;

public class UserFileRepository : FileRepository<User>, IUserRepository
{
    public UserFileRepository() : base(Path.Combine("Data", "users.json")) { }

    // ADD USER
    public async Task<User> AddAsync(User user)
    {
        var items = await LoadAsync();

        // validate required fields
        if (string.IsNullOrWhiteSpace(user.Username))
            throw new ValidationException("Username cannot be empty.");
        if (string.IsNullOrWhiteSpace(user.Password))
            throw new ValidationException("Password cannot be empty.");

        // duplicate username -> treat as validation error (400)
        if (items.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
            throw new ValidationException($"Username '{user.Username}' already exists.");

        // assign new ID
        user.Id = items.Count == 0 ? 1 : items.Max(u => u.Id) + 1;

        items.Add(user);
        await SaveAsync(items);
        return user;
    }

    // UPDATE USER
    public async Task UpdateAsync(User user)
    {
        var items = await LoadAsync();
        var idx = items.FindIndex(u => u.Id == user.Id);
        if (idx < 0)
            throw new NotFoundException($"User with ID {user.Id} not found.");

        // duplicate username on update -> validation (400)
        if (items.Any(u => u.Id != user.Id &&
                           u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
            throw new ValidationException($"Username '{user.Username}' already exists.");

        items[idx] = user;
        await SaveAsync(items);
    }

    // DELETE USER
    public async Task DeleteAsync(int id)
    {
        var items = await LoadAsync();
        var removed = items.RemoveAll(u => u.Id == id);
        if (removed == 0)
            throw new NotFoundException($"User with ID {id} not found.");
        await SaveAsync(items);
    }

    // GET SINGLE USER
    public async Task<User> GetSingleAsync(int id)
    {
        var items = await LoadAsync();
        var user = items.SingleOrDefault(u => u.Id == id);
        if (user is null)
            throw new NotFoundException($"User with ID {id} not found.");
        return user;
    }

    // GET MANY USERS
    public IQueryable<User> GetManyAsync()
        => LoadAsync().Result.AsQueryable();
}
