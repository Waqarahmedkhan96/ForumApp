using System.Collections.Generic;
using System.Threading.Tasks;
using ApiContracts.Posts;

namespace BlazorApp.Services.Interfaces;

public interface IPostService
{
    Task<PostDto> CreatePostAsync(CreatePostDto request);
    Task<PostDto> GetPostByIdAsync(int postId);
    Task UpdatePostAsync(int postId, UpdatePostDto request);
    Task DeletePostAsync(int postId);
    Task<List<PostSummaryDto>> GetPostsAsync(PostQueryParameters queryParameters);
    Task<List<PostSummaryDto>> GetAllPostsAsync();
}
