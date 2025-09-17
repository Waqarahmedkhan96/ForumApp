using Entities;
using RepositoryContracts;
using System.Linq;

namespace InMemoryRepositories;

public class PostInMemoryRepository : IPostRepository
{
    private readonly List<Post> posts = new();

    public Task<Post> AddAsync(Post post)
    {
        post.Id = posts.Any() ? posts.Max(p => p.Id) + 1 : 1;
        posts.Add(post);
        return Task.FromResult(post);
    }

    public Task UpdateAsync(Post post)
    {
        var existing = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existing is null)
            throw new InvalidOperationException($"Post with ID '{post.Id}' not found");

        posts.Remove(existing);
        posts.Add(post);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var toRemove = posts.SingleOrDefault(p => p.Id == id);
        if (toRemove is null)
            throw new InvalidOperationException($"Post with ID '{id}' not found");

        posts.Remove(toRemove);
        return Task.CompletedTask;
    }

    public Task<Post> GetSingleAsync(int id)
    {
        var post = posts.SingleOrDefault(p => p.Id == id)
                   ?? throw new InvalidOperationException($"Post with ID '{id}' not found");
        return Task.FromResult(post);
    }

    public IQueryable<Post> GetManyAsync() => posts.AsQueryable();
}
