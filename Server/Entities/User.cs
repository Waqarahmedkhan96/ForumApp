namespace Entities;

public class User
{
    public int Id { get; set; }           // required int Id
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
