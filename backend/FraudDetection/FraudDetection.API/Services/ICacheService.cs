namespace FraudDetection.API.Services;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task setAsync<T>(string key, T value,TimeSpan expiry);
    Task RemoveAsync(string key);
}