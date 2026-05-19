namespace FraudDetection.API.Jobs;

public interface IHighRiskUserDetectionJob
{
    Task ExecuteAsync();
}