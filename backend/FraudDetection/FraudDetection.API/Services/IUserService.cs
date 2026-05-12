using FraudDetection.API.DTOs;
using FraudDetection.API.Models;

namespace FraudDetection.API.Services;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(CreateUserDtos dto);
}