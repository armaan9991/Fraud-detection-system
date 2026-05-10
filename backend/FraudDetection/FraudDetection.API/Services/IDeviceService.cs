using FraudDetection.API.Models;

namespace FraudDetection.API.Services;

public interface IDeviceService
{
    Task<List<Device>> GetDeviceAsync();
    Task<Device?> GetDeviceByIdAsync(int id);
    Task<Device> CreateDeviceAsync(Device device);
}