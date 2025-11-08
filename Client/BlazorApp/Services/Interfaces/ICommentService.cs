using System.Collections.Generic;
using System.Threading.Tasks;
using ApiContracts.Comments;

namespace BlazorApp.Services.Interfaces;

public interface ICommentService
{
    Task<CommentDto> CreateCommentAsync(int postId, CreateCommentDto request);
    Task<CommentDto?> GetCommentByIdAsync(int commentId);
    Task UpdateCommentAsync(int commentId, UpdateCommentDto request);
    Task DeleteCommentAsync(int commentId);
    Task<List<CommentDto>> GetCommentsByPostIdAsync(int postId);
    Task<List<CommentDto>> GetAllCommentsAsync();
}
