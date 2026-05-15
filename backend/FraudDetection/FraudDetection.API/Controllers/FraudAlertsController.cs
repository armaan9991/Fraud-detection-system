using FraudDetection.API.DTOs;
using FraudDetection.API.Services;
using Microsoft.AspNetCore.Mvc;
using  Microsoft.AspNetCore.Authorization;
using FraudDetection.API.Models;

namespace FraudDetection.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles ="Admin")]
public class FraudAlertController : ControllerBase
{
    private readonly IFraudAlertService _fraudAlert;
    public FraudAlertController(IFraudAlertService fraudAlert)
    {
        _fraudAlert = fraudAlert;
    }

     [HttpGet]
    public async Task<IActionResult> GetAlerts()
    {
        var alerts = await _fraudAlert.GetAlertAsync();

        return Ok(ApiResponse<List<FraudAlert>>.SuccessResponse(alerts,"Alert found!!!"));
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetAlertById(int id)
    {
        var alert = await _fraudAlert.GetAlertByIdAsync(id);

        if (alert == null)
        {
            return NotFound(ApiResponse<string>.ErrorResponse($"no alert found with ID: {id}"));
        }

        return Ok(ApiResponse<FraudAlert>.SuccessResponse(alert,$"Found alert with id {id}"));
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
            ApiResponse<FraudAlert>.SuccessResponse(createdAlert, "Alert is created Succesfully!")
        );
    }
}
