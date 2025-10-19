using ApiContracts.Comments;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository _comments;
    public CommentsController(ICommentRepository comments) => _comments = comments;

    // POST /comments
    [HttpPost]
    public async Task<ActionResult<CommentDto>> Create([FromBody] CreateCommentDto dto)
    {
        var c = new Comment { Body = dto.Body, PostId = dto.PostId, UserId = dto.AuthorUserId };
        var created = await _comments.AddAsync(c);

        var result = new CommentDto { Id = created.Id, Body = created.Body, PostId = created.PostId, AuthorUserId = created.UserId };
        return Created($"/comments/{result.Id}", result);
    }

    // GET /comments/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetById(int id)
    {
        Comment? c;
        try { c = await _comments.GetSingleAsync(id); }
        catch (KeyNotFoundException) { return NotFound(); }

        if (c is null) return NotFound();

        return new CommentDto { Id = c.Id, Body = c.Body, PostId = c.PostId, AuthorUserId = c.UserId };
    }

    // GET /comments?postId=&authorUserId=
    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetMany([FromQuery] int? postId, [FromQuery] int? authorUserId)
    {
        var q = _comments.GetManyAsync(); // IQueryable<Comment>
        if (postId is not null)      q = q.Where(c => c.PostId == postId.Value);
        if (authorUserId is not null) q = q.Where(c => c.UserId == authorUserId.Value);

        var list = q
            .Select(c => new CommentDto { Id = c.Id, Body = c.Body, PostId = c.PostId, AuthorUserId = c.UserId })
            .ToList();

        return list;
    }

    // PUT /comments/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCommentDto dto)
    {
        var c = new Comment { Id = id, Body = dto.Body, PostId = dto.PostId, UserId = dto.AuthorUserId };
        try
        {
            await _comments.UpdateAsync(c);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // DELETE /comments/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _comments.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
