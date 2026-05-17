namespace FraudDetection.API.Models;

public class RefreshToken
{
    public int Id{get;set;}
    public int UserId{get;set;}
    public string Token{get;set;} = string.Empty;
    public DateTime ExpiresAt{get;set;} = DateTime.UtcNow;
    public bool IsRevoked {get;set;} =false;

    public User user{get;set;}  = null;
}