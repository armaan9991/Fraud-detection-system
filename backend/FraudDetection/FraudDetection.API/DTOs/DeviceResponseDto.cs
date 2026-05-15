namespace FraudAlert.API.DTOs;

public class DeviceResponseDto
{
    public int DeviceId {get;set;}
    public int UserId {get;set;}
    public string DeviceName{get;set;} = string.Empty;
    public string IPAddress {get;set;} =string.Empty;
    public DateTime LastUsed {get;set;}= DateTime.UtcNow;
}