using FraudDetection.API.Data;
using FraudDetection.API.DTOs;
using FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using FraudDetection.API.Services;
using  Microsoft.AspNetCore.Authorization;



namespace FraudDetection.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeviceController : ControllerBase
{
    private readonly IDeviceService _deviceService;
    public  DeviceController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    public  async Task<IActionResult> GetAllDevice(int page =1 , int pageSize =20)
    {
        var device_list = await _deviceService.GetDeviceAllAsync(page,pageSize);
        if (device_list == null)
        {
            return NotFound(ApiResponse<string>.ErrorResponse("no device present"));
            // return NotFound();
        }
        return Ok(ApiResponse<PagedResult<DeviceResponseDto>>.SuccessResponse(device_list,"Device present"));
        // return Ok(device_list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeviceById(int id)
    {
        var found_device = await _deviceService.GetDeviceByIdAsync(id);

        if (found_device == null)
        {
            return NotFound(ApiResponse<string>.ErrorResponse($"No device of {id} is present!"));
            // return NotFound();
        }
        return Ok(ApiResponse<Device>.SuccessResponse(found_device ,$"Device with id {id} is found"));
        // return Ok(found_device);
    }

    [HttpPost]
    public async Task<IActionResult>  CreateDevice(CreateDeviceDto dtos)
    {
     var CreateDevice = await _deviceService.CreateDeviceAsync(dtos);
     
     if (CreateDevice == null)
        {
            return NotFound(ApiResponse<string>.ErrorResponse("Couldnt find user with this id to register Device with user."));
        }
     return CreatedAtAction(
        nameof(GetDeviceById),
        new {id = CreateDevice.DeviceId}
        ,ApiResponse<Device>.SuccessResponse(CreateDevice, "device is created!!")
     );

    }
}