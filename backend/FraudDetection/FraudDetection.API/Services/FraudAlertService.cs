using FraudDetection.API.DTOs;
using FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;
using FraudDetection.API.Data;
using FraudDetection.API.Controllers;

namespace FraudDetection.API.Services;

public class FraudAlertService : IFraudAlertService
{
    private readonly ApplicationDbContext _context;
    public FraudAlertService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FraudAlert>> GetAlertAsync()
    {
        var alert_list  = await _context.FraudAlerts.Include(t => t.Transaction).ToListAsync();
        return alert_list;
    }

    public async Task<FraudAlert?> GetAlertByIdAsync(int id)
    {
        var found_alert = await _context.FraudAlerts.Include(u => u.Transaction).FirstOrDefaultAsync(f => f.FraudAlertId == id);
        return found_alert;
    }

    public async Task<FraudAlert?> CreateAlertAsync(CreateFraudAlertDto dto)
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
}