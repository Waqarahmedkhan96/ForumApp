
using ApiContracts.Posts;
using ApiContracts.Comments;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using RepositoryContracts.ExceptionHandling;
using Microsoft.EntityFrameworkCore; // EF async

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository _posts;
    private readonly ICommentRepository _comments;

    public PostsController(IPostRepository posts, ICommentRepository comments)
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
            AuthorUserId = created.UserId,
            AuthorName = null, // don*t load user here
            Comments = new() // empty by default
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
            AuthorUserId = post.UserId,
            AuthorName = null,
            Comments = new()
        };

        return Ok(dto);
    }

    // GET /posts?titleContains=...&authorUserId=1
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetMany(
        [FromQuery] string? titleContains,
        [FromQuery] int? authorUserId)
    {
        var query = _posts.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(titleContains))
        {
            string term = titleContains.ToLower(); // EF-safe
            query = query.Where(p => p.Title.ToLower().Contains(term));
        }

        if (authorUserId is not null)
            query = query.Where(p => p.UserId == authorUserId.Value);

        var list = await query
            .Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Body = p.Body,
                AuthorUserId = p.UserId,
                AuthorName = null,
                Comments = new()
            })
            .ToListAsync(); // async DB

        return Ok(list);
    }

    // PUT /posts/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePostDto dto)
    {
        var existingPost = await _posts.GetSingleAsync(id);   // keeping same author
        existingPost.Title = dto.Title;
        existingPost.Body  = dto.Body;

        await _posts.UpdateAsync(existingPost);
        return NoContent();
    }

    // DELETE /posts/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        //  cascade comments for this post
        var commentIds = await _comments.GetManyAsync()
                                        .Where(c => c.PostId == id)
                                        .Select(c => c.Id)
                                        .ToListAsync(); // async

        foreach (var cid in commentIds)
            await _comments.DeleteAsync(cid);

        await _posts.DeleteAsync(id);
        return NoContent();
    }

    // ADVANCED: GET /posts/{id}/details?includeAuthor=true&includeComments=true
    [HttpGet("{id:int}/details")]
    public async Task<ActionResult<PostDto>> GetDetails(
        int id,
        [FromQuery] bool includeAuthor = false,
        [FromQuery] bool includeComments = false)
    {
        // base query: just this post
        IQueryable<Post> query = _posts.GetManyAsync()
            .Where(p => p.Id == id);

        // includes (joins in SQL)
        if (includeAuthor)
            query = query.Include(p => p.User); // join User table

        if (includeComments)
            query = query.Include(p => p.Comments).ThenInclude(c => c.User);  // join Comment table

        // project to existing PostDto
        var dto = await query
            .Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Body = p.Body,
                AuthorUserId = p.UserId,
                AuthorName = includeAuthor ? p.User.Username : null,

                   Comments = includeComments
                    ? p.Comments.Select(c => new CommentDto
                    {
                        Id = c.Id,
                        PostId = c.PostId,
                        AuthorUserId = c.UserId,
                        AuthorName = c.User.Username,
                        Body = c.Body,
                        CreatedAt = c.CreatedAt   
                    }).ToList()
                    : new List<CommentDto>()
            })
            .FirstOrDefaultAsync(); // execute query

        if (dto is null)
            return NotFound();

        return Ok(dto);
    }
}
