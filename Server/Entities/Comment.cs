namespace Entities;

public class Comment
{
    public int Id { get; set; }           // required int Id
    public string Body { get; set; } = null!;
    public int PostId { get; set; }       // FK to Post
    public int UserId { get; set; }       // FK to User (author)
}
