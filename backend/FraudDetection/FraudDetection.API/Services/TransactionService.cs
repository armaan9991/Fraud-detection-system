using FraudDetection.API.DTOs;
using FraudDetection.API.Data;
using FraudDetection.API.Models;
using Microsoft.EntityFrameworkCore;
// using Microsoft.Net;

namespace FraudDetection.API.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;
    private readonly IFraudAlertService _IFraudAlertService;
    private readonly IMLPredictionService _mLPredictionService;
    private record FraudScoreResult(int Score,List<string> reason);
    private readonly IAuditLogService _auditLogService;
    private readonly IEmailService _emailService;
    
    public TransactionService(ApplicationDbContext context , IFraudAlertService fraudalertservice, IMLPredictionService mLPredictionService,IAuditLogService auditLogService, IEmailService emailService)
    {
        _context = context;
        _IFraudAlertService = fraudalertservice;
        _mLPredictionService = mLPredictionService;
        _auditLogService = auditLogService;
        _emailService = emailService;
    }

    public async Task<TransactionResponseDto?> CreateTransactionAsync(int UserId, CreateTransactionDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u=>u.UserId == UserId);
        
        if (user == null)
        {
            return null;
        }
        
        int fraud_score;
        List<string> Reasons ;
        (fraud_score,Reasons) = CalculateFraudScore(dto);

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
           IsForeignTransaction = transaction.Country.ToLower() != "canada" ? 1 : 0,
           IsNightTransaction = DateTime.UtcNow.Hour < 6 ? 1 : 0,
           Hour = DateTime.UtcNow.Hour,
           IsNonCadCurrency = transaction.Currency.ToUpper() != "CAD" ? 1 : 0

        };

        var prediction = await _mLPredictionService.PredictFraudAsync(mlrequest);
        
        if (prediction != null)
        {
            transaction.FraudScore = (int)(prediction.FraudProbability * 100);
            transaction.Status = prediction.Prediction == 1 ? "HIGH" : GetRiskLevel(fraud_score);
        }

         _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        
        await _auditLogService.CreateLogAsync(transaction.UserId,"Created Transaction", "transaction", transaction.TransactionId, $"Transaction of {transaction.Amount} created!");
        if (transaction.FraudScore >= 60)
        {
           await _IFraudAlertService.CreateAutomaticAlertAsync(transaction.TransactionId , transaction.Status, Reasons.Count>0? string.Join(";", Reasons) :"suspicious reasons");
            var emailbody =$"Fraud Alert \n hello {user.Name} \n a suspicious transacion is made \n  Amount:{transaction.Amount}\n Currency : {transaction.Currency} \n Country: {transaction.Country} \nRisk Level:{transaction.Status}\n Fraud Score: {transaction.FraudScore} \n Time {transaction.TransactionTime:yyyy-MM-dd HH:mm} UTC \n";
            //  try{
            //     // await _emailService.SendEmailAsync(user.Email,"Fraud Alert",emailbody);
            //     _ = Task.Run(() => _emailService.SendEmailAsync(user.Email,"Fraud Alert",emailbody));
            //  }
            //  catch (Exception e)
            //     {
            //         Console.Write(e+" failed to send email.");
            //        await _auditLogService.CreateLogAsync(user.UserId,"Failed email","User",user.UserId,"Transation Saved but failed to send email");
            //     }
            try
            {
                 await _emailService.SendEmailAsync(user.Email,"Fraud Alert",emailbody);
            }
            catch(Exception e)
            {
                Console.Write(e+" failed to send email.");
                await _auditLogService.CreateLogAsync(user.UserId,"Failed email","User",user.UserId,"Transation Saved but failed to send email");
            }

        }
        return MapToTransactionResponseDto(transaction);
    }

    public async Task<PagedResult<TransactionResponseDto>> GetAllTransactionsAsync(int userId , string role ,int page, int pageSize,TransactionFilterDto filterDto)
    {
        var query = _context.Transactions.Include(t => t.User).AsQueryable();
        if (role != "admin")
        {
            query = query.Where(t => t.UserId == userId);
        }
        if (!string.IsNullOrEmpty(filterDto.Status))
        {
            query = query.Where(t => t.Status == filterDto.Status);
        }
        if (!string.IsNullOrEmpty(filterDto.Country))
        {
            query = query.Where(t => t.Country == filterDto.Country);
        }
        if (!string.IsNullOrEmpty(filterDto.Currency))
        {
            query = query.Where(t => t.Currency == filterDto.Currency);
        }
        if (filterDto.MinAmount.HasValue)
        {
            query = query.Where(t => t.Amount >= filterDto.MinAmount.Value);
        }
        if (filterDto.MaxAmount.HasValue)
        {
            query = query.Where(t => t.Amount <= filterDto.MaxAmount.Value);
        }
        if (filterDto.FromDate.HasValue)
        {
            query = query.Where(t => t.TransactionTime >= filterDto.FromDate.Value);
        }
        if (filterDto.ToDate.HasValue)
        {
            query = query.Where(t => t.TransactionTime <= filterDto.ToDate.Value);
        }
        if (filterDto.MinFraudScore.HasValue)
        {
            query = query.Where(t => t.FraudScore >= filterDto.MinFraudScore.Value);
        }
        
        if (filterDto.MaxFraudScore.HasValue)
        {
            query = query.Where(t => t.FraudScore <= filterDto.MaxFraudScore.Value);
        }
       

        var totalrecords = await query.CountAsync();

        var items = await query
                    .OrderByDescending(t => t.TransactionTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
       
        return new PagedResult<TransactionResponseDto>
        {
            Items = items.Select( t=> MapToTransactionResponseDto(t)).ToList(),
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

     
    private FraudScoreResult CalculateFraudScore(CreateTransactionDto dto)
    {
        int f_score = 0 ;
        var reason = new List<string>();
        if (dto.Amount > 5000)
        {
            f_score += 40;
            reason.Add($"High amount ({dto.Amount})");
        }
        if (dto.Country != "canada")
        {
            f_score += 25;
            reason.Add($"Foriegn Country ({dto.Country})");
        }
        if(dto.Currency.ToLower() != "cad")
        {
            f_score +=10;
            reason.Add($"Non canadian currency ({dto.Currency})");

        }
        int Hour = DateTime.UtcNow.Hour;
    
        if (Hour >=0 && Hour <=5)
        {
            f_score += 15;
            reason.Add($"Overnight transaction ({Hour})");

        }

        return new FraudScoreResult(f_score,reason);
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
