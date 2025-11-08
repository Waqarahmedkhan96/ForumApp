using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiContracts.Posts;
using BlazorApp.Services.Interfaces;

namespace BlazorApp.Services.HttpPostService
{
    public class HttpPostService : IPostService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public HttpPostService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ✅ Create a new post
        public async Task<PostDto> CreatePostAsync(CreatePostDto request)
        {
            var response = await _httpClient.PostAsJsonAsync("posts", request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(body);

            return JsonSerializer.Deserialize<PostDto>(body, _json)!;
        }

        // ✅ Get post by ID
        public async Task<PostDto> GetPostByIdAsync(int postId)
        {
            var post = await _httpClient.GetFromJsonAsync<PostDto>($"posts/{postId}");
            return post ?? throw new Exception($"Post with ID {postId} not found");
        }

        // ✅ Update an existing post
        public async Task UpdatePostAsync(int postId, UpdatePostDto request)
        {
            var response = await _httpClient.PutAsJsonAsync($"posts/{postId}", request);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception(body);
            }
        }

        // ✅ Delete a post
        public async Task DeletePostAsync(int postId)
        {
            var response = await _httpClient.DeleteAsync($"posts/{postId}");
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception(body);
            }
        }

        // ✅ Get posts with optional filters (titleContains, authorUserId)
        public async Task<List<PostSummaryDto>> GetPostsAsync(PostQueryParameters queryParameters)
        {
            var url = "posts";

            var queryParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(queryParameters.TitleContains))
                queryParts.Add($"titleContains={Uri.EscapeDataString(queryParameters.TitleContains)}");
            if (queryParameters.AuthorUserId.HasValue)
                queryParts.Add($"authorUserId={queryParameters.AuthorUserId.Value}");

            if (queryParts.Count > 0)
                url += "?" + string.Join("&", queryParts);

            var posts = await _httpClient.GetFromJsonAsync<List<PostSummaryDto>>(url);
            return posts ?? new List<PostSummaryDto>();
        }

        // ✅ Get all posts (no filters)
        public async Task<List<PostSummaryDto>> GetAllPostsAsync()
        {
            var posts = await _httpClient.GetFromJsonAsync<List<PostSummaryDto>>("posts");
            return posts ?? new List<PostSummaryDto>();
        }
    }
}
