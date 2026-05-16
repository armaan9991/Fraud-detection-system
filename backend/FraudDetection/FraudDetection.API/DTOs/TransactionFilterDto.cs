namespace FraudDetection.API.DTOs;

public class TransactionFilterDto
{
    public decimal? MinAmount {get; set;}
    public decimal? MaxAmount {get; set;}

    public DateTime? FromDate {get; set;}
    public DateTime? ToDate {get; set;}
    
    public string? Currency {get; set;} = string.Empty;
    public string? Country { get;set;} =  string.Empty;
    public int? MinFraudScore {get;set;} 
    public int? MaxFraudScore {get;set;} 
    
    public string? Status {get; set;} = string.Empty;

}