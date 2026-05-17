using System.Security.AccessControl;
using FraudDetection.API.DTOs;

namespace FraudDetection.API.Services;

public interface IAuditLogService
{
    Task CreateLogAsync(int? userId, string action,string entityType, int? entityId, string details);
    Task <PagedResult<AuditLogDto>> GetLogAsync(AuditLogFilterDto filterDto, int page, int pageSize);
}