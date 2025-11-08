using System.Collections.Generic;
using System.Threading.Tasks;
using ApiContracts.Users;

namespace BlazorApp.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> AddUserAsync(CreateUserDto request);
    Task UpdateUserAsync(int userId, UpdateUserDto request);
    Task DeleteUserAsync(int userId);
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<List<UserDto>> GetAllUsersAsync();
}
