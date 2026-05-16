using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FraudDetection.API.DTOs;

public class CreateUserDto
{

    [Required]
    [StringLength(100)]
    public string Name {get; set;} = string.Empty;
    [Required]
    [EmailAddress]
    public string Email {get; set;} = string.Empty;
    [Required]
    [MinLength(6)]
    public string Password{get;set;}=string.Empty;
}