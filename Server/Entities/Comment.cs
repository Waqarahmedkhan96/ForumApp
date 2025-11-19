namespace Entities;

public class Comment
{
    public int Id { get; set; }             // PK by convention
    public string Body { get; set; } = null!; 

    public int PostId { get; set; }         // FK to Post
    public Post Post { get; set; } = null!; // navigation Post

    public int UserId { get; set; }         // FK to User
    public User User { get; set; } = null!; // navigation User

    public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public Comment() { } // For EFC only

    public Comment(string body, int postId, int userId)
    {
        Body = body; 
        PostId = postId; // set post FK
        UserId = userId; // set author FK
    }
}
