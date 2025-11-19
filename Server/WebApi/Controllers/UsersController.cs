using ApiContracts.Users;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using RepositoryContracts.ExceptionHandling;
using System.Linq;
using Microsoft.EntityFrameworkCore; // EF async extensions

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly IPostRepository _posts;  
    private readonly ICommentRepository _comments;

    public UsersController(IUserRepository user, IPostRepository posts, ICommentRepository comments)
    {
        _users = user;
        _posts = posts;
        _comments = comments;
    }

    // POST /users
    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser([FromBody] CreateUserDto request)
    {
        var user = new User(request.UserName, request.Password);
        var created = await _users.AddAsync(user);

        var dto = new UserDto { Id = created.Id, UserName = created.Username };
        return Created($"/users/{dto.Id}", dto);
    }

    // GET /users/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _users.GetSingleAsync(id);
        var dto = new UserDto { Id = user.Id, UserName = user.Username };
        return Ok(dto);
    }

    // GET /users?usernameContains=waq
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] string? usernameContains)
    {
        var query = _users.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(usernameContains))
        {
            string term = usernameContains.ToLower(); // EF-safe comparison
            query = query.Where(u => u.Username.ToLower().Contains(term));
        }

        var list = await query
            .Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.Username
            })
            .ToListAsync(); // async DB query

        return Ok(list);
    }

    // PUT /users/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] CreateUserDto request)
    {
        var user = new User(request.UserName, request.Password) { Id = id };
        await _users.UpdateAsync(user);
        return NoContent();
    }

    // DELETE /users/{id}
    // Cascading delete: comments (on any posts) + posts + comments on those posts, then the user.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        // 1) comments written by the user (anywhere)
        var userCommentIds = await _comments.GetManyAsync()
                                            .Where(c => c.UserId == id)
                                            .Select(c => c.Id)
                                            .ToListAsync(); // async list

        foreach (var cid in userCommentIds)
            await _comments.DeleteAsync(cid);

        // 2) posts written by the user
        var userPostIds = await _posts.GetManyAsync()
                                      .Where(p => p.UserId == id)
                                      .Select(p => p.Id)
                                      .ToListAsync(); // async list

        // 2a) comments on those posts
        var commentIdsOnUserPosts = await _comments.GetManyAsync()
                                                   .Where(c => userPostIds.Contains(c.PostId))
                                                   .Select(c => c.Id)
                                                   .ToListAsync(); // async list

        foreach (var cid in commentIdsOnUserPosts)
            await _comments.DeleteAsync(cid);

        // 2b) delete the posts
        foreach (var pid in userPostIds)
            await _posts.DeleteAsync(pid);

        // 3) finally delete the user
        await _users.DeleteAsync(id);

        return NoContent();
    }
}
