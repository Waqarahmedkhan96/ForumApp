using ApiContracts.Users;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _users;
    public UsersController(IUserRepository users) => _users = users;

    // POST /users
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto)
    {
        var user = new User { Username = dto.UserName, Password = dto.Password };
        var created = await _users.AddAsync(user);

        var result = new UserDto { Id = created.Id, UserName = created.Username };
        return Created($"/users/{result.Id}", result);
    }

    // GET /users/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        User? u;
        try { u = await _users.GetSingleAsync(id); }
        catch (KeyNotFoundException) { return NotFound(); }

        if (u is null) return NotFound();

        return new UserDto { Id = u.Id, UserName = u.Username };
    }

    // GET /users?usernameContains=waq
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetMany([FromQuery] string? usernameContains)
    {
        var q = _users.GetManyAsync(); // IQueryable<User>
        if (!string.IsNullOrWhiteSpace(usernameContains))
            q = q.Where(u => u.Username.Contains(usernameContains, StringComparison.OrdinalIgnoreCase));

        var list = q
            .Select(u => new UserDto { Id = u.Id, UserName = u.Username })
            .ToList(); // materialize IQueryable

        return list;
    }

    // PUT /users/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUserDto dto)
    {
        // simple update DTO reuse (username+password)
        var user = new User { Id = id, Username = dto.UserName, Password = dto.Password };
        try
        {
            await _users.UpdateAsync(user);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // DELETE /users/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _users.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
