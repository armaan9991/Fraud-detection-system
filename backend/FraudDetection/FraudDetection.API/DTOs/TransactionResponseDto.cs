namespace FraudDetection.API.DTOs;

public class TransactionResponseDto
{
     public int TransactionId {get; set;}
    public int UserId {get; set;}

    public decimal Amount {get; set;}
    public DateTime TransactionTime {get; set;}
    public string Currency {get; set;} = string.Empty;
    public string Country { get;set;} =  string.Empty;
    public int FraudScore {get;set;} 
    public string Status {get; set;} = string.Empty;

   
}