using FraudDetection.API.DTOs;
using FraudDetection.API.Services;
using Microsoft.AspNetCore.Mvc;
using  Microsoft.AspNetCore.Authorization;
using FraudDetection.API.Models;
using Microsoft.AspNetCore.RateLimiting;

namespace FraudDetection.API.Controllers;

[ApiController]
[EnableRateLimiting("api")]
[Route("api/[controller]")]
[Authorize]
public class FraudAlertController : ControllerBase
{
    private readonly IFraudAlertService _fraudAlert;
    public FraudAlertController(IFraudAlertService fraudAlert)
    {
        _fraudAlert = fraudAlert;
    }

     [HttpGet]
    public async Task<IActionResult> GetAlerts(int page =1,int pageSize =20, [FromQuery]FraudAlertFilterDto filterDto = null!)
    {
        filterDto = new FraudAlertFilterDto();
        var alerts = await _fraudAlert.GetAlertAsync(page,pageSize,filterDto);

        return Ok(ApiResponse<PagedResult<FraudAlertDto>>.SuccessResponse(alerts,"Alerts found!!!"));
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetAlertById(int id)
    {
        var alert = await _fraudAlert.GetAlertByIdAsync(id);

        if (alert == null)
        {
            return NotFound(ApiResponse<string>.ErrorResponse($"no alert found with ID: {id}"));
        }

        return Ok(ApiResponse<FraudAlertDto>.SuccessResponse(alert,$"Found alert with id {id}"));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAlert(CreateFraudAlertDto dto)
    {
        var createdAlert = await _fraudAlert.CreateAlertAsync(dto);

        if (createdAlert == null)
        {
            return NotFound(ApiResponse<string>.ErrorResponse($"Transaction not found with ID {dto.TransactionId}"));
        }

        return CreatedAtAction(
            nameof(GetAlertById),
            new { id = createdAlert.FraudAlertId },
            ApiResponse<FraudAlertDto>.SuccessResponse(createdAlert, "Alert is created Succesfully!")
        );
    }
}
