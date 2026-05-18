using FraudDetection.API.DTOs;
using FraudDetection.API.Models;

namespace FraudDetection.API.Services;

public interface IDeviceService
{
    Task<PagedResult<DeviceResponseDto>> GetDeviceAllAsync(int page,int pageSize);
    Task<DeviceResponseDto?> GetDeviceByIdAsync(int id);
    Task<DeviceResponseDto?> CreateDeviceAsync(CreateDeviceDto dto);
} 