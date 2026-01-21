using Domain.Entities.Agent;

namespace Application.Services.Agent;

/// <summary>
/// Definición de herramientas MCP (Model Context Protocol) para el agente
/// </summary>
public static class MCPTools
{
    /// <summary>
    /// Obtiene todas las herramientas disponibles para el agente
    /// </summary>
    public static List<AgentTool> GetAvailableTools()
    {
        return new List<AgentTool>
        {
            GetEmployeeProfileTool(),
            GetProjectRequirementsTool(),
            GetSkillsTool(),
            GetCertificationsTool(),
            GetProjectHistoryTool()
        };
    }

    private static AgentTool GetEmployeeProfileTool()
    {
        return new AgentTool
        {
            Name = "get_employee_profile",
            Description = "Obtiene el perfil completo de un empleado con sus habilidades y certificaciones",
            Schema = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""userId"": { ""type"": ""string"", ""format"": ""uuid"" }
                },
                ""required"": [""userId""]
            }",
            Handler = async (parameters) =>
            {
                // Este handler se inyectará dinámicamente con los servicios reales
                var userId = Guid.Parse(parameters["userId"].ToString()!);
                return new { userId, message = "Profile tool executed" };
            }
        };
    }

    private static AgentTool GetProjectRequirementsTool()
    {
        return new AgentTool
        {
            Name = "get_project_requirements",
            Description = "Obtiene los requisitos de habilidades de un proyecto específico",
            Schema = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""projectId"": { ""type"": ""string"", ""format"": ""uuid"" }
                },
                ""required"": [""projectId""]
            }",
            Handler = async (parameters) =>
            {
                var projectId = Guid.Parse(parameters["projectId"].ToString()!);
                return new { projectId, message = "Project requirements tool executed" };
            }
        };
    }

    private static AgentTool GetSkillsTool()
    {
        return new AgentTool
        {
            Name = "get_skills",
            Description = "Obtiene el catálogo de habilidades disponibles en la organización",
            Schema = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""category"": { ""type"": ""string"", ""nullable"": true }
                }
            }",
            Handler = async (parameters) =>
            {
                var category = parameters.ContainsKey("category")
                    ? parameters["category"]?.ToString()
                    : null;
                return new { category, message = "Skills tool executed" };
            }
        };
    }

    private static AgentTool GetCertificationsTool()
    {
        return new AgentTool
        {
            Name = "get_certifications",
            Description = "Obtiene las certificaciones de un empleado",
            Schema = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""userId"": { ""type"": ""string"", ""format"": ""uuid"" }
                },
                ""required"": [""userId""]
            }",
            Handler = async (parameters) =>
            {
                var userId = Guid.Parse(parameters["userId"].ToString()!);
                return new { userId, message = "Certifications tool executed" };
            }
        };
    }

    private static AgentTool GetProjectHistoryTool()
    {
        return new AgentTool
        {
            Name = "get_project_history",
            Description = "Obtiene el historial de participación en proyectos de un empleado",
            Schema = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""userId"": { ""type"": ""string"", ""format"": ""uuid"" }
                },
                ""required"": [""userId""]
            }",
            Handler = async (parameters) =>
            {
                var userId = Guid.Parse(parameters["userId"].ToString()!);
                return new { userId, message = "Project history tool executed" };
            }
        };
    }

    /// <summary>
    /// Genera el prompt del sistema con la definición de herramientas
    /// </summary>
    public static string GenerateToolsPrompt()
    {
        var tools = GetAvailableTools();
        var toolDescriptions = string.Join("\n\n", tools.Select(t =>
            $"**{t.Name}**: {t.Description}\nSchema: {t.Schema}"));

        return $@"
Tienes acceso a las siguientes herramientas para consultar datos:

{toolDescriptions}

Para usar una herramienta, incluye en tu respuesta:
TOOL_CALL: {{""name"": ""nombre_herramienta"", ""parameters"": {{...}}}}

Puedes usar múltiples herramientas en secuencia para completar tareas complejas.";
    }
}