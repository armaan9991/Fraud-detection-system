using FraudDetection.API.Controllers;
using FraudDetection.API.Data;
using FraudDetection.API.DTOs;
using FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;


namespace FraudDetection.API.Services;

public  class  DeviceService : IDeviceService
{
    private readonly ApplicationDbContext  _context;
    private readonly IAuditLogService _auditlogservice;
    public DeviceService(ApplicationDbContext context, IAuditLogService auditLogService)
    {
        _context = context;
        _auditlogservice = auditLogService;
    }

    public async Task<PagedResult<DeviceResponseDto>> GetDeviceAllAsync(int page,int pageSize)
    {
        var device_list = _context.Devices.Include(d => d.User).AsQueryable();
        var totalrecords = await device_list.CountAsync();

        var items = await device_list.OrderByDescending(t =>t.DeviceId).Skip((page-1)*pageSize)
        .Take(pageSize).Select(t=> new DeviceResponseDto
        {
            DeviceId = t.DeviceId,
            UserId = t.UserId,
            DeviceName =t.DeviceName,
            IPAddress = t.IPAddress,
            LastUsed =t.LastUsed
        }).ToListAsync();

        return new PagedResult<DeviceResponseDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalRecords = totalrecords,
            TotalPages = (int)Math.Ceiling(totalrecords/(double)pageSize)
        };
    }

    public  async Task<Device?> GetDeviceByIdAsync(int id)
    {
        var found_device = await _context.Devices.Include(d => d.User).FirstOrDefaultAsync(d => d.DeviceId ==  id);
        return found_device;
    }

    public async Task<Device?> CreateDeviceAsync(CreateDeviceDto dto)
    {
       var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId);

       if (user == null)
        {
            return null;
        }
        var dev = new Device
        {
            UserId = dto.UserId,
            DeviceName = dto.DeviceName,
            IPAddress = dto.IPAddress,
            LastUsed = DateTime.UtcNow
        };
       _context.Devices.Add(dev);
       await _context.SaveChangesAsync();
       await _auditlogservice.CreateLogAsync(dev.UserId, "Registered new device","device",dev.DeviceId,$"new device is registered device id:{dev.DeviceId} user id :{dto.UserId};"  );

       return dev;
    }
}