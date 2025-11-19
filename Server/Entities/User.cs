using System.Collections.Generic;  // for ICollection

namespace Entities;

// one user → many posts & comments
public class User
{
    public int Id { get; set; } // PK by convention
    public string Username { get; set; } = null!; 
    public string Password { get; set; } = null!; 

    public ICollection<Post> Posts { get; set; }  // user’s posts
        = new List<Post>();                       // init collection

    public ICollection<Comment> Comments { get; set; } // user’s comments
        = new List<Comment>();                         // init collection

    public User() { }  // For EFC only

    public User(string userName, string password) 
    {
        Username = userName;       
        Password = password;                   
    }
}
