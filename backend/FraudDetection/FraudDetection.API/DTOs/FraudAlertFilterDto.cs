namespace FraudDetection.API.DTOs;

public class FraudAlertFilterDto
{
    public string? RiskLevel {get;set;} = string.Empty;
    public string? Reason {get;set;} =string.Empty;
    public DateTime? FromCreated {get;set;} 
    public DateTime? ToCreated {get;set;} 
}