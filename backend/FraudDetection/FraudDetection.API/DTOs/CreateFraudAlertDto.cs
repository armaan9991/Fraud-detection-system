namespace FraudDetection.API.DTOs;

public class CreateFraudAlertDto
{
        public int TransactionId {get;set;}
        public string RiskLevel {get;set;} = string.Empty;
        public string Reason {get;set;} =string.Empty;
}