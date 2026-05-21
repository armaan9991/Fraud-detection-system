using FraudDetection.API.Data;
using FraudDetection.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.API.Services;

public class AdminService : IAdminService
{
    private readonly IAuditLogService _auditloggingservice;
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheservice;
    public AdminService(ApplicationDbContext context, IAuditLogService auditLogService, ICacheService cacheService)
    {
        _context = context;
        _auditloggingservice = auditLogService;
        _cacheservice = cacheService;
    }

    public async Task<AdminStatsDto> GetStatsAsync()
    {
        const string cacheKey = "admin_stats";

        var cached = await _cacheservice.GetAsync<AdminStatsDto>(cacheKey);

        if(cached != null) return cached;

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

        var stats= new AdminStatsDto
        {
            TotalUsers = totalUsers,
            TotalTranction = totalTransactions,
            TotalFraudAlerts = totalAlerts,
            TotalTransactionAmount = totalAmount,
            AverageFraudScore = avgScore,
            TransactionsByStatus = byStatus,
            AlertRiskLevel = byRisk
        };

        await _cacheservice.SetAsync(cacheKey,stats,TimeSpan.FromMinutes(5));
        return stats;
    }

     public async Task<PagedResult<UserResponseDto>> GetUsersAsync(int page,int pageSize, UserFilterDto filterDto)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(filterDto.Role))
        {
            query = query.Where(t => t.Role == filterDto.Role.ToLower());
        }
        if(!string.IsNullOrEmpty(filterDto.Email))
        {
        query = query.Where(t => t.Email.Contains(filterDto.Email.ToLower()));
        }
        if(!string.IsNullOrEmpty(filterDto.Name))
        {
            query.Where(t => t.Name.Contains(filterDto.Name.ToLower()));
        }
        if(filterDto.FromData.HasValue)
        {
            query.Where(t => t.CreatedAt >= filterDto.FromData);
        }
        if (filterDto.ToDate.HasValue)
        {
            query.Where( t=> t.CreatedAt <= filterDto.ToDate);
        }
        if (filterDto.IsFlagged == true)
        {
            query.Where( t=> t.IsFlagged == filterDto.IsFlagged);

        }
        var totalRecords = await query.CountAsync();

        var users = await query
                    .OrderByDescending( t=> t.UserId)
                    .Skip((page-1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
        return new PagedResult<UserResponseDto>
        {
            Page = page,
            PageSize =pageSize,
            TotalPages = (int)Math.Ceiling( totalRecords/(double)pageSize),
            TotalRecords = (int)totalRecords,
            Items = users.Select( u => new UserResponseDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt,
                IsFlagged = u.IsFlagged
            }).ToList()
        };
    }

    public async Task<PagedResult<TransactionResponseDto>> GetUserTransactionAsync(int userId, int page,int pageSize)
    {
        var query = _context.Transactions.Where(t=> t.UserId == userId);

        var totalrecords = await query.CountAsync();

        var transactions =await query  
                            .OrderByDescending( t => t.TransactionTime)
                            .Skip((page-1)*pageSize)
                            .Take(pageSize)
                            .ToListAsync();
        
        return new PagedResult<TransactionResponseDto>
        {
            TotalRecords =totalrecords,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalrecords / pageSize),
            Items = transactions.Select(t => new TransactionResponseDto
            {
                TransactionId = t.TransactionId,
                UserId = t.UserId,
                Amount = t.Amount,
                TransactionTime = t.TransactionTime,
                Currency = t.Currency,
                Country = t.Country,
                FraudScore =t.FraudScore,
                Status = t.Status
            }).ToList()
        };
    }


    public async Task<TransactionResponseDto?> UpdateTransactionStatusAsync(int transactionId,string status)
    {
        var transaction =await _context.Transactions.FindAsync(transactionId);
        if (transaction == null) return null;

        transaction.Status = status.ToUpper();

        await _context.SaveChangesAsync();

        await _auditloggingservice.CreateLogAsync(null,"updated transaction staus", "transaction", transaction.TransactionId, "status updated by user!");
       
       await _cacheservice.RemoveAsync("admin_stats");
        return new TransactionResponseDto
        {
            TransactionId =transaction.TransactionId,
            UserId = transaction.UserId,
            Amount = transaction.Amount,
            TransactionTime = transaction.TransactionTime,
            Currency = transaction.Currency,
            Country = transaction.Country,
            FraudScore = transaction.FraudScore,
            Status = transaction.Status
        };
    }

    public async Task<UserResponseDto?> UnflagUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        user.IsFlagged =false;
        user.FlagReason = null;

        await _context.SaveChangesAsync();


        return new UserResponseDto
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            IsFlagged = user.IsFlagged,
            FlagReason = user.FlagReason
        };
    }

    
}