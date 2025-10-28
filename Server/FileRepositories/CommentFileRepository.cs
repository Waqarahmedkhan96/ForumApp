using System.Linq;
using Entities;
using RepositoryContracts;
using RepositoryContracts.ExceptionHandling; // for NotFoundException & ValidationException

namespace FileRepositories;

public class CommentFileRepository : FileRepository<Comment>, ICommentRepository
{
    public CommentFileRepository() : base(Path.Combine("Data", "comments.json")) { }

    // ADD COMMENT
    public async Task<Comment> AddAsync(Comment comment)
    {
        var items = await LoadAsync();

        // validate input
        if (string.IsNullOrWhiteSpace(comment.Body))
            throw new ValidationException("Comment body cannot be empty.");
        if (comment.PostId <= 0)
            throw new ValidationException("Post ID must be greater than zero.");
        if (comment.UserId <= 0)
            throw new ValidationException("User ID must be greater than zero.");

        // assign new ID
        comment.Id = items.Count == 0 ? 1 : items.Max(x => x.Id) + 1;

        items.Add(comment);
        await SaveAsync(items);
        return comment;
    }

    // UPDATE COMMENT
    public async Task UpdateAsync(Comment comment)
    {
        var items = await LoadAsync();
        var idx = items.FindIndex(x => x.Id == comment.Id);

        if (idx < 0)
            throw new NotFoundException($"Comment with ID {comment.Id} not found.");

        // validate body
        if (string.IsNullOrWhiteSpace(comment.Body))
            throw new ValidationException("Comment body cannot be empty.");

        items[idx] = comment;
        await SaveAsync(items);
    }

    // DELETE COMMENT
    public async Task DeleteAsync(int id)
    {
        var items = await LoadAsync();
        var removed = items.RemoveAll(x => x.Id == id);

        if (removed == 0)
            throw new NotFoundException($"Comment with ID {id} not found.");

        await SaveAsync(items);
    }

    // GET SINGLE COMMENT
    public async Task<Comment> GetSingleAsync(int id)
    {
        var items = await LoadAsync();
        var comment = items.SingleOrDefault(x => x.Id == id);

        if (comment is null)
            throw new NotFoundException($"Comment with ID {id} not found.");

        return comment;
    }

    // GET MANY COMMENTS
    public IQueryable<Comment> GetManyAsync()
        => LoadAsync().Result.AsQueryable();
}
