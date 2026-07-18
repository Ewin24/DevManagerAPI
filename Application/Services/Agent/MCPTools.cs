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
            GetCurrentUserContextTool(),
            GetEmployeeProfileTool(),
            GetProjectRequirementsTool(),
            GetSkillsTool(),
            GetCertificationsTool(),
            GetProjectHistoryTool(),
            GetProjectApplicationsTool(),
            GetRoleAssignmentsTool(),
            GetTrainingGapsTool(),
            GetOrgOverviewTool()
        };
    }

    private static AgentTool GetCurrentUserContextTool()
    {
        return new AgentTool
        {
            Name = "get_current_user_context",
            Description = "Obtiene el contexto del usuario actual que está interactuando con el agente. Esta herramienta no requiere parámetros y retorna el perfil, habilidades, certificaciones e historial de proyectos del usuario que hace la consulta.",
            Schema = @"{
                ""type"": ""object"",
                ""properties"": {}
            }",
            Handler = async (parameters) =>
            {
                return new { message = "Use the current user context from the conversation. This tool is called automatically when the query refers to 'yo', 'mi', 'mis' or similar pronouns." };
            }
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

    private static AgentTool GetProjectApplicationsTool()
    {
        return new AgentTool
        {
            Name = "get_project_applications",
            Description = "Obtiene las postulaciones a proyectos de la organización. Puede filtrar por proyecto o mostrar todas las postulaciones pendientes de revisión.",
            Schema = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""projectId"": { ""type"": ""string"", ""format"": ""uuid"", ""nullable"": true },
                    ""status"": { ""type"": ""string"", ""enum"": [""Applied"", ""Approved"", ""Rejected""], ""nullable"": true }
                }
            }",
            Handler = async (parameters) =>
            {
                var projectId = parameters.ContainsKey("projectId") ? parameters["projectId"]?.ToString() : null;
                var status = parameters.ContainsKey("status") ? parameters["status"]?.ToString() : null;
                return new { projectId, status, message = "Project applications tool executed" };
            }
        };
    }

    private static AgentTool GetRoleAssignmentsTool()
    {
        return new AgentTool
        {
            Name = "get_role_assignments",
            Description = "Obtiene los roles de la organización y los usuarios asignados a cada rol. Útil para consultas sobre permisos, accesos y control RBAC.",
            Schema = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""roleName"": { ""type"": ""string"", ""nullable"": true }
                }
            }",
            Handler = async (parameters) =>
            {
                var roleName = parameters.ContainsKey("roleName") ? parameters["roleName"]?.ToString() : null;
                return new { roleName, message = "Role assignments tool executed" };
            }
        };
    }

    private static AgentTool GetTrainingGapsTool()
    {
        return new AgentTool
        {
            Name = "get_training_gaps",
            Description = "Analiza las brechas de capacitación del equipo comparando habilidades actuales con las demandadas por los proyectos activos. Identifica empleados con skills en nivel bajo.",
            Schema = @"{
                ""type"": ""object"",
                ""properties"": {
                    ""minLevel"": { ""type"": ""integer"", ""minimum"": 1, ""maximum"": 5, ""nullable"": true }
                }
            }",
            Handler = async (parameters) =>
            {
                var minLevel = parameters.ContainsKey("minLevel") ? parameters["minLevel"]?.ToString() : "3";
                return new { minLevel, message = "Training gaps tool executed" };
            }
        };
    }

    private static AgentTool GetOrgOverviewTool()
    {
        return new AgentTool
        {
            Name = "get_org_overview",
            Description = "Obtiene un resumen general de la organización: total de usuarios, proyectos por estado, principales habilidades del equipo y empleados con perfil registrado.",
            Schema = @"{
                ""type"": ""object"",
                ""properties"": {}
            }",
            Handler = async (parameters) =>
            {
                return new { message = "Org overview tool executed — returns total users, projects by status, top skills" };
            }
        };
    }
}