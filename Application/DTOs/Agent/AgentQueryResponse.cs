namespace Application.DTOs.Agent;

/// <summary>
/// Respuesta del agente
/// </summary>
public record AgentQueryResponse
{
    public required string Response { get; init; }
    public required string ReasoningSteps { get; init; } // Chain of Thought
    public List<ToolExecutionResult> ToolsExecuted { get; init; } = new();
    public bool RequiresHumanApproval { get; init; }
    public Guid? ActionId { get; init; } // Para tracking
}

public record ToolExecutionResult
{
    public required string ToolName { get; init; }
    public required object Input { get; init; }
    public required object Output { get; init; }
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}
