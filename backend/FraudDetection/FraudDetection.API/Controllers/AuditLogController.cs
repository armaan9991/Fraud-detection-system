using FraudDetection.API.DTOs;
using FraudDetection.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetection.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles ="admin")]
public class AuditLogController : ControllerBase
{
    private readonly IAuditLogService _auditservice;
    public AuditLogController(IAuditLogService auditLogService)
    {
        _auditservice = auditLogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLogs(AuditLogFilterDto filterDto, int page =1 , int pageSize =20)
    {
        var logs = await _auditservice.GetLogAsync(filterDto,page,pageSize);

        return Ok(ApiResponse<PagedResult<AuditLogDto>>.SuccessResponse(logs , "Audit Logs!!"));
    }
}