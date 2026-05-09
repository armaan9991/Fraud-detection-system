using System.Transactions;

namespace FraudDetection.API.Models;

public class User
{
    public int UserId {get; set ;}
    public string Name {get; set;}
    public string Email {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public List<Transaction> Transactions {get; set;} = new();

}