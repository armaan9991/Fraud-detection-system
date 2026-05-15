using FraudDetection.API.DTOs;
using FraudDetection.API.Models;

namespace FraudDetection.API.Services;
public interface IFraudAlertService
{
    Task<PagedResult<FraudAlertDto>> GetAlertAsync(int page, int pageSize);
    Task<FraudAlert?> GetAlertByIdAsync(int id);
    Task<FraudAlert?> CreateAlertAsync(CreateFraudAlertDto dto);
    Task<FraudAlert> CreateAutomaticAlertAsync(int transactionId , string risklevel, string reason);
}