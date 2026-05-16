using FraudDetection.API.DTOs;

namespace FraudDetection.API.Services;
public interface IAdminService
{
    Task<AdminStatsDto> GetStatsAsync();
    Task<PagedResult<UserResponseDto>> GetUserAync(int page,int pageSize, UserFilterDto filterDto);
    Task<PagedResult<TransactionResponseDto>> GetUserTransactionAsync(int userId, int page, int pageSize, TransactionFilterDto filterDto);
    Task<TransactionResponseDto?> UpdateTransactionStatusAsync(int transactionId, string status);

}