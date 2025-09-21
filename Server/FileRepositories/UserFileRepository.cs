using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class UserFileRepository : FileRepository<User>, IUserRepository
{
    public UserFileRepository() : base(Path.Combine("Data", "users.json")) { }

    public async Task<User> AddAsync(User user)
    {
        var items = await LoadAsync();
        user.Id = items.Count == 0 ? 1 : items.Max(u => u.Id) + 1;
        items.Add(user);
        await SaveAsync(items);
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        var items = await LoadAsync();
        var idx = items.FindIndex(u => u.Id == user.Id);
        if (idx < 0) throw new InvalidOperationException($"User {user.Id} not found");
        items[idx] = user;
        await SaveAsync(items);
    }

    public async Task DeleteAsync(int id)
    {
        var items = await LoadAsync();
        items.RemoveAll(u => u.Id == id);
        await SaveAsync(items);
    }

    public async Task<User> GetSingleAsync(int id)
    {
        var items = await LoadAsync();
        var u = items.SingleOrDefault(u => u.Id == id);
        return u ?? throw new InvalidOperationException($"User {id} not found");
    }

    public IQueryable<User> GetManyAsync()
        => LoadAsync().Result.AsQueryable();
}
