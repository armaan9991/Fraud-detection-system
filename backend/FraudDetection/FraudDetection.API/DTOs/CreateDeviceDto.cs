using System.ComponentModel.DataAnnotations;

namespace FraudDetection.API.DTOs;

public class CreateDeviceDto
{
    [Required]
    [Range(1001,100000)]
    public int UserId {get;set;}
    [Required]
    [StringLength(100)]
    public string DeviceName {get;set;}=string.Empty;
    [Required]
    [StringLength(100)]
    public string IPAddress{get;set;}=string.Empty;
}