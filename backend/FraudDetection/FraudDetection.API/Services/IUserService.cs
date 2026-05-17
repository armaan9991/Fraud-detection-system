using FraudDetection.API.DTOs;
using FraudDetection.API.Models;

namespace FraudDetection.API.Services;

public interface IUserService
{
    Task<PagedResult<UserResponseDto>> GetAllUsersAsync(UserFilterDto filterDto,int page, int pageSize);
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(CreateUserDto dto);
}