using System.ComponentModel.DataAnnotations;

namespace FraudDetection.API.DTOs;

public class CreateFraudAlertDto
{
        [Required]
        public int TransactionId {get;set;}
         [Required]
        [StringLength(20)]
        public string RiskLevel {get;set;} = string.Empty;
         [Required]
        [StringLength(200)]
        public string Reason {get;set;} =string.Empty;
}