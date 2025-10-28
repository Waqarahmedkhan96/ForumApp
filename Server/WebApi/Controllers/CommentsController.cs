using ApiContracts.Comments;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using RepositoryContracts.ExceptionHandling;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository _comments;

    public CommentsController(ICommentRepository comments)
    {
        _comments = comments;
    }

    // POST /comments
    [HttpPost]
    public async Task<ActionResult<CommentDto>> Create([FromBody] CreateCommentDto dto)
    {
        // Create comment and let repository handle validation
        var comment = new Comment
        {
            Body = dto.Body,
            PostId = dto.PostId,
            UserId = dto.AuthorUserId
        };

        var created = await _comments.AddAsync(comment);

        var result = new CommentDto
        {
            Id = created.Id,
            Body = created.Body,
            PostId = created.PostId,
            AuthorUserId = created.UserId
        };

        return Created($"/comments/{result.Id}", result);
    }

    // GET /comments/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetById(int id)
    {
        var comment = await _comments.GetSingleAsync(id); // repo throws NotFoundException if not found

        var dto = new CommentDto
        {
            Id = comment.Id,
            Body = comment.Body,
            PostId = comment.PostId,
            AuthorUserId = comment.UserId
        };

        return Ok(dto);
    }

    // GET /comments?postId=&authorUserId=
    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetMany([FromQuery] int? postId, [FromQuery] int? authorUserId)
    {
        var query = _comments.GetManyAsync();

        if (postId is not null)
            query = query.Where(c => c.PostId == postId.Value);

        if (authorUserId is not null)
            query = query.Where(c => c.UserId == authorUserId.Value);

        var list = query.Select(c => new CommentDto
        {
            Id = c.Id,
            Body = c.Body,
            PostId = c.PostId,
            AuthorUserId = c.UserId
        }).ToList();

        return Ok(list);
    }

    // PUT /comments/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCommentDto dto)
    {
        var comment = new Comment
        {
            Id = id,
            Body = dto.Body,
            PostId = dto.PostId,
            UserId = dto.AuthorUserId
        };

        await _comments.UpdateAsync(comment); // repo handles NotFoundException
        return NoContent();
    }

    // DELETE /comments/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _comments.DeleteAsync(id); // repo handles NotFoundException
        return NoContent();
    }
}
