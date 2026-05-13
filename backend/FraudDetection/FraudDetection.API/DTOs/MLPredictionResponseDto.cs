namespace FraudDetection.API.DTOs;

public class MLPredictionResponseDto
{
    public int Prediction {get;set;}
    public double FraudProbality{get;set;}
}