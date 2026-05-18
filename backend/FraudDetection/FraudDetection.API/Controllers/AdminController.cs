using FraudDetection.API.DTOs;
using FraudDetection.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetection.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles ="admin")]
public class AdminController :ControllerBase
{
    private readonly IAdminService _adminservice;
    public AdminController(IAdminService adminService)
    {
        _adminservice = adminService;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats =  await _adminservice.GetStatsAsync();
        return Ok(ApiResponse<AdminStatsDto>.SuccessResponse(stats,"All stats are here!"));
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(int page=1, int pageSize =20, UserFilterDto filter = null!)
    {
        filter ??=new UserFilterDto();
        var result = await _adminservice.GetUsersAsync(page,pageSize,filter);
        return Ok(ApiResponse<PagedResult<UserResponseDto>>.SuccessResponse(result,"This is successfull!"));
    }
    [HttpGet("users/{UserId}/transactions")]
    public async Task<IActionResult> GetuserTransactions(int UserId, int page=1, int pageSize = 20)
    {
        var user_tran = await _adminservice.GetUserTransactionAsync(UserId,page,pageSize);
        return Ok(ApiResponse<PagedResult<TransactionResponseDto>>.SuccessResponse(user_tran,"This is success"));
    }
    [HttpPatch("transactions/{transactionId}/status")]
    public async Task<IActionResult> UpdateTransaction(int transactionId,string status)
    {
        var updated_t  = await _adminservice.UpdateTransactionStatusAsync(transactionId,status);
        if(updated_t == null)
        {
            return NotFound(ApiResponse<string>.ErrorResponse("No transaction found to update!!"));
        }
        return Ok(ApiResponse<TransactionResponseDto>.SuccessResponse(updated_t,"this is success"));
    }
}