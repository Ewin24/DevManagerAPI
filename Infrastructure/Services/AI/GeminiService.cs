using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.AI;

/// <summary>
/// Implementación del servicio de Google Gemini AI
/// </summary>
public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeminiService> _logger;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly string _endpoint;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public GeminiService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GeminiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["GoogleAI:ApiKey"] 
            ?? throw new InvalidOperationException("GoogleAI:ApiKey no configurado");
        _model = configuration["GoogleAI:Model"] ?? "gemini-1.5-flash";
        _endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent";
    }

    public async Task<string> QueryAsync(string prompt, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Consultando Gemini con prompt de {Length} caracteres", prompt.Length);

            var request = new GeminiRequest
            {
                Contents = new[]
                {
                    new Content
                    {
                        Parts = new[] { new Part { Text = prompt } }
                    }
                },
                GenerationConfig = new GenerationConfig
                {
                    Temperature = 0.7,
                    MaxOutputTokens = 8192,
                    TopP = 0.95
                }
            };

            var response = await SendRequestAsync(request, cancellationToken);
            var text = ExtractTextFromResponse(response);

            _logger.LogInformation("Respuesta de Gemini: {TokenCount} tokens", 
                response.UsageMetadata?.TotalTokenCount ?? 0);

            return text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar Gemini");
            throw;
        }
    }

    public async Task<(string Response, string Reasoning)> QueryWithReasoningAsync(
        string prompt, 
        CancellationToken cancellationToken = default)
    {
        var enhancedPrompt = $@"
Analiza la siguiente solicitud usando Chain of Thought reasoning.
Proporciona tu respuesta en formato JSON estricto:
{{
    ""reasoning"": ""Paso a paso de tu razonamiento"",
    ""response"": ""Tu respuesta final""
}}

Solicitud: {prompt}

Responde SOLO con el JSON, sin markdown ni texto adicional.";

        var result = await QueryAsync(enhancedPrompt, cancellationToken);
        
        try
        {
            // Limpiar posibles markdown code blocks
            var cleanJson = result.Trim().Replace("```json", "").Replace("```", "").Trim();
            
            using var doc = JsonDocument.Parse(cleanJson);
            var reasoning = doc.RootElement.GetProperty("reasoning").GetString() ?? "";
            var response = doc.RootElement.GetProperty("response").GetString() ?? "";
            
            return (response, reasoning);
        }
        catch (JsonException)
        {
            _logger.LogWarning("No se pudo parsear respuesta estructurada, retornando texto completo");
            return (result, "No se pudo extraer el razonamiento estructurado");
        }
    }

    public async Task<T> AnalyzeStructuredDataAsync<T>(
        string prompt, 
        object inputData, 
        CancellationToken cancellationToken = default)
    {
        var jsonData = JsonSerializer.Serialize(inputData, JsonOptions);
        var enhancedPrompt = $@"
Analiza los siguientes datos y {prompt}

Datos de entrada:
{jsonData}

Responde SOLO con JSON válido que pueda parsearse directamente, sin markdown ni explicaciones adicionales.";

        var result = await QueryAsync(enhancedPrompt, cancellationToken);
        
        // Limpiar markdown code blocks
        var cleanJson = result.Trim().Replace("```json", "").Replace("```", "").Trim();
        
        return JsonSerializer.Deserialize<T>(cleanJson, JsonOptions) 
            ?? throw new InvalidOperationException("No se pudo deserializar la respuesta de Gemini");
    }

    private async Task<GeminiResponse> SendRequestAsync(
        GeminiRequest request, 
        CancellationToken cancellationToken)
    {
        var requestUrl = $"{_endpoint}?key={_apiKey}";
        
        var json = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(requestUrl, content, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Error de API Gemini ({StatusCode}): {Error}", 
                response.StatusCode, error);
            throw new HttpRequestException(
                $"Error de API Gemini ({response.StatusCode}): {error}");
        }

        var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>(
            JsonOptions, cancellationToken);

        return geminiResponse 
            ?? throw new InvalidOperationException("Respuesta vacía de Gemini");
    }

    private static string ExtractTextFromResponse(GeminiResponse response)
    {
        if (response.Candidates == null || response.Candidates.Length == 0)
            return string.Empty;

        var candidate = response.Candidates[0];
        var parts = candidate.Content.Parts;

        if (parts == null || parts.Length == 0)
            return string.Empty;

        return parts[0].Text ?? string.Empty;
    }
}
