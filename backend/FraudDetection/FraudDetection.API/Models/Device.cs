namespace FraudDetection.API.Models;

public class Device
{
    public int DeviceId {get;set;}
    public int UserId {get;set;}
    public string DeviceName{get;set;} = string.Empty;
    public string IPAddress {get;set;} =string.Empty;
    public DateTime LastUsed {get;set;}= DateTime.UtcNow;

    public User? User {get;set;}
    public List<Transaction> Transactions{get;set;}= new();
}