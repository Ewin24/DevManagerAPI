namespace Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de Google Gemini AI
/// </summary>
public interface IGeminiService
{
    /// <summary>
    /// Consulta directa al modelo Gemini
    /// </summary>
    Task<string> QueryAsync(string prompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consulta con razonamiento estructurado (Chain of Thought)
    /// </summary>
    Task<(string Response, string Reasoning)> QueryWithReasoningAsync(
        string prompt, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Análisis de datos estructurados (JSON in/out)
    /// </summary>
    Task<T> AnalyzeStructuredDataAsync<T>(
        string prompt, 
        object inputData, 
        CancellationToken cancellationToken = default);
}
