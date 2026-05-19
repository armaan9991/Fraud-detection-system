using FraudDetection.API.Data;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.API.Jobs;

public class HighRiskUserDetectionJob :IHighRiskUserDetectionJob
{
    private readonly ApplicationDbContext _context;
    public HighRiskUserDetectionJob(ApplicationDbContext context)
    {
        _context = context;
    } 
    public async Task ExecuteAsync()
    {
        var since = DateTime.UtcNow.AddHours(-24);

        var suspiciousUsers = await _context.Transactions
        .Where(t => t.Status =="HIGH" && t.TransactionTime >= since)
        .GroupBy(t => t.UserId)
        .Where(g => g.Count() >=3)
        .Select(g => new {UserId = g.Key, Count = g.Count()})
        .ToListAsync();

        foreach(var suspiciousUser in suspiciousUsers)
        {
            var user = await _context.Users.FindAsync(suspiciousUser.UserId);
            if (user == null || user.IsFlagged) continue;

            user.IsFlagged =  true;
            user.FlagReason = $"{suspiciousUser.Count} High Risk tranasction in last 24 hours..";

        }

        await _context.SaveChangesAsync();
    }
}