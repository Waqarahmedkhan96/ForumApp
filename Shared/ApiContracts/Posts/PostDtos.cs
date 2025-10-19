namespace ApiContracts.Posts;

public class CreatePostDto
{
    public required string Title { get; set; }
    public required string Body  { get; set; }
    public required int AuthorUserId { get; set; }
}

public class PostDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Body  { get; set; }
    public required int AuthorUserId { get; set; }
}
