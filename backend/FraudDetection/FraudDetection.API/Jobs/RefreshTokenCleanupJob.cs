using FraudDetection.API.Data;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.API.Jobs;

public class RefreshTokenCleanupJob : IRefreshTokenCleanupJob
{
    private readonly ApplicationDbContext _context;
    public RefreshTokenCleanupJob(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task ExecuteAsync()
    {
        var cutoff = DateTime.UtcNow;

        var staletoken = await _context.RefreshTokens
                        .Where(t => t.IsRevoked || t.ExpiresAt < cutoff)
                        .ToListAsync();

        _context.RefreshTokens.RemoveRange(staletoken);
        await _context.SaveChangesAsync();
    }
}