using System.Linq;
using Entities;
using RepositoryContracts;
using RepositoryContracts.ExceptionHandling; // for NotFoundException & ValidationException

namespace FileRepositories;

public class PostFileRepository : FileRepository<Post>, IPostRepository
{
    public PostFileRepository() : base(Path.Combine("Data", "posts.json")) { }

    // ADD POST
    public async Task<Post> AddAsync(Post post)
    {
        var items = await LoadAsync();

        // validate input
        if (string.IsNullOrWhiteSpace(post.Title))
            throw new ValidationException("Title cannot be empty.");
        if (string.IsNullOrWhiteSpace(post.Body))
            throw new ValidationException("Body cannot be empty.");
        if (post.UserId <= 0)
            throw new ValidationException("User ID must be greater than zero.");

        // assign new ID
        post.Id = items.Count == 0 ? 1 : items.Max(p => p.Id) + 1;

        items.Add(post);
        await SaveAsync(items);
        return post;
    }

    // UPDATE POST
    public async Task UpdateAsync(Post post)
    {
        var items = await LoadAsync();
        var idx = items.FindIndex(p => p.Id == post.Id);

        if (idx < 0)
            throw new NotFoundException($"Post with ID {post.Id} not found.");

        // validate input
        if (string.IsNullOrWhiteSpace(post.Title))
            throw new ValidationException("Title cannot be empty.");
        if (string.IsNullOrWhiteSpace(post.Body))
            throw new ValidationException("Body cannot be empty.");

        items[idx] = post;
        await SaveAsync(items);
    }

    // DELETE POST
    public async Task DeleteAsync(int id)
    {
        var items = await LoadAsync();
        var removed = items.RemoveAll(p => p.Id == id);

        if (removed == 0)
            throw new NotFoundException($"Post with ID {id} not found.");

        await SaveAsync(items);
    }

    // GET SINGLE POST
    public async Task<Post> GetSingleAsync(int id)
    {
        var items = await LoadAsync();
        var post = items.SingleOrDefault(p => p.Id == id);

        if (post is null)
            throw new NotFoundException($"Post with ID {id} not found.");

        return post;
    }

    // GET MANY POSTS
    public IQueryable<Post> GetManyAsync()
        => LoadAsync().Result.AsQueryable();
}
