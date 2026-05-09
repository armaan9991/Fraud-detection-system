namespace FraudDetection.API.Models;

public class FraudAlert
{
    public int FraudAlertId {get;set;}
    public int TransactionId {get;set;}
    public string RiskLevel {get;set;} = string.Empty;
    public string Reason {get;set;} =string.Empty;
    public DateTime CreatedAt {get;set;} = DateTime.UtcNow;
    public Transaction? Transaction {get;set;}

}