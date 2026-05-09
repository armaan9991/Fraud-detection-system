using FraudDetection.API.DTOs;
using FraudDetection.API.Data;
using FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.API.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;
    public TransactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction>? CreateTransactionAsync(CreateTransactionDtos dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u=>u.UserId ==dto.UserId);
        
        if (user == null)
        {
            return null;
        }
        var transaction = new Transaction
        {
            UserId = dto.UserId,
            Amount = dto.Amount,
            Currency = dto.Currency,
            Country = dto.Country,
            FraudScore = 0 ,
            Status = "PENDING"
        };
        _context.SaveChangesAsync();
        
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
}