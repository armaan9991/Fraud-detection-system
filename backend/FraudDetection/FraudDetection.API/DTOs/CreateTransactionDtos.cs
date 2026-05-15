using System.ComponentModel.DataAnnotations;

namespace FraudDetection.API.DTOs;

public class CreateTransactionDtos
{
    
    [Required]
    [Range(1, 1000000)]
    public decimal Amount {get;set;}
    [Required]
    [StringLength(10)]
    public string Currency {get;set;}="CAD";
    [Required]
    [StringLength(50)]
    public string Country{get;set;}=string.Empty;
}