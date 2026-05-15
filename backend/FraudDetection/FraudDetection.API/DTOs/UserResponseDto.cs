namespace FraudDetection.API.DTOs;

public class UserResponseDto
{
    public int UserId {get; set ;}
    public string Name {get; set;}
    public string Email {get; set;}
    public string Role{get;set;}="User";
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
}