using FraudDetection.API.Data;
using FraudDetection.API.DTOs;
using FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using FraudDetection.API.Services;
using  Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateTransaction(CreateTransactionDtos dto)
    {
        var userIdclaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdclaim == null)
        {
            return Unauthorized(ApiResponse<string>.ErrorResponse("User Id is not present!"));
        }
        int userId = int.Parse(userIdclaim);
        var transaction_c = await _transactionService.CreateTransactionAsync(userId,dto);
         if(transaction_c == null)
        {
            return NotFound(ApiResponse<string>.ErrorResponse("User is not present!"));
        }

       
        return CreatedAtAction(
            nameof(GetTransactionById),
            new { id = transaction_c.TransactionId},ApiResponse<TransactionResponseDto>.SuccessResponse(transaction_c, "Transaction created successfull!")
        );
    }


    // get api/transaction
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetTransactions(int page =1,int pageSize = 20)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        
        var transactions =await _transactionService.GetAllTransactionsAsync(userId, role,page,pageSize);

        return Ok(ApiResponse<PagedResult<TransactionResponseDto>>.SuccessResponse(transactions,"All Transactions present!"));
    }

    [Authorize]
   [HttpGet("{id}")] 
   public async Task<IActionResult> GetTransactionById(int id)
    {
        var transaction = await _transactionService.GetTransactionByIdAsync(id);
        if (transaction == null)
        {
            return NotFound(ApiResponse<string>.ErrorResponse($"No transaction of {id} is present!"));
        }

        var role = User.FindFirst(ClaimTypes.Role)!.Value;
        int userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        if (role != "Admin" && transaction.UserId != userid)
        {
            return Forbid();
        }

        return Ok(ApiResponse<TransactionResponseDto>.SuccessResponse(transaction, "found !"));
    }

    [HttpGet("ml-data")]
    public async Task<IActionResult> GetMLTrainingData()
    {
        var data = await _transactionService.GetMLTrainingDataAsync();

        return Ok(ApiResponse<List<TransactionMLDto>>.SuccessResponse(data,"Training data"));
    }

    // to generate random data.
    [HttpPost("seed")]
    public async Task<IActionResult> SeedTransaction()
    {
        int count = await _transactionService.SeedTransactionAsync();

        return Ok($"{count}  transactions are created!");
    }

}