using System.ComponentModel.DataAnnotations;

namespace FraudDetection.API.DTOs;

public class CreateUserDtos
{

    [Required]
    [StringLength(100)]
    public string Name {get; set;} = string.Empty;
    [Required]
    [EmailAddress]
    public string Email {get; set;} = string.Empty;
}