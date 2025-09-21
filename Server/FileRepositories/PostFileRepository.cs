using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class PostFileRepository : FileRepository<Post>, IPostRepository
{
    public PostFileRepository() : base(Path.Combine("Data", "posts.json")) { }

    public async Task<Post> AddAsync(Post post)
    {
        var items = await LoadAsync();
        post.Id = items.Count == 0 ? 1 : items.Max(p => p.Id) + 1;
        items.Add(post);
        await SaveAsync(items);
        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        var items = await LoadAsync();
        var idx = items.FindIndex(p => p.Id == post.Id);
        if (idx < 0) throw new InvalidOperationException($"Post {post.Id} not found");
        items[idx] = post;
        await SaveAsync(items);
    }

    public async Task DeleteAsync(int id)
    {
        var items = await LoadAsync();
        items.RemoveAll(p => p.Id == id);
        await SaveAsync(items);
    }

    public async Task<Post> GetSingleAsync(int id)
    {
        var items = await LoadAsync();
        var p = items.SingleOrDefault(p => p.Id == id);
        return p ?? throw new InvalidOperationException($"Post {id} not found");
    }

    public IQueryable<Post> GetManyAsync()
        => LoadAsync().Result.AsQueryable();
}
