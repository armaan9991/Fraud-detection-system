using FraudDetection.API.Controllers;
using FraudDetection.API.Data;
using FraudDetection.API.DTOs;
using FraudDetection.API.Models;


namespace FraudDetection.API.Services;

public  class  DeviceService : IDeviceService
{
    private readonly ApplicationDbContext  _context;
    public DeviceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Device>> GetDeviceAsync()
    {
        var device_list = await _context.Devices.Include(d => d.User).ToListAsync();
        return device_list;
    }

    public  async Task<Device?> GetDeviceByIdAsync(int id)
    {
        var found_device = await _context.Devices.Include(d => d.User).FirstOrDefaultAsync(d => d.DeviceId ==  id);
        return found_device;
    }

    public async Task<Device> CreateDeviceAsync(Device device)
    {
       await _context.Devices.AddAsync(device);
       await _context.SaveChangesAsync();

       return device;
    }
}