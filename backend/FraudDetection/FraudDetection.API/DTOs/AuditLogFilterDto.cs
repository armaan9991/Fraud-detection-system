namespace FraudDetection.API.DTOs;

public class AuditLogFilterDto
{
    public int? UserId {get;set;}
    public string? Action {get;set;}
    public string? EntityType {get;set;}
    public int? EntityId {get;set;}
    public DateTime fromCreatedAt{get;set;}   
    public DateTime toCreatedAt{get;set;}   

}