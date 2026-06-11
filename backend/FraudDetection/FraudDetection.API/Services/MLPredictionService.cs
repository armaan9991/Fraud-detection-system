using FraudDetection.API.DTOs;
using  System.Text.Json;

namespace FraudDetection.API.Services;
public class MLPredictionService : IMLPredictionService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public MLPredictionService(HttpClient httpClient,IConfiguration configuration){
        _httpClient =httpClient;
        _configuration = configuration;
    }

    public async Task<MLPredictionResponseDto?> PredictFraudAsync(MLPredictionRequestDto dto)
    {
    try{
       var json =JsonSerializer.Serialize( dto, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Console.WriteLine(json);

        var content = new StringContent(json,System.Text.Encoding.UTF8,"application/json");
        var mlUrl = _configuration["ML:BaseUrl"] ?? "http://127.0.0.1:8000";
        var response = await _httpClient.PostAsync($"{mlUrl}/predict", content);
        // var response = await _httpClient.PostAsync("http://127.0.0.1:8000/predict",content);

        // Console.WriteLine(json);


        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var responseContent = await response.Content.ReadAsStringAsync();
        var result= JsonSerializer.Deserialize<MLPredictionResponseDto>(responseContent , new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive =true
        });
        Console.WriteLine(result?.Prediction);
        Console.WriteLine(result?.FraudProbability);
        return result;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }
}