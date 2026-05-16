using FraudDetection.API.Data;
using FraudDetection.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.API.Services;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;
    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminStatsDto> GetStatsAsync()
    {
        var totalUsers = await _context.Users.CountAsync();
        var totalTransactions = await _context.Transactions.CountAsync();
        var totalAlerts = await _context.FraudAlerts.CountAsync();
        var totalAmount = await _context.Transactions.SumAsync(t=> t.Amount);

        var byStatus = await _context.Transactions
                    .GroupBy(t => t.Status)
                    .Select(g => new{g.Key, Count = g.Count()})  // g is group g.key is status, count counts how many are in there.
                    .ToDictionaryAsync(x=>x.Key, x=>x.Count); // add these groups in dictionary

        var byRisk = await _context.FraudAlerts
                    .GroupBy(t=>t.RiskLevel)
                    .Select(g => new {g.Key,Count= g.Count()})
                    .ToDictionaryAsync(x=> x.Key , x=>x.Count);

        var avgScore = totalTransactions >0 ?
                    await _context.Transactions.AverageAsync(t => (double)t.FraudScore) : 0;

        return new AdminStatsDto
        {
            TotalUsers = totalUsers,
            TotalTranction = totalTransactions,
            TotalFraudAlerts = totalAlerts,
            TotalTransactionAmount = totalAmount,
            AverageFraudScore = avgScore,
            TransactionsByStatus = byStatus,
            AlertRiskLevel = byRisk
        };
    }

}