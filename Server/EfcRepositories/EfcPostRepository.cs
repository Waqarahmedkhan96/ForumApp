using System.Linq;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using RepositoryContracts.ExceptionHandling;

namespace EfcRepositories;

public class EfcPostRepository : IPostRepository
{
    private readonly AppContext ctx;  // EF DbContext

    public EfcPostRepository(AppContext ctx)
    {
        this.ctx = ctx;   // injected context
    }

    // ADD POST (DB)
    public async Task<Post> AddAsync(Post post)
    {
        // validate input
        if (string.IsNullOrWhiteSpace(post.Title))
            throw new ValidationException("Title cannot be empty.");
        if (string.IsNullOrWhiteSpace(post.Body))
            throw new ValidationException("Body cannot be empty.");
        if (post.UserId <= 0)
            throw new ValidationException("User ID must be greater than zero.");

        await ctx.Posts.AddAsync(post);// track new post
        await ctx.SaveChangesAsync(); // write to SQLite
        return post; // Id now will set by DB
    }

    // UPDATE POST (DB)
    public async Task UpdateAsync(Post post)
    {
        var exists = await ctx.Posts
            .AnyAsync(p => p.Id == post.Id);          // PK lookup

        if (!exists)
            throw new NotFoundException($"Post with ID {post.Id} not found.");

        // validate again
        if (string.IsNullOrWhiteSpace(post.Title))
            throw new ValidationException("Title cannot be empty.");
        if (string.IsNullOrWhiteSpace(post.Body))
            throw new ValidationException("Body cannot be empty.");

        ctx.Posts.Update(post); // mark as modified
        await ctx.SaveChangesAsync(); // commit changes
    }

    // DELETE POST (DB)
    public async Task DeleteAsync(int id)
    {
        var existing = await ctx.Posts
            .SingleOrDefaultAsync(p => p.Id == id);   // single row query

        if (existing is null)
            throw new NotFoundException($"Post with ID {id} not found.");

        ctx.Posts.Remove(existing);  // mark for delete
        await ctx.SaveChangesAsync(); // commit delete
    }

    // GET SINGLE POST (DB)
    public async Task<Post> GetSingleAsync(int id)
    {
        var post = await ctx.Posts
            .SingleOrDefaultAsync(p => p.Id == id);   // PK lookup

        if (post is null)
            throw new NotFoundException($"Post with ID {id} not found.");

        return post;     
    }

    // GET MANY POSTS (DB)
    public IQueryable<Post> GetManyAsync()
        => ctx.Posts.AsQueryable();
}
