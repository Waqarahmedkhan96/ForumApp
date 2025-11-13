using System.Threading.Tasks;
using ApiContracts.Authentication;
using ApiContracts.Users;

namespace BlazorApp.Services.Interfaces;

public interface IAuthService
{
    Task<UserDto> LoginAsync(LoginRequest request);
}
