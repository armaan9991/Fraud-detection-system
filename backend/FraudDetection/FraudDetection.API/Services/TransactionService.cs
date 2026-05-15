using FraudDetection.API.DTOs;
using FraudDetection.API.Data;
using FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FraudDetection.API.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;
    private readonly IFraudAlertService _IFraudAlertService;
    private readonly IMLPredictionService _mLPredictionService;
    public TransactionService(ApplicationDbContext context , IFraudAlertService fraudalertservice, IMLPredictionService mLPredictionService)
    {
        _context = context;
        _IFraudAlertService = fraudalertservice;
        _mLPredictionService = mLPredictionService;
    }

    public async Task<TransactionResponseDto?> CreateTransactionAsync(int UserId, CreateTransactionDtos dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u=>u.UserId == UserId);
        
        if (user == null)
        {
            return null;
        }

        int fraud_score = CalculateFraudScore(dto);

        string risk_level = GetRiskLevel(fraud_score);

        var transaction = new Transaction
        {
            UserId = UserId,
            Amount = dto.Amount,
            Currency = dto.Currency,
            Country = dto.Country,
            FraudScore = fraud_score ,
            Status = risk_level
        };

        var mlrequest = new MLPredictionRequestDto
        {
        Amount = transaction.Amount,

        IsForeignTransaction = transaction.Country.ToLower() != "Canada"  ? 1 : 0,

        IsNightTransaction =  DateTime.UtcNow.Hour < 6 ? 1 : 0,

        FraudScore = transaction.FraudScore
        };

        var prediction = await _mLPredictionService.PredictFraudAsync(mlrequest);
        
        if (prediction != null)
        {
            transaction.FraudScore=(int)(prediction.FraudProbability *100);
            if (prediction.Prediction == 1)
            {
                transaction.Status = "HIGH_RISK";
            }
        }

         _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        
        if (transaction.FraudScore >= 60)
        {
           await _IFraudAlertService.CreateAutomaticAlertAsync(transaction.TransactionId , risk_level, "Suspicious transaction!!");
        }
        return MapToTransactionResponseDto(transaction);
    }

    public async Task<PagedResult<TransactionResponseDto>> GetAllTransactionsAsync(int userId , string role ,int page, int pageSize)
    {
        var query = _context.Transactions.Include(t => t.User).AsQueryable();

        if (role != "Admin"){
            query = query.Where(t => t.UserId == userId);
        }

        var totalrecords = await query.CountAsync();

        var items = await query.OrderByDescending(t => t.TransactionTime).Skip((page-1)*pageSize).Take(pageSize).Select(t=> MapToTransactionResponseDto(t)).ToListAsync();
       
        return new PagedResult<TransactionResponseDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalRecords = totalrecords,
            TotalPages = (int)Math.Ceiling(totalrecords/(double)pageSize)
        };
    }

    public async Task<TransactionResponseDto?> GetTransactionByIdAsync(int id)
    {
        var transaction =  await _context.Transactions.Include(u=>u.User).FirstOrDefaultAsync(t=>t.TransactionId == id);
        if (transaction == null){return null;}
        return MapToTransactionResponseDto(transaction);
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

    public async Task<List<TransactionMLDto>> GetMLTrainingDataAsync()
    {
        var found_transaction = await _context.Transactions.ToListAsync();
        return found_transaction.Select(t=>
        new TransactionMLDto
        {
            Amount = t.Amount,
            IsForeignTransaction = t.Country.ToLower() == "canada" ? 0 : 1,
            IsNightTransaction = (t.TransactionTime.Hour >=0 && t.TransactionTime.Hour <= 5) ? 1 :0,
            FraudScore = t.FraudScore,
            IsFraud = t.FraudScore >60 ? 1:0
        }).ToList();
    }

    // generate random data 
    public async Task<int> SeedTransactionAsync()
    {
        var random = new Random();

        var Countries = new[]
        {
            "canada",
            "russia",
            "uk",
            "usa",
            "china"
        };

        var transactions = new List<Transaction>();

        for (int i=0; i<100 ; i++)
        {
            decimal Amount = random.Next(100,20000);
            string country = Countries[random.Next(Countries.Length)];

            int fraud_score = 0;

            if (Amount > 5000)
        {
            fraud_score += 40;
        }
        if (country != "canada")
        {
            fraud_score += 25;
        }
        int Hour = random.Next(0,23);
    
        if (Hour >=0 && Hour <=5)
        {
            fraud_score += 15;
        }

         string riskLevel = fraud_score >= 60 ? "HIGH" : fraud_score >= 30 ? "MEDIUM": "LOW";

        var transaction = new Transaction
            {
                UserId = 2,
                Amount = Amount,
                Currency = "CAD",
                Country = country,
                FraudScore = fraud_score,
                Status = riskLevel
            };
            transactions.Add(transaction);
        }


        await _context.Transactions.AddRangeAsync(transactions);

        await _context.SaveChangesAsync();
        return transactions.Count;
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
        if(dto.Currency.ToLower() != "cad")
        {
            f_score +=10;
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
        else if(f_score >= 35)
        {
            return "MEDIUM";
        }
        else
        {
            return"LOW";
        }
    }
}
