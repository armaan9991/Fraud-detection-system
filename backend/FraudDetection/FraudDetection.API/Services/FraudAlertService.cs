using FraudDetection.API.DTOs;
using FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;
using FraudDetection.API.Data;
using FraudDetection.API.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FraudDetection.API.Services;

public class FraudAlertService : IFraudAlertService
{
    private readonly ApplicationDbContext _context;
    public FraudAlertService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<FraudAlertDto>> GetAlertAsync(int page , int pageSize)
    {
        var alert_list  =  _context.FraudAlerts.Include(t => t.Transaction).AsQueryable();
        var totalrecords = await alert_list.CountAsync();

        var items =  await alert_list.OrderByDescending(t => t.FraudAlertId).Skip((page-1)*pageSize).Take(pageSize).Select(t=> new FraudAlertDto
        {
            FraudAlertId = t.FraudAlertId,
            TransactionId = t.TransactionId,
            RiskLevel = t.RiskLevel,
            Reason = t.Reason,
            CreatedAt = t.CreatedAt
        }).ToListAsync();

        return new PagedResult<FraudAlertDto>
        {
            Items =items,
            Page = page,
            PageSize = pageSize,
            TotalRecords =totalrecords,
            TotalPages = (int)Math.Ceiling(totalrecords/(double)pageSize)
        };
    }

    public async Task<FraudAlertDto?> GetAlertByIdAsync(int id)
    {
        var found_alert = await _context.FraudAlerts.Include(u => u.Transaction).FirstOrDefaultAsync(f => f.FraudAlertId == id);
        return found_alert;
    }

    public async Task<FraudAlertDto?> CreateAlertAsync(CreateFraudAlertDto dto)
    {
        var transaction = await _context.Transactions.FirstOrDefaultAsync( t => t.TransactionId == dto.TransactionId);

        if (transaction == null)
        {
            return null;
        }

        var alert = new FraudAlert
        {
            TransactionId = dto.TransactionId,
            RiskLevel = dto.RiskLevel,
            Reason = dto.Reason,
            CreatedAt = DateTime.UtcNow
        };

        _context.FraudAlerts.Add(alert);

        await _context.SaveChangesAsync();
        return alert;
    }
    public async Task<FraudAlertDto> CreateAutomaticAlertAsync(int transactionId , string risklevel , string reason)
    {
        var alert = new FraudAlertDto
        {
            TransactionId = transactionId,
            RiskLevel = risklevel,
            Reason = reason,
            CreatedAt = DateTime.UtcNow
        };

        _context.FraudAlerts.Add(alert);

        await _context.SaveChangesAsync();
        return alert;
    }
}