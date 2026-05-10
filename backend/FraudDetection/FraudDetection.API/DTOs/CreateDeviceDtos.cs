namespace FraudDetection.API.DTOs;

public class CreateDeviceDtos
{
    public int UserId {get;set;}
    public string DeviceName {get;set;}=string.Empty;
    public string IPAddress{get;set;}=string.Empty;
}