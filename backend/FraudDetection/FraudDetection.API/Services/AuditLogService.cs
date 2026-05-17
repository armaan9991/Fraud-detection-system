using FraudDetection.API.Data;
using FraudDetection.API.DTOs;
using FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.API.Services;
public class AuditLogService : IAuditLogService
{
    private readonly ApplicationDbContext _context;
    public AuditLogService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task CreateLogAsync(int? userId, string action,string entityType, int? entityId, string details)
    {
        var log = new AuditLog
        {
            UserId = userId,
            Action =action,
            EntityId = entityId,
            EntityType = entityType,
            Details = details
        };

        _context.AuditLogs.Add(log);

        await _context.SaveChangesAsync();
    }

    public async Task<PagedResult<AuditLogDto>> GetLogAsync(AuditLogFilterDto filterDto, int page, int pageSize)
    {
        var query = _context.AuditLogs.AsQueryable();
        if (filterDto.UserId.HasValue)
        {
            query = query.Where(x=> x.UserId == filterDto.UserId.Value);
        }
        if (filterDto.EntityId.HasValue)
        {
            query = query.Where(x=> x.EntityId == filterDto.EntityId.Value);
        }
              if (!string.IsNullOrEmpty(filterDto.EntityType))
        {
            query = query.Where(x => x.EntityType == filterDto.EntityType);
        }
        
        if (!string.IsNullOrEmpty(filterDto.Action))
        {
            query = query.Where(x => x.Action.ToLower() == filterDto.Action);
        }
        if (filterDto.toCreatedAt.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= filterDto.toCreatedAt.Value);
        }
         if (filterDto.fromCreatedAt.HasValue)
        {
            query = query.Where(x => x.CreatedAt >=  filterDto.fromCreatedAt.Value);
        }
  

        int Totalrecords = await query.CountAsync();

        var items = await query.OrderByDescending(t => t.CreatedAt)
                        .Skip((page-1)*pageSize)
                        .Take(pageSize)
                        .Select( x => new AuditLogDto
                        {
                            AuditLogId = x.AuditLogId,
                            UserId = x.UserId,
                            Action = x.Action,
                            EntityType = x.EntityType,
                            EntityId =x.EntityId,
                            CreatedAt = x.CreatedAt,
                            Details = x.Details
                        }).ToListAsync();
        return new PagedResult<AuditLogDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalRecords = Totalrecords,
            TotalPages = (int)Math.Ceiling(Totalrecords/(double)pageSize)
        };
    }
}