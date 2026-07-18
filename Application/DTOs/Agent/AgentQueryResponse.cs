using System.Text.Json.Serialization;

namespace Application.DTOs.Agent;

public enum ResponseType
{
    Text,
    Table,
    List,
    Mixed,
    Error
}

public record AgentQueryResponse
{
    [JsonPropertyName("response_type")]
    public required string ResponseType { get; init; }

    [JsonPropertyName("summary")]
    public required string Summary { get; init; }

    [JsonPropertyName("markdown")]
    public required string Markdown { get; init; }

    [JsonPropertyName("payload")]
    public ResponsePayload? Payload { get; init; }

    [JsonPropertyName("metadata")]
    public ResponseMetadata Metadata { get; init; } = new();

    [JsonPropertyName("suggested_actions")]
    public List<SuggestedAction>? SuggestedActions { get; init; }
}

public record ResponsePayload
{
    [JsonPropertyName("text")]
    public string? Text { get; init; }

    [JsonPropertyName("table")]
    public TablePayload? Table { get; init; }

    [JsonPropertyName("list")]
    public ListPayload? List { get; init; }
}

public record TablePayload
{
    [JsonPropertyName("headers")]
    public required List<string> Headers { get; init; }

    [JsonPropertyName("rows")]
    public required List<List<string>> Rows { get; init; }
}

public record ListPayload
{
    [JsonPropertyName("items")]
    public required List<ListItem> Items { get; init; }
}

public record ListItem
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("label")]
    public required string Label { get; init; }

    [JsonPropertyName("value")]
    public string? Value { get; init; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; init; }
}

public record ResponseMetadata
{
    [JsonPropertyName("reasoning")]
    public string? Reasoning { get; init; }

    [JsonPropertyName("tools_executed")]
    public List<ToolExecutionResult>? ToolsExecuted { get; init; }

    [JsonPropertyName("requires_human_approval")]
    public bool RequiresHumanApproval { get; init; }

    [JsonPropertyName("action_id")]
    public Guid? ActionId { get; init; }
}

public record SuggestedAction
{
    [JsonPropertyName("label")]
    public required string Label { get; init; }

    [JsonPropertyName("query")]
    public required string Query { get; init; }

    [JsonPropertyName("icon")]
    public string? Icon { get; init; }
}

public record ToolExecutionResult
{
    [JsonPropertyName("tool_name")]
    public required string ToolName { get; init; }

    [JsonPropertyName("input")]
    public required object Input { get; init; }

    [JsonPropertyName("output")]
    public required object Output { get; init; }

    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; init; }
}
