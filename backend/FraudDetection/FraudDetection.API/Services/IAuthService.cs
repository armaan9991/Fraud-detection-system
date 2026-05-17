using FraudDetection.API.DTOs;
namespace FraudDetection.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<AuthResponseDto?> RefreshTokenAsync(string refreshtoken);
    Task<bool>  LogoutAsync(string refreshtoken);
} 