using FraudDetection.API.DTOs;
using FraudDetection.API.Models;

namespace FraudDetection.API.Services;

public interface IUserService
{
    Task<PagedResult<UserResponseDto>> GetAllUsersAsync(int page,int pageSize,UserFilterDto filterDto);
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(CreateUserDto dto);
}