using FraudDetection.API.Controllers;
using FraudDetection.API.Data;
using FraudDetection.API.DTOs;
using FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;


namespace FraudDetection.API.Services;

public  class  DeviceService : IDeviceService
{
    private readonly ApplicationDbContext  _context;
    public DeviceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Device>> GetDeviceAllAsync()
    {
        var device_list = await _context.Devices.Include(d => d.User).ToListAsync();
        return device_list;
    }

    public  async Task<Device?> GetDeviceByIdAsync(int id)
    {
        var found_device = await _context.Devices.Include(d => d.User).FirstOrDefaultAsync(d => d.DeviceId ==  id);
        return found_device;
    }

    public async Task<Device> CreateDeviceAsync(CreateDeviceDtos dtos)
    {
       var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dtos.UserId);

       if (user == null)
        {
            return null;
        }
        var dev = new Device
        {
            UserId = dtos.UserId,
            DeviceName = dtos.DeviceName,
            IPAddress = dtos.IPAddress,
            LastUsed = DateTime.UtcNow
        };
       _context.Devices.Add(dev);
       await _context.SaveChangesAsync();

       return dev;
    }
}