using FraudDetection.API.DTOs;
using FraudDetection.API.Models;

namespace FraudDetection.API.Services;

public interface IDeviceService
{
    Task<PagedResult<DeviceResponseDto>> GetDeviceAllAsync(int page,int pageSize);
    Task<Device?> GetDeviceByIdAsync(int id);
    Task<Device?> CreateDeviceAsync(CreateDeviceDto dto);
}