namespace FraudDetection.API.Models;
 
public class Transaction
{
    public int TransactionId {get; set;}
    public int UserId {get; set;}
    public decimal Amount {get; set;}
    public DateTime TransactionTime {get; set;} = DateTime.UtcNow;
    public string Currency {get; set;} = "CAD";
    public string Country { get;set;}
    public int FraudScore {get;set;} = 0;
    public string Status {get; set;} = "PENDING";

    public  User? user  {get;set;}
    public List<FraudAlert> FraudAlerts {get; set;} =new();
}