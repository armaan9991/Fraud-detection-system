using FraudDetection.API.Models;
using FraudDetection.API.DTOs;
using FraudDetection.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using  Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.RateLimiting;

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
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var response = await _authService.RegisterAsync(dto);
        if (response == null)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse("User is already Present!"));
            // return BadRequest("User is already present!");
        }
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response,"User Register"));
        // return Ok(response);
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var response = await _authService.LoginAsync(dto);
        if (response == null)
        {
            return NotFound(ApiResponse<string>.ErrorResponse("no User found!"));
        }

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Login successfull!"));
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Refresh(RefreshTokenRequestDto dto)
    {
        var res = await _authService.RefreshTokenAsync(dto.RefreshToken);
        if (res == null)
        {
            return NotFound(ApiResponse<string>.ErrorResponse("Cant Refresh token"));
        }

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(res, "Token is Refreshed!"));
    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequestDto dto)
    {
        var response = await _authService.LogoutAsync(dto.RefreshToken);
        if(response == false)
        {
            return NotFound(ApiResponse<string>.ErrorResponse("User was not logged in!!"));
        }
        return Ok(ApiResponse<bool>.SuccessResponse(response, "Logout successfull!"));
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