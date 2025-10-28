using ApiContracts.Users;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using RepositoryContracts.ExceptionHandling; // for exceptions from repository

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repo;

    public UsersController(IUserRepository repo)
    {
        _repo = repo;
    }

    // POST /users
    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser([FromBody] CreateUserDto request)
    {
        // Create user and let repository handle validation
        var user = new User(request.UserName, request.Password);
        var created = await _repo.AddAsync(user);

        var dto = new UserDto { Id = created.Id, UserName = created.Username };
        return Created($"/users/{dto.Id}", dto);
    }

    // GET /users/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _repo.GetSingleAsync(id); // repo throws NotFoundException if not found
        var dto = new UserDto { Id = user.Id, UserName = user.Username };
        return Ok(dto);
    }

    // GET /users?usernameContains=waq
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetUsers([FromQuery] string? usernameContains)
    {
        var query = _repo.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(usernameContains))
        {
            query = query.Where(u =>
                u.Username.Contains(usernameContains, StringComparison.OrdinalIgnoreCase));
        }

        var list = query.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.Username
        }).ToList();

        return Ok(list);
    }

    // PUT /users/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] CreateUserDto request)
    {
        var user = new User(request.UserName, request.Password) { Id = id };
        await _repo.UpdateAsync(user); // repo handles not found or duplicate username
        return NoContent();
    }

    // DELETE /users/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _repo.DeleteAsync(id); // repo throws NotFoundException if not found
        return NoContent();
    }
}
