namespace FraudDetection.API.Jobs;

public interface IRefreshTokenCleanupJob
{
    Task ExecuteAsync();
}