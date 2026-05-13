using FraudDetection.API.DTOs;

namespace FraudDetection.API.Services;
public interface IMLPredictionService
{
    Task<MLPredictionResponseDto?> PredictFraudAsync(MLPredictionRequestDto dto);
}