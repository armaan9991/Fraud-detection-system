using FraudDetection.API.Data;
using FraudDetection.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.API.Jobs;

public class HighRiskUserDetectionJob :IHighRiskUserDetectionJob
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _service;
    public HighRiskUserDetectionJob(ApplicationDbContext context, IEmailService service)
    {
        _context = context;
        _service =service;
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

            var emailbody = $"Account Security \n hello {user.Name}, \n Your account is flagged! \n we have detected an unusual activity";
            await _service.SendEmailAsync(user.Email,"Account Flagged high risk",emailbody);
        }

        await _context.SaveChangesAsync();
    }
}