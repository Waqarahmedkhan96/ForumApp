namespace Entities;

public class User
{
    public User() { }
    public User(string userName, string password)
    {
        Username = userName;
        Password = password;
    }

    public int Id { get; set; }           // required int Id
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
