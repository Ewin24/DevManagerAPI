namespace Infrastructure.Services.AI;

/// <summary>
/// Modelos de request/response para Google Gemini API
/// </summary>
internal class GeminiRequest
{
    public required Content[] Contents { get; set; }
    public GenerationConfig? GenerationConfig { get; set; }
}

internal class Content
{
    public required Part[] Parts { get; set; }
    public string? Role { get; set; } // "user" o "model"
}

internal class Part
{
    public string? Text { get; set; }
}

internal class GenerationConfig
{
    public double? Temperature { get; set; }
    public int? MaxOutputTokens { get; set; }
    public double? TopP { get; set; }
    public int? TopK { get; set; }
}

internal class GeminiResponse
{
    public required Candidate[] Candidates { get; set; }
    public UsageMetadata? UsageMetadata { get; set; }
}

internal class Candidate
{
    public required Content Content { get; set; }
    public string? FinishReason { get; set; }
    public int Index { get; set; }
}

internal class UsageMetadata
{
    public int PromptTokenCount { get; set; }
    public int CandidatesTokenCount { get; set; }
    public int TotalTokenCount { get; set; }
}
