namespace Entities;

public class Post
{
    public int Id { get; set; }           // required int Id
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public int UserId { get; set; }       // FK to User (no navigation yet)
}
