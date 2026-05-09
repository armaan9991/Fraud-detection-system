using FraudDetection.API.Models;
using FraudDetection.API.DTOs;
namespace FraudDetection.API.Services;

public interface ITransactionService
{
    Task<Transaction?> CreateTransactionAsync(CreateTransactionDtos dto);
    Task<Transaction?> GetAllTransactionAsync();
    Task<Transaction?> GetTransactionByIdAsync(int id);
}