using FraudDetection.API.Models;
using FraudDetection.API.DTOs;
namespace FraudDetection.API.Services;

public interface ITransactionService
{
    Task<TransactionResponseDto?> CreateTransactionAsync(int userId, CreateTransactionDto dto);
    Task<PagedResult<TransactionResponseDto>> GetAllTransactionsAsync(int userId, string role , int page, int pageSize);
    Task<TransactionResponseDto?> GetTransactionByIdAsync(int id);
    Task<List<TransactionMLDto>> GetMLTrainingDataAsync();
    Task<int> SeedTransactionAsync();
}