using FraudDetection.API.Data;
using FraudDetection.API.DTOs;
using FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;


namespace FraudDetection.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController: ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TransactionsController(ApplicationDbContext context)
    {
        _context = context;
    }

// post   api/transactions
    [HttpPost]
    public async Task<IActionResult> CreateTransaction(CreateTransactionDtos dto)
    {
         var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId);

         if(user == null)
        {
            return NotFound("User is not present!");
        }

        var transaction = new Transaction
        {
            UserId = dto.UserId,
            Amount = dto.Amount,
            Currency = dto.Currency,
            Country = dto.Country,
            FraudScore = 0,
            Status = "PENDING"
        };

        _context.Transactions.Add(transaction);

        await _context.SaveChangesAsync();
        
        return CreatedAtAction(
            nameof(GetTransactionById),
            new { id = transaction.TransactionId}, transaction
        );
    }


    // get api/transaction

    [HttpGet]
    public async Task<IActionResult> GetTransactions()
    {
        var transactions =await _context.Transactions.Include(t =>t.User).ToListAsync();

        return Ok(transactions);
    }


   [HttpGet("{id}")] 
   public async Task<IActionResult> GetTransactionById(int id)
    {
        var transaction = await _context.Transactions.Include(t=>t.User).FirstOrDefaultAsync(t=>t.TransactionId == id);

        if (transaction == null)
        {
            return NotFound();
        }
        return Ok(transaction);
    }

}