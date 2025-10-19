using ApiContracts.Posts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository _posts;
    public PostsController(IPostRepository posts) => _posts = posts;

    // POST /posts
    [HttpPost]
    public async Task<ActionResult<PostDto>> Create([FromBody] CreatePostDto dto)
    {
        var post = new Post { Title = dto.Title, Body = dto.Body, UserId = dto.AuthorUserId };
        var created = await _posts.AddAsync(post);

        var result = new PostDto { Id = created.Id, Title = created.Title, Body = created.Body, AuthorUserId = created.UserId };
        return Created($"/posts/{result.Id}", result);
    }

    // GET /posts/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetById(int id)
    {
        Post? p;
        try { p = await _posts.GetSingleAsync(id); }
        catch (KeyNotFoundException) { return NotFound(); }

        if (p is null) return NotFound();

        return new PostDto { Id = p.Id, Title = p.Title, Body = p.Body, AuthorUserId = p.UserId };
    }

    // GET /posts?titleContains=...&authorUserId=1
    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetMany([FromQuery] string? titleContains, [FromQuery] int? authorUserId)
    {
        var q = _posts.GetManyAsync(); // IQueryable<Post>
        if (!string.IsNullOrWhiteSpace(titleContains))
            q = q.Where(p => p.Title.Contains(titleContains, StringComparison.OrdinalIgnoreCase));
        if (authorUserId is not null)
            q = q.Where(p => p.UserId == authorUserId.Value);

        var list = q
            .Select(p => new PostDto { Id = p.Id, Title = p.Title, Body = p.Body, AuthorUserId = p.UserId })
            .ToList();

        return list;
    }

    // PUT /posts/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePostDto dto)
    {
        var post = new Post { Id = id, Title = dto.Title, Body = dto.Body, UserId = dto.AuthorUserId };
        try
        {
            await _posts.UpdateAsync(post);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // DELETE /posts/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _posts.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
