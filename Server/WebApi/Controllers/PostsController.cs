using ApiContracts.Posts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using RepositoryContracts.ExceptionHandling;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository _posts;
    private readonly ICommentRepository _comments; // ✅ add

    public PostsController(IPostRepository posts, ICommentRepository comments) // ✅ inject
    {
        _posts = posts;
        _comments = comments;
    }

    // POST /posts
    [HttpPost]
    public async Task<ActionResult<PostDto>> Create([FromBody] CreatePostDto dto)
    {
        var post = new Post { Title = dto.Title, Body = dto.Body, UserId = dto.AuthorUserId };
        var created = await _posts.AddAsync(post);

        var result = new PostDto
        {
            Id = created.Id,
            Title = created.Title,
            Body = created.Body,
            AuthorUserId = created.UserId
        };

        return Created($"/posts/{result.Id}", result);
    }

    // GET /posts/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetById(int id)
    {
        var post = await _posts.GetSingleAsync(id);

        var dto = new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            AuthorUserId = post.UserId
        };

        return Ok(dto);
    }

    // GET /posts?titleContains=...&authorUserId=1
    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetMany([FromQuery] string? titleContains, [FromQuery] int? authorUserId)
    {
        var query = _posts.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(titleContains))
            query = query.Where(p => p.Title.Contains(titleContains, StringComparison.OrdinalIgnoreCase));

        if (authorUserId is not null)
            query = query.Where(p => p.UserId == authorUserId.Value);

        var list = query.Select(p => new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            Body = p.Body,
            AuthorUserId = p.UserId
        }).ToList();

        return Ok(list);
    }

    // PUT /posts/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePostDto dto)
    {
        var existingPost = await _posts.GetSingleAsync(id);   // keep same author
        existingPost.Title = dto.Title;
        existingPost.Body  = dto.Body;

        await _posts.UpdateAsync(existingPost);
        return NoContent();
    }

    // DELETE /posts/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        // ✅ cascade comments for this post
        var commentIds = _comments.GetManyAsync()
                                  .Where(c => c.PostId == id)
                                  .Select(c => c.Id)
                                  .ToList();
        foreach (var cid in commentIds)
            await _comments.DeleteAsync(cid);

        await _posts.DeleteAsync(id);
        return NoContent();
    }
}
