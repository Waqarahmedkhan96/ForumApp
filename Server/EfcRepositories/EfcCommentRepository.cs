using System.Linq;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using RepositoryContracts.ExceptionHandling;

namespace EfcRepositories;

public class EfcCommentRepository : ICommentRepository
{
    private readonly AppContext ctx;// EF DbContext

    public EfcCommentRepository(AppContext ctx)
    {
        this.ctx = ctx;  // injected context
    }

    // ADD COMMENT (DB)
    public async Task<Comment> AddAsync(Comment comment)
    {
        // validate input
        if (string.IsNullOrWhiteSpace(comment.Body))
            throw new ValidationException("Comment body cannot be empty.");
        if (comment.PostId <= 0)
            throw new ValidationException("Post ID must be greater than zero.");
        if (comment.UserId <= 0)
            throw new ValidationException("User ID must be greater than zero.");

        await ctx.Comments.AddAsync(comment); // track new comment
        await ctx.SaveChangesAsync();  // write to SQLite
        return comment;  // Id now set by DB
    }

    // UPDATE COMMENT (DB)
    public async Task UpdateAsync(Comment comment)
    {
        var exists = await ctx.Comments
            .AnyAsync(c => c.Id == comment.Id);  // PK lookup

        if (!exists)
            throw new NotFoundException($"Comment with ID {comment.Id} not found.");

        // validate body
        if (string.IsNullOrWhiteSpace(comment.Body))
            throw new ValidationException("Comment body cannot be empty.");

        ctx.Comments.Update(comment); // mark as modified
        await ctx.SaveChangesAsync(); // commit changes
    }

    // DELETE COMMENT (DB)
    public async Task DeleteAsync(int id)
    {
        var existing = await ctx.Comments
            .SingleOrDefaultAsync(c => c.Id == id);   // single row query

        if (existing is null)
            throw new NotFoundException($"Comment with ID {id} not found.");

        ctx.Comments.Remove(existing); // mark for delete
        await ctx.SaveChangesAsync();  // commit delete
    }

    // GET SINGLE COMMENT (DB)
    public async Task<Comment> GetSingleAsync(int id)
    {
        var comment = await ctx.Comments
            .SingleOrDefaultAsync(c => c.Id == id);   // PK lookup

        if (comment is null)
            throw new NotFoundException($"Comment with ID {id} not found.");

        return comment;  // tracked entity
    }

    // GET MANY COMMENTS (DB)
    public IQueryable<Comment> GetManyAsync()
        => ctx.Comments.AsQueryable();
}
