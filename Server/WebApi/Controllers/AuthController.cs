using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using ApiContracts.Authentication;
using ApiContracts.Users;
using System.Linq;
using Microsoft.EntityFrameworkCore; // EF async

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository users;

        public AuthController(IUserRepository users)
        {
            this.users = users;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequest loginRequest)
        {
            // 1️ Find user by username (async, EF-safe)
            string name = loginRequest.UserName.ToLower();
            var user = await users.GetManyAsync()
                .FirstOrDefaultAsync(u =>
                    u.Username.ToLower() == name);

            // 2️ If not found → Unauthorized
            if (user is null)
                return Unauthorized("Invalid username or password.");

            // 3️ Check password
            if (user.Password != loginRequest.Password)
                return Unauthorized("Invalid username or password.");

            // 4️ Convert to DTO (never send password)
            var dto = new UserDto
            {
                Id = user.Id,
                UserName = user.Username
            };

            // 5️ Return always the UserDto not the object itself
            return Ok(dto);
        }
    }
}
