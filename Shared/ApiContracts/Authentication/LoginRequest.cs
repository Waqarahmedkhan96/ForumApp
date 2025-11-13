using System;

namespace ApiContracts.Authentication;

public sealed class LoginRequest
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    // Additional properties can be added here as needed like roles, remember me, etc.
}
