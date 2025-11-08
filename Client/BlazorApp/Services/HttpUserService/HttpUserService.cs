using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiContracts.Users;
using BlazorApp.Services.Interfaces;

namespace BlazorApp.Services.HttpUserService
{
    public class HttpUserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public HttpUserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ✅ Add new user
        public async Task<UserDto> AddUserAsync(CreateUserDto request)
        {
            var response = await _httpClient.PostAsJsonAsync("users", request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(body);

            return JsonSerializer.Deserialize<UserDto>(body, _json)!;
        }

        // Update existing user
        public async Task UpdateUserAsync(int userId, UpdateUserDto request)
        {
            var response = await _httpClient.PutAsJsonAsync($"users/{userId}", request);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception(body);
            }
        }

        // Delete user
        public async Task DeleteUserAsync(int userId)
        {
            var response = await _httpClient.DeleteAsync($"users/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception(body);
            }
        }

        // Get single user
        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            return await _httpClient.GetFromJsonAsync<UserDto>($"users/{userId}");
        }

        // Get all users
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<UserDto>>("users") ?? new List<UserDto>();
        }
    }
}
