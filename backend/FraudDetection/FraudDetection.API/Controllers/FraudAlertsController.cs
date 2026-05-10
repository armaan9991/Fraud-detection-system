using FraudDetection.API.DTOs;
using FraudDetection.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetection.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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

        return Ok(alerts);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetAlertById(int id)
    {
        var alert = await _fraudAlert.GetAlertByIdAsync(id);

        if (alert == null)
        {
            return NotFound();
        }

        return Ok(alert);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAlert(CreateFraudAlertDto dto)
    {
        var createdAlert = await _fraudAlert.CreateAlertAsync(dto);

        if (createdAlert == null)
        {
            return NotFound("Transaction not found.");
        }

        return CreatedAtAction(
            nameof(GetAlertById),
            new { id = createdAlert.FraudAlertId },
            createdAlert
        );
    }
}
