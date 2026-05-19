namespace FraudDetection.API.DTOs;

public class UserFilterDto
{
    public string? Name {get; set;}
    public string? Email {get; set;}
    public string? Role{get;set;}
    public DateTime? FromData {get;set;}
    public DateTime? ToDate{get;set;}
    public bool? IsFlagged {get;set;}
}