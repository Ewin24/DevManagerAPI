namespace Domain.Entities.Agent;

/// <summary>
/// Representa una herramienta disponible para el agente (patrón MCP Tool Use)
/// </summary>
public class AgentTool
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Schema { get; set; } // JSON Schema de los parámetros
    public required Func<Dictionary<string, object>, Task<object>> Handler { get; set; }
}
