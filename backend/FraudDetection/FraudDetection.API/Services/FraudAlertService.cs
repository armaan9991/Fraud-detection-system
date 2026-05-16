using FraudDetection.API.DTOs;
using  FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;
using FraudDetection.API.Data;

namespace FraudDetection.API.Services;

public class FraudAlertService : IFraudAlertService
{
    private readonly ApplicationDbContext _context;

    public FraudAlertService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<FraudAlertDto>> GetAlertAsync(int page, int pageSize)
    {
        var alert_list = _context.FraudAlerts.Include(t => t.Transaction).AsQueryable();
        var totalRecords = await alert_list.CountAsync();

        var items = await alert_list
            .OrderByDescending(t => t.FraudAlertId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new FraudAlertDto
            {
                FraudAlertId = t.FraudAlertId,
                TransactionId = t.TransactionId,
                RiskLevel = t.RiskLevel,
                Reason = t.Reason,
                CreatedAt = t.CreatedAt
            }).ToListAsync();

        return new PagedResult<FraudAlertDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
        };
    }

    public async Task<FraudAlertDto?> GetAlertByIdAsync(int id)
    {
        var found_alert = await _context.FraudAlerts
            .Include(u => u.Transaction)
            .FirstOrDefaultAsync(f => f.FraudAlertId == id);

        if (found_alert == null) return null;

        return new FraudAlertDto
        {
            FraudAlertId = found_alert.FraudAlertId,
            TransactionId = found_alert.TransactionId,
            RiskLevel = found_alert.RiskLevel,
            Reason = found_alert.Reason,
            CreatedAt = found_alert.CreatedAt
        };
    }

    public async Task<FraudAlertDto?> CreateAlertAsync(CreateFraudAlertDto dto)
    {
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.TransactionId == dto.TransactionId);

        if (transaction == null) return null;

        var alert = new FraudAlert
        {
            TransactionId = dto.TransactionId,
            RiskLevel = dto.RiskLevel,
            Reason = dto.Reason,
            CreatedAt = DateTime.UtcNow
        };

        _context.FraudAlerts.Add(alert);
        await _context.SaveChangesAsync();

        return new FraudAlertDto
        {
            FraudAlertId = alert.FraudAlertId,
            TransactionId = alert.TransactionId,
            RiskLevel = alert.RiskLevel,
            Reason = alert.Reason,
            CreatedAt = alert.CreatedAt
        };
    }

    public async Task<FraudAlertDto> CreateAutomaticAlertAsync(int transactionId, string riskLevel, string reason)
    {
        var alert = new FraudAlert
        {
            TransactionId = transactionId,
            RiskLevel = riskLevel,
            Reason = reason,
            CreatedAt = DateTime.UtcNow
        };

        _context.FraudAlerts.Add(alert);
        await _context.SaveChangesAsync();

        return new FraudAlertDto
        {
            FraudAlertId = alert.FraudAlertId,
            TransactionId = alert.TransactionId,
            RiskLevel = alert.RiskLevel,
            Reason = alert.Reason,
            CreatedAt = alert.CreatedAt
        };
    }
}