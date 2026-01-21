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
            // Limpiar posibles markdown code blocks y saltos de línea problemáticos
            var cleanJson = result.Trim()
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();
            
            // Intentar encontrar el JSON si está embebido en texto
            var jsonStart = cleanJson.IndexOf('{');
            var jsonEnd = cleanJson.LastIndexOf('}');
            
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                cleanJson = cleanJson.Substring(jsonStart, jsonEnd - jsonStart + 1);
            }
            
            _logger.LogDebug("JSON limpio a parsear: {Json}", cleanJson);
            
            using var doc = JsonDocument.Parse(cleanJson);
            var root = doc.RootElement;
            
            // Intentar obtener reasoning (puede ser string u objeto)
            string reasoning = "";
            if (root.TryGetProperty("reasoning", out var reasoningElement))
            {
                reasoning = reasoningElement.ValueKind == JsonValueKind.String 
                    ? reasoningElement.GetString() ?? ""
                    : reasoningElement.ToString();
            }
            
            // Intentar obtener response (puede ser string u objeto)
            string response = "";
            if (root.TryGetProperty("response", out var responseElement))
            {
                response = responseElement.ValueKind == JsonValueKind.String 
                    ? responseElement.GetString() ?? ""
                    : responseElement.ToString();
            }
            
            // Si alguno está vacío, usar el resultado completo
            if (string.IsNullOrWhiteSpace(response))
            {
                _logger.LogWarning("Response vacío en JSON, usando resultado completo");
                return (result, reasoning);
            }
            
            return (response, reasoning);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudo parsear respuesta estructurada, retornando texto completo");
            
            // Fallback: dividir el texto en dos partes si es posible
            var lines = result.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 1)
            {
                var reasoning = string.Join("\n", lines.Take(lines.Length / 2));
                var response = string.Join("\n", lines.Skip(lines.Length / 2));
                return (response, reasoning);
            }
            
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
