using FraudDetection.API.DTOs;
using FraudDetection.API.Models;

namespace FraudDetection.API.Services;

public interface IDeviceService
{
    Task<List<Device>> GetDeviceAllAsync();
    Task<Device?> GetDeviceByIdAsync(int id);
    Task<Device> CreateDeviceAsync(CreateDeviceDtos dtos);
}