using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class CommentFileRepository : FileRepository<Comment>, ICommentRepository
{
    public CommentFileRepository() : base(Path.Combine("Data", "comments.json")) { }

    public async Task<Comment> AddAsync(Comment c)
    {
        var items = await LoadAsync();
        c.Id = items.Count == 0 ? 1 : items.Max(x => x.Id) + 1;
        items.Add(c);
        await SaveAsync(items);
        return c;
    }

    public async Task UpdateAsync(Comment c)
    {
        var items = await LoadAsync();
        var idx = items.FindIndex(x => x.Id == c.Id);
        if (idx < 0) throw new InvalidOperationException($"Comment {c.Id} not found");
        items[idx] = c;
        await SaveAsync(items);
    }

    public async Task DeleteAsync(int id)
    {
        var items = await LoadAsync();
        items.RemoveAll(x => x.Id == id);
        await SaveAsync(items);
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        var items = await LoadAsync();
        var c = items.SingleOrDefault(x => x.Id == id);
        return c ?? throw new InvalidOperationException($"Comment {id} not found");
    }

    public IQueryable<Comment> GetManyAsync()
        => LoadAsync().Result.AsQueryable();
}
