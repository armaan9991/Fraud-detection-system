using FraudDetection.API.Data;
using FraudDetection.API.DTOs;
using FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using FraudDetection.API.Services;
using  Microsoft.AspNetCore.Authorization;


namespace FraudDetection.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        var transaction_c = await _transactionService.CreateTransactionAsync(dto);
         if(transaction_c == null)
        {
            return NotFound("User is not present!");
        }

       
        return CreatedAtAction(
            nameof(GetTransactionById),
            new { id = transaction_c.TransactionId}, transaction_c
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

    [HttpGet("ml-data")]
    public async Task<IActionResult> GetMLTrainingData()
    {
        var data = await _transactionService.GetMLTrainingDataAsync();

        return Ok(data);
    }

    // to generate random data.
    [HttpPost("seed")]
    public async Task<IActionResult> SeedTransaction()
    {
        int count = await _transactionService.SeedTransactionAsync();

        return Ok($"{count}  transactions are created!");
    }

}