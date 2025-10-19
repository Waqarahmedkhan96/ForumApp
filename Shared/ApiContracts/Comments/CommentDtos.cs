namespace ApiContracts.Comments;

public class CreateCommentDto
{
    public required int PostId { get; set; }
    public required int AuthorUserId { get; set; }
    public required string Body { get; set; }
}

public class CommentDto
{
    public int Id { get; set; }
    public required int PostId { get; set; }
    public required int AuthorUserId { get; set; }
    public required string Body { get; set; }
}
