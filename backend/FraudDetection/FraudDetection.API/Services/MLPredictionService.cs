using FraudDetection.API.DTOs;
using  System.Text.Json;

namespace FraudDetection.API.Services;
public class MLPredictionService : IMLPredictionService
{
    private readonly HttpClient _httpClient;
    public MLPredictionService(HttpClient httpClient){
        _httpClient =httpClient;
    }

    public async Task<MLPredictionResponseDto?> PredictFraudAsync(MLPredictionRequestDto dto)
    {
        var json = JsonSerializer.Serialize(dto);

        var content = new StringContent(json,System.Text.Encoding.UTF8,"application/json");

        var response = await _httpClient.PostAsync("http://127.0.0.1:8000/predict",content);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MLPredictionResponseDto>(responseContent , new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive =true
        });
    }
}