namespace FraudDetection.API.DTOs;

public class TransactionMLDto
{
    public decimal Amount{get;set;}
    public int IsForeignTransaction{get;set;}
    public int IsNightTransaction{get;set;}
    public int FraudScore{get;set;}
    public int IsFraud{get;set;}
}