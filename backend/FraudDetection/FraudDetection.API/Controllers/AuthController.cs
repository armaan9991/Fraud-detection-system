using FraudDetection.API.Models;
using FraudDetection.API.DTOs;
using FraudDetection.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using  Microsoft.AspNetCore.Authorization;

namespace FraudDetection.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var response = await _authService.RegisterAsync(dto);
        if (response == null)
        {
            return BadRequest("User is already present!");
        }
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var response = await _authService.LoginAsync(dto);
        if (response == null)
        {
            return NotFound("no User found!");
        }

        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId =
            User.FindFirst(
                System.Security.Claims
                .ClaimTypes.NameIdentifier
            )?.Value;

        var email =
            User.FindFirst(
                System.Security.Claims
                .ClaimTypes.Email
            )?.Value;

        var role =
            User.FindFirst(
                System.Security.Claims
                .ClaimTypes.Role
            )?.Value;

        return Ok(new
        {
            UserId = userId,
            Email = email,
            Role = role
        });
    }
}