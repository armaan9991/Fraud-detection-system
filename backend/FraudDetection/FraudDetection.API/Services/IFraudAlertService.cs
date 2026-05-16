using FraudDetection.API.DTOs;
using FraudDetection.API.Models;

namespace FraudDetection.API.Services;
public interface IFraudAlertService
{
    Task<PagedResult<FraudAlertDto>> GetAlertAsync(int page, int pageSize,FraudAlertFilterDto filterDto);
    Task<FraudAlertDto?> GetAlertByIdAsync(int id);
    Task<FraudAlertDto?> CreateAlertAsync(CreateFraudAlertDto dto);
    Task<FraudAlertDto> CreateAutomaticAlertAsync(int transactionId , string risklevel, string reason);
}