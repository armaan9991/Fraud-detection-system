using FraudDetection.API.Data;
using FraudDetection.API.DTOs;
using FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using FraudDetection.API.Services;


namespace FraudDetection.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController: ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        
        _transactionService = transactionService;
    }

// post   api/transactions
    [HttpPost]
    public async Task<IActionResult> CreateTransaction(CreateTransactionDtos dto)
    {
        var user = await _transactionService.CreateTransactionAsync(dto);
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
            FraudScore = 0 ,
            Status = "PENDING"
        };
        return CreatedAtAction(
            nameof(GetTransactionById),
            new { id = transaction.TransactionId}, transaction
        );
    }


    // get api/transaction

    [HttpGet]
    public async Task<IActionResult> GetTransactions()
    {
        var transactions =await _transactionService.GetAllTransactionsAsync();

        return Ok(transactions);
    }


   [HttpGet("{id}")] 
   public async Task<IActionResult> GetTransactionById(int id)
    {
        var transaction = await _transactionService.GetTransactionByIdAsync(id);
        if (transaction == null)
        {
            return NotFound();
        }
        return Ok(transaction);
    }

}