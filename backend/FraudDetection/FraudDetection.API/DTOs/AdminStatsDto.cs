namespace FraudDetection.API.DTOs;

public class AdminStatsDto
{
    public int TotalUsers {get;set;}
    public int TotalTranction{get;set;}
    public int TotalFraudAlerts {get;set;}
    public decimal TotalTransactionAmount{get;set;}
    public double AverageFraudScore {get;set;}
    public Dictionary<string,int> TransactionsByStatus {get;set;} = new();
    public Dictionary<string,int> AlertRiskLevel {get;set;} = new();
}