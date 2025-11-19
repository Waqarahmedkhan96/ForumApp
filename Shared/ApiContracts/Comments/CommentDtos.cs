namespace ApiContracts.Comments;

public class CommentDto
{
    public int Id { get; set; }
    public required int PostId { get; set; }
    public required int AuthorUserId { get; set; }
    public string? AuthorName { get; set; } 
    public required string Body { get; set; }
    public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}


public class CreateCommentDto
{
    public required int PostId { get; set; }
    public required string Body { get; set; }
    public required int AuthorUserId { get; set; }
}


public class UpdateCommentDto
{
    public required int PostId { get; set; }
    public required string Body { get; set; }
}

public class CommentQueryParameters
{
    public int? PostId { get; set; }
    public int? AuthorUserId { get; set; }
}
