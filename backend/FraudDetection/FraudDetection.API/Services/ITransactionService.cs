using FraudDetection.API.Models;
using FraudDetection.API.DTOs;
namespace FraudDetection.API.Services;

public interface ITransactionService
{
    Task<TransactionResponseDto?> CreateTransactionAsync(int userId, CreateTransactionDtos dto);
    Task<List<TransactionResponseDto>> GetAllTransactionsAsync();
    Task<TransactionResponseDto?> GetTransactionByIdAsync(int id);
    Task<List<TransactionMLDto>> GetMLTrainingDataAsync();
    Task<int> SeedTransactionAsync();
}