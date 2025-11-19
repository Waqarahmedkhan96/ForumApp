using System.Collections.Generic; // for ICollection

namespace Entities;

public class Post
{
    public int Id { get; set; }  // PK by convention
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;

    public int UserId { get; set; }        // FK to User
    public User User { get; set; } = null!;    // navigation User

    public ICollection<Comment> Comments { get; set; } // post comments
        = new List<Comment>();                         // init collection

    public Post() { }  // fOr EFC only

    public Post(string title, string body, int userId)
    {
        Title = title; 
        Body = body;  
        UserId = userId;   // set author FK
    }
}
