using System.Transactions;

namespace FraudDetection.API.Models;

public class User
{
    public int UserId {get; set ;}
    public string Name {get; set;}
    public string Email {get; set;}
    public string PasswordHash {get;set;} = string.Empty;
    public string Role{get;set;}="User";
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public List<Transaction> Transactions {get; set;} = new();

}