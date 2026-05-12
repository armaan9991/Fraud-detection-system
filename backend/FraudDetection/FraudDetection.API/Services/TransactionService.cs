using FraudDetection.API.DTOs;
using FraudDetection.API.Data;
using FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.API.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;
    private readonly IFraudAlertService _IFraudAlertService;
    public TransactionService(ApplicationDbContext context , IFraudAlertService fraudalertservice)
    {
        _context = context;
        _IFraudAlertService = fraudalertservice;
    }

    public async Task<Transaction?> CreateTransactionAsync(CreateTransactionDtos dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u=>u.UserId ==dto.UserId);
        
        if (user == null)
        {
            return null;
        }

        int fraud_score = CalculateFraudScore(dto);

        string risk_level = GetRiskLevel(fraud_score);

        var transaction = new Transaction
        {
            UserId = dto.UserId,
            Amount = dto.Amount,
            Currency = dto.Currency,
            Country = dto.Country,
            FraudScore = fraud_score ,
            Status = risk_level
        };

        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
        
        if (fraud_score >= 60)
        {
           await _IFraudAlertService.CreateAutomaticAlertAsync(transaction.TransactionId , risk_level, "Suspicious transaction!!");
        }
        return transaction;
    }

    public async Task<List<Transaction>> GetAllTransactionsAsync()
    {
        return await _context.Transactions.Include(u=>u.User).ToListAsync();
    }

    public async Task<Transaction?> GetTransactionByIdAsync(int id)
    {
        return  await _context.Transactions.Include(u=>u.User).FirstOrDefaultAsync(t=>t.TransactionId == id);
    }
    private TransactionResponseDto MapToTransactionResponseDto(Transaction transaction)
    {
        return new TransactionResponseDto
        {
            TransactionId = transaction.TransactionId,
            UserId = transaction.UserId,
            Amount = transaction.Amount,
            Currency = transaction.Currency,
            Country = transaction.Country,
            FraudScore = transaction.FraudScore,
            Status = transaction.Status,
            TransactionTime = transaction.TransactionTime
        };
    }

    public int CalculateFraudScore(CreateTransactionDtos dto)
    {
        int f_score = 0 ;
        if (dto.Amount > 5000)
        {
            f_score += 40;
        }
        if (dto.Country != "canada")
        {
            f_score += 25;
        }
        int Hour = DateTime.UtcNow.Hour;
    
        if (Hour >=0 && Hour <=5)
        {
            f_score += 15;
        }

        return f_score;
    }
    public string GetRiskLevel(int f_score)
    {
        if (f_score >= 60)
        {
            return "HIGH";
        }
        else if(f_score >= 30)
        {
            return "MEDIUM";
        }
        else
        {
            return"LOW";
        }
    }
}
