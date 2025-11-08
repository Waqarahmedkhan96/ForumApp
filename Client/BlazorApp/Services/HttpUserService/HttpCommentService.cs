using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiContracts.Comments;
using BlazorApp.Services.Interfaces;

namespace BlazorApp.Services
{
    public class HttpCommentService : ICommentService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public HttpCommentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Create a new comment for a given post
        public async Task<CommentDto> CreateCommentAsync(int postId, CreateCommentDto request)
        {
            var response = await _httpClient.PostAsJsonAsync($"posts/{postId}/comments", request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(body);

            return JsonSerializer.Deserialize<CommentDto>(body, _json)!;
        }

        //Get a comment by its ID
        public async Task<CommentDto?> GetCommentByIdAsync(int commentId)
        {
            return await _httpClient.GetFromJsonAsync<CommentDto>($"comments/{commentId}");
        }

        // Update a comment
        public async Task UpdateCommentAsync(int commentId, UpdateCommentDto request)
        {
            var response = await _httpClient.PutAsJsonAsync($"comments/{commentId}", request);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception(body);
            }
        }

        // Delete a comment
        public async Task DeleteCommentAsync(int commentId)
        {
            var response = await _httpClient.DeleteAsync($"comments/{commentId}");
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception(body);
            }
        }

        // Get all comments for a specific post
        public async Task<List<CommentDto>> GetCommentsByPostIdAsync(int postId)
        {
            var comments = await _httpClient.GetFromJsonAsync<List<CommentDto>>($"posts/{postId}/comments");
            return comments ?? new List<CommentDto>();
        }

        // Get all comments (optional admin endpoint)
        public async Task<List<CommentDto>> GetAllCommentsAsync()
        {
            var comments = await _httpClient.GetFromJsonAsync<List<CommentDto>>("comments");
            return comments ?? new List<CommentDto>();
        }
    }
}
