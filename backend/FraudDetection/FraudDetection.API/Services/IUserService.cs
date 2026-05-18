using FraudDetection.API.DTOs;
using FraudDetection.API.Models;

namespace FraudDetection.API.Services;

public interface IUserService
{
    Task<PagedResult<UserResponseDto>> GetAllUsersAsync(UserFilterDto filterDto,int page, int pageSize);
    Task<UserResponseDto?> GetUserByIdAsync(int id);
    Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);
}