using FraudDetection.API.Data;
using FraudDetection.API.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.API.Jobs;

public class DailyFraudReportJob: IDailyFraudReportJob
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _service;
    public DailyFraudReportJob(ApplicationDbContext context, ICacheService service)
    {
        _context = context;
        _service = service;
    }

    public async Task ExecuteAsync()
    {
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var today = DateTime.UtcNow.Date;

        var transaction = await _context.Transactions.Where(t=> t.TransactionTime >= yesterday && t.TransactionTime <= today).ToListAsync();

        var report = new
        {
            Date = yesterday.ToString("yyyy-MM-dd"),
            totalTransactions = transaction.Count,
            TotalAmount = transaction.Sum(t => t.Amount),
            HighRiskCount = transaction.Count(t => t.Status =="HIGH"),
            MediumRiskCount = transaction.Count(t => t.Status =="MEDIUM"),
            LowRiskCount = transaction.Count(t => t.Status =="LOW"),
            AverageFraudScore = transaction.Any()? transaction.Average(t =>t.FraudScore) : 0 // ANY() means if transaction have any row in it then calculate the average for that..
        };

        await _service.setAsync($"Fraud Report _ {yesterday:yyyy-MM-dd}",report,TimeSpan.FromDays(30));
    }
}