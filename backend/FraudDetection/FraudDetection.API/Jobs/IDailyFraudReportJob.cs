namespace FraudDetection.API.Jobs;

public interface IDailyFraudReportJob
{
    Task ExecuteAsync();
}