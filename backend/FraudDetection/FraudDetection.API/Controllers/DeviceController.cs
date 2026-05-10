using FraudDetection.API.Data;
using FraudDetection.API.DTOs;
using FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using FraudDetection.API.Services;


namespace FraudDetection.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class DeviceController : ControllerBase
{
    private readonly IDeviceService _deviceService;
    public  DeviceController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    public  async Task<IActionResult> GetAllDevice()
    {
        var device_list = await _deviceService.GetDeviceAllAsync();
        return Ok(device_list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeviceById(int id)
    {
        var found_device = await _deviceService.GetDeviceByIdAsync(id);

        if (found_device == null)
        {
            return NotFound();
        }
        return Ok(found_device);
    }

    [HttpPost]
    public async Task<IActionResult>  CreateDevice(Device device)
    {
     var CreateDevice = await _deviceService.CreateDeviceAsync(device);
     return CreatedAtAction(
        nameof(GetDeviceById),
        new {id = CreateDevice.DeviceId}
        ,CreateDevice
     );

    }
}