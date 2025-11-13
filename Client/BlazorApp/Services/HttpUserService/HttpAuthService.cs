using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts.Authentication;
using ApiContracts.Users;
using BlazorApp.Services.Interfaces;

namespace BlazorApp.Services;

public class HttpAuthService : IAuthService
{
    private readonly HttpClient client;

    public HttpAuthService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<UserDto> LoginAsync(LoginRequest request)
    {
        var response = await client.PostAsJsonAsync("auth/login", request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(content);

        return JsonSerializer.Deserialize<UserDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
}
