namespace FraudDetection.API.DTOs;

public class CreateTranscationDtos
{
    public int UserId {get;set;}
    public decimal Amount {get;set;}
    public string Currency {get;set;}="CAD";
    public string Country{get;set;}=string.Empty;
}