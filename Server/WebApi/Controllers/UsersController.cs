using ApiContracts.Users;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using RepositoryContracts.ExceptionHandling;
using System.Linq;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repo;
    private readonly IPostRepository _posts;       // ✅ add
    private readonly ICommentRepository _comments; // ✅ add

    public UsersController(IUserRepository repo, IPostRepository posts, ICommentRepository comments) // ✅ inject
    {
        _repo = repo;
        _posts = posts;
        _comments = comments;
    }

    // POST /users
    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser([FromBody] CreateUserDto request)
    {
        var user = new User(request.UserName, request.Password);
        var created = await _repo.AddAsync(user);

        var dto = new UserDto { Id = created.Id, UserName = created.Username };
        return Created($"/users/{dto.Id}", dto);
    }

    // GET /users/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _repo.GetSingleAsync(id);
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
        await _repo.UpdateAsync(user);
        return NoContent();
    }

    // DELETE /users/{id}
    // ✅ Cascading delete: comments (on any posts) + posts + comments on those posts, then the user.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        // 1) comments written by the user (anywhere)
        var userCommentIds = _comments.GetManyAsync()
                                      .Where(c => c.UserId == id)
                                      .Select(c => c.Id)
                                      .ToList();
        foreach (var cid in userCommentIds)
            await _comments.DeleteAsync(cid);

        // 2) posts written by the user
        var userPostIds = _posts.GetManyAsync()
                                .Where(p => p.UserId == id)
                                .Select(p => p.Id)
                                .ToList();

        // 2a) comments on those posts
        var commentIdsOnUserPosts = _comments.GetManyAsync()
                                             .Where(c => userPostIds.Contains(c.PostId))
                                             .Select(c => c.Id)
                                             .ToList();
        foreach (var cid in commentIdsOnUserPosts)
            await _comments.DeleteAsync(cid);

        // 2b) delete the posts
        foreach (var pid in userPostIds)
            await _posts.DeleteAsync(pid);

        // 3) finally delete the user
        await _repo.DeleteAsync(id);

        return NoContent();
    }
}
