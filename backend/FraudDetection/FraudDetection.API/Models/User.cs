using System.Transactions;

namespace FraudDetection.API.Models;

public class User
{
    public int UserId {get; set ;}
    public string Name {get; set;}
    public string Email {get; set;}
    public string PasswordHash {get;set;} = string.Empty;
    public string Role{get;set;}="user";
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public bool IsFlagged {get;set;}
    public string? FlagReason {get;set;}
    public List<Transaction> Transactions {get; set;} = new();

}