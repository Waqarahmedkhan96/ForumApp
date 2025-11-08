namespace ApiContracts.Users;

public class UserDto
{
    public int Id { get; set; }
    public required string UserName { get; set; }
}

public class CreateUserDto
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}

public class UpdateUserDto
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}

public class UserListDto
{
    public List<UserDto> Users { get; set; } = new();
}

public class UserQueryParameters
{
    public string? UserNameContains { get; set; }
}
