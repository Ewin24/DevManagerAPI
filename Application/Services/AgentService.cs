using Application.DTOs.Agent;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Application.Services;

/// <summary>
/// Orquestador principal del agente de talento (patrón MCP Tool Use)
/// </summary>
public class AgentService : IAgentService
{
    private readonly IGeminiService _geminiService;
    private readonly IProfileService _profileService;
    private readonly ISkillService _skillService;
    private readonly IEmployeeSkillService _employeeSkillService;
    private readonly IProjectService _projectService;
    private readonly IUserService _userService;
    private readonly IAgentRepository _agentRepository;
    private readonly ICertificationService _certificationService;
    private readonly IRolePermissionService _rolePermissionService;
    private readonly IApplicationService _applicationService;
    private readonly ILogger<AgentService> _logger;

    public AgentService(
        IGeminiService geminiService,
        IProfileService profileService,
        ISkillService skillService,
        IEmployeeSkillService employeeSkillService,
        IProjectService projectService,
        IUserService userService,
        IAgentRepository agentRepository,
        ICertificationService certificationService,
        IRolePermissionService rolePermissionService,
        IApplicationService applicationService,
        ILogger<AgentService> logger)
    {
        _geminiService = geminiService;
        _profileService = profileService;
        _skillService = skillService;
        _employeeSkillService = employeeSkillService;
        _projectService = projectService;
        _userService = userService;
        _agentRepository = agentRepository;
        _certificationService = certificationService;
        _rolePermissionService = rolePermissionService;
        _applicationService = applicationService;
        _logger = logger;
    }

    public async Task<AgentQueryResponse> QueryAsync(
        Guid organizationId,
        Guid userId,
        AgentQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Procesando query del agente para org {OrgId}, usuario {UserId}: {Query}",
            organizationId, userId, request.Query);

        var toolsExecuted = new List<ToolExecutionResult>();

        // Guardia de intención: rechazar consultas fuera del dominio antes de llamar a Gemini
        if (IsOffTopicQuery(request.Query))
        {
            _logger.LogInformation("Consulta off-topic rechazada para org {OrgId}: {Query}", organizationId, request.Query);
            return new AgentQueryResponse
            {
                ResponseType = "text",
                Summary = "Consulta fuera del ámbito del sistema DevManager.",
                Markdown = "## Fuera de mi ámbito\n\nSolo puedo responder consultas relacionadas con la gestión de talento y proyectos de tu organización.\n\n**Puedo ayudarte con:**\n- Habilidades y competencias del equipo\n- Proyectos activos y sus requisitos\n- Perfiles y certificaciones de empleados\n- Postulaciones a proyectos\n- Brechas de capacitación\n- Roles y permisos de la organización\n- Resumen general de la organización",
                SuggestedActions = new List<SuggestedAction>
                {
                    new() { Label = "Ver habilidades del equipo", Query = "¿Qué habilidades tiene el equipo?", Icon = "skill" },
                    new() { Label = "Proyectos activos", Query = "¿Cuáles son los proyectos activos?", Icon = "project" },
                    new() { Label = "Resumen de la organización", Query = "Dame un resumen general de la organización", Icon = "org" }
                }
            };
        }

        try
        {
            // Determinar qué datos necesita la consulta y obtenerlos
            var contextData = await GatherContextDataAsync(organizationId, userId, request.Query, cancellationToken);

            if (contextData.Any())
            {
                toolsExecuted.AddRange(contextData.Select(cd => new ToolExecutionResult
                {
                    ToolName = cd.Key,
                    Input = request.Query,
                    Output = $"Obtenidos {(cd.Value as System.Collections.IEnumerable)?.Cast<object>().Count() ?? 1} registros",
                    Success = true
                }));
            }

            // Construir prompt con contexto del sistema y datos reales
            var systemPrompt = BuildSystemPrompt(organizationId);
            var dataContext = BuildDataContext(contextData);

            var fullPrompt = $@"{systemPrompt}

**DATOS DISPONIBLES DE LA ORGANIZACIÓN:**
{dataContext}

**Consulta del usuario:** {request.Query}";

            if (!string.IsNullOrEmpty(request.Context))
            {
                fullPrompt += $"\n\n**Contexto adicional:** {request.Context}";
            }

            fullPrompt += @"

**INSTRUCCIONES DE FORMATO:**
- USA FORMATO MARKDOWN para estructurar tu respuesta
- NO devuelvas JSON puro como respuesta principal
- Usa encabezados (##, ###), listas (-, *), tablas cuando sea necesario
- El campo 'summary' debe ser un resumen corto (1-2 oraciones)
- El contenido principal debe ser markdown legible directamente por el usuario
- Analiza los DATOS REALES proporcionados arriba
- Proporciona respuestas específicas basadas en esos datos
- Incluye números, nombres y métricas concretas
- NO uses placeholders como '[Nombre de la habilidad]'
- Si los datos no son suficientes, indícalo claramente";

            // Obtener respuesta con razonamiento (Chain of Thought)
            var (response, reasoning) = await _geminiService.QueryWithReasoningAsync(
                fullPrompt, cancellationToken);

            // Limpiar y formatear la respuesta como markdown
            var cleanedMarkdown = CleanGeminiResponse(response);

            // Registrar la acción del agente
            var actionId = await _agentRepository.CreateActionAsync(
                organizationId,
                "GENERAL_QUERY",
                request.Query,
                JsonSerializer.Serialize(request),
                JsonSerializer.Serialize(new { response, reasoning }),
                request.RequireApproval ? "PENDING_APPROVAL" : "SUCCESS");

            return new AgentQueryResponse
            {
                ResponseType = DetermineResponseType(cleanedMarkdown, contextData),
                Summary = BuildSummary(cleanedMarkdown, request.Query),
                Markdown = cleanedMarkdown,
                Payload = BuildPayload(response, contextData),
                Metadata = new ResponseMetadata
                {
                    Reasoning = reasoning,
                    ToolsExecuted = toolsExecuted,
                    RequiresHumanApproval = request.RequireApproval,
                    ActionId = actionId
                },
                SuggestedActions = GenerateSuggestedActions(request.Query, cleanedMarkdown)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando query del agente");
            throw;
        }
    }

    private static string CleanGeminiResponse(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            return "No se pudo generar una respuesta.";

        var cleaned = response.Trim();

        // Si la respuesta es JSON embebido, intentar parsear y convertir a markdown
        if (cleaned.Contains("{") && cleaned.Contains("}"))
        {
            // Remover markdown code blocks
            cleaned = cleaned.Replace("```json", "").Replace("```", "").Replace("```json\n", "").Trim();

            // Si empieza con {, intentamos parsear como JSON
            if (cleaned.TrimStart().StartsWith("{"))
            {
                try
                {
                    var jsonDoc = JsonDocument.Parse(cleaned);
                    return ConvertJsonToMarkdown(jsonDoc.RootElement);
                }
                catch
                {
                    // Si no se puede parsear, limpiar como texto
                    return CleanJsonString(cleaned);
                }
            }
        }

        // Limpiar cualquier resto de JSON en el texto
        return CleanJsonString(cleaned);
    }

    private static string ConvertJsonToMarkdown(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var md = new System.Text.StringBuilder();
            var properties = element.EnumerateObject().ToList();

            for (int i = 0; i < properties.Count; i++)
            {
                var prop = properties[i];
                var value = prop.Value;

                if (value.ValueKind == JsonValueKind.Object)
                {
                    md.AppendLine($"### {FormatPropertyName(prop.Name)}");
                    md.AppendLine(ConvertJsonToMarkdown(value));
                }
                else if (value.ValueKind == JsonValueKind.Array)
                {
                    md.AppendLine($"### {FormatPropertyName(prop.Name)}");
                    var items = value.EnumerateArray().ToList();
                    for (int j = 0; j < items.Count; j++)
                    {
                        var item = items[j];
                        if (item.ValueKind == JsonValueKind.Object)
                        {
                            md.AppendLine($"- **{j + 1}.** ");
                            md.AppendLine(ConvertJsonToMarkdown(item));
                        }
                        else
                        {
                            md.AppendLine($"- {FormatValue(item)}");
                        }
                    }
                }
                else
                {
                    var formattedValue = FormatValue(value);
                    if (!string.IsNullOrEmpty(formattedValue))
                    {
                        md.AppendLine($"**{FormatPropertyName(prop.Name)}:** {formattedValue}");
                    }
                }
            }
            return md.ToString();
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            var md = new System.Text.StringBuilder();
            foreach (var item in element.EnumerateArray())
            {
                md.AppendLine($"- {FormatValue(item)}");
            }
            return md.ToString();
        }

        return FormatValue(element);
    }

    private static string FormatPropertyName(string name)
    {
        // Convertir camelCase a Título con espacios
        return System.Text.RegularExpressions.Regex.Replace(name, 
            "([a-z])([A-Z])", "$1 $2").Trim();
    }

    private static string FormatValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? "",
            JsonValueKind.Number => element.GetRawText(),
            JsonValueKind.True => "Sí",
            JsonValueKind.False => "No",
            JsonValueKind.Null => "",
            _ => element.GetRawText()
        };
    }

    private static string CleanJsonString(string text)
    {
        // Remover claves JSON restantes que puedan estar en el texto
        var cleaned = System.Text.RegularExpressions.Regex.Replace(
            text, 
            @"""[\w]+"":\s*", 
            "");
        
        cleaned = cleaned.Replace("{", "").Replace("}", "");
        cleaned = cleaned.Replace("[\n", "").Replace("\n]", "");
        cleaned = cleaned.Replace("\"", "");

        // Limpiar commas restantes al final de líneas
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @",\s*$", "", System.Text.RegularExpressions.RegexOptions.Multiline);
        
        return cleaned.Trim();
    }

    private static string DetermineResponseType(string response, Dictionary<string, object> contextData)
    {
        var responseLower = response.ToLowerInvariant();

        if (contextData.ContainsKey("SkillStatistics") ||
            contextData.ContainsKey("EmployeeProfiles") ||
            contextData.ContainsKey("Projects") ||
            contextData.ContainsKey("OrgCertifications") ||
            contextData.ContainsKey("ProjectApplications") ||
            contextData.ContainsKey("UserRoleAssignments") ||
            contextData.ContainsKey("TrainingGapAnalysis") ||
            contextData.ContainsKey("OrgOverview"))
        {
            if (responseLower.Contains("tabla") || responseLower.Contains("tabla de") ||
                responseLower.Contains("nivel") || responseLower.Contains("empleado"))
            {
                return "table";
            }
            if (responseLower.Contains("lista") || responseLower.Contains("listado") ||
                responseLower.Contains("recomendaciones") || responseLower.Contains("certificaciones") ||
                responseLower.Contains("postulaciones") || responseLower.Contains("roles"))
            {
                return "list";
            }
            return "mixed";
        }

        if (responseLower.Contains("recomendación") || responseLower.Contains("opciones") ||
            responseLower.Contains("alternativas"))
        {
            return "list";
        }

        return "text";
    }

    private static string BuildSummary(string response, string query)
    {
        var firstLines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Take(3)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrEmpty(l) && !l.StartsWith("**"))
            .ToList();

        if (firstLines.Count == 0)
        {
            return $"Resultado para: {query}";
        }

        var summary = string.Join(" ", firstLines);
        return summary.Length > 200 ? summary.Substring(0, 197) + "..." : summary;
    }

    private static ResponsePayload BuildPayload(string response, Dictionary<string, object> contextData)
    {
        var payload = new ResponsePayload { Text = response };

        if (contextData.ContainsKey("SkillStatistics"))
        {
            var stats = contextData["SkillStatistics"] as List<dynamic>;
            if (stats != null && stats.Any())
            {
                payload = payload with
                {
                    Table = new TablePayload
                    {
                        Headers = new List<string> { "Habilidad", "Empleados", "Nivel Promedio", "Nivel Máximo" },
                        Rows = stats.Take(10).Select(s => new List<string>
                        {
                            s.SkillName?.ToString() ?? "",
                            s.EmployeeCount?.ToString() ?? "0",
                            s.AverageLevel?.ToString("F1") ?? "0",
                            s.MaxLevel?.ToString() ?? "0"
                        }).ToList()
                    }
                };
            }
        }

        if (contextData.ContainsKey("EmployeeProfiles"))
        {
            var profiles = contextData["EmployeeProfiles"] as List<dynamic>;
            if (profiles != null && profiles.Any() && payload.Table == null)
            {
                var profileList = profiles.Take(10).Select(p => new ListItem
                {
                    Id = p.UserId?.ToString(),
                    Label = $"{p.FirstName} {p.LastName}",
                    Value = $"Años exp: {p.YearsExperience ?? 0}"
                }).ToList();

                payload = payload with
                {
                    List = new ListPayload { Items = profileList }
                };
            }
        }

        return payload;
    }

    private static List<SuggestedAction> GenerateSuggestedActions(string query, string response)
    {
        var actions = new List<SuggestedAction>();
        var queryLower = query.ToLowerInvariant();
        var responseLower = response.ToLowerInvariant();

        if (queryLower.Contains("habilidad") || queryLower.Contains("skill"))
        {
            actions.Add(new SuggestedAction { Label = "Ver empleados por habilidad", Query = "dame la lista de empleados por habilidad" });
            actions.Add(new SuggestedAction { Label = "Estadísticas de skills", Query = "muéstrame las estadísticas de habilidades" });
            actions.Add(new SuggestedAction { Label = "Brechas de capacitación", Query = "identifica las brechas de capacitación del equipo" });
        }

        if (queryLower.Contains("proyecto") || queryLower.Contains("proyect"))
        {
            actions.Add(new SuggestedAction { Label = "Buscar candidatos", Query = "busca candidatos para este proyecto" });
            actions.Add(new SuggestedAction { Label = "Ver postulaciones", Query = "muéstrame las postulaciones a los proyectos activos" });
        }

        if (queryLower.Contains("certificac") || queryLower.Contains("cert"))
        {
            actions.Add(new SuggestedAction { Label = "Mis certificaciones", Query = "muéstrame mis certificaciones" });
            actions.Add(new SuggestedAction { Label = "Certificaciones del equipo", Query = "qué certificaciones tiene el equipo" });
        }

        if (queryLower.Contains("postulac") || queryLower.Contains("aplica"))
        {
            actions.Add(new SuggestedAction { Label = "Ver proyectos activos", Query = "muéstrame los proyectos activos" });
            actions.Add(new SuggestedAction { Label = "Postulaciones pendientes", Query = "cuáles son las postulaciones pendientes de revisión" });
        }

        if (queryLower.Contains("rol") || queryLower.Contains("permiso") || queryLower.Contains("acceso"))
        {
            actions.Add(new SuggestedAction { Label = "Ver todos los roles", Query = "muéstrame todos los roles de la organización" });
            actions.Add(new SuggestedAction { Label = "Quién es administrador", Query = "quiénes tienen rol de administrador" });
        }

        if (queryLower.Contains("brecha") || queryLower.Contains("capacitac") || queryLower.Contains("formac"))
        {
            actions.Add(new SuggestedAction { Label = "Skills del equipo", Query = "qué habilidades tiene el equipo" });
            actions.Add(new SuggestedAction { Label = "Empleados sin skills", Query = "cuántos empleados no tienen habilidades registradas" });
        }

        if (responseLower.Contains("no cuento con la información") || responseLower.Contains("no tengo información"))
        {
            actions.Clear();
            actions.Add(new SuggestedAction { Label = "Resumen general", Query = "dame un resumen general de la organización" });
            actions.Add(new SuggestedAction { Label = "Estado de proyectos", Query = "muéstrame el estado de todos los proyectos" });
            actions.Add(new SuggestedAction { Label = "Estadísticas de skills", Query = "muéstrame las estadísticas de habilidades del equipo" });
        }

        if (!actions.Any())
        {
            if (!queryLower.Contains("mi perfil") && !queryLower.Contains("mis habilidades"))
                actions.Add(new SuggestedAction { Label = "Mi perfil", Query = "muéstrame mi perfil" });
            actions.Add(new SuggestedAction { Label = "Resumen general", Query = "dame un resumen general de la organización" });
            actions.Add(new SuggestedAction { Label = "Proyectos activos", Query = "cuáles son los proyectos activos" });
        }

        return actions.Take(3).ToList();
    }

    private async Task<Dictionary<string, object>> GatherContextDataAsync(
        Guid organizationId,
        Guid userId,
        string query,
        CancellationToken cancellationToken)
    {
        var contextData = new Dictionary<string, object>();
        var queryLower = query.ToLowerInvariant();

        var isCurrentUserQuery = queryLower.Contains("yo") || 
                                  queryLower.Contains("mi ") || 
                                  queryLower.Contains("mis ") ||
                                  queryLower.Contains("mí") ||
                                  queryLower.Contains("para mí") ||
                                  queryLower.Contains("para mi") ||
                                  queryLower.Contains("que me ") ||
                                  queryLower.Contains("me recomi");
        try
        {
            // Detectar si la query es sobre habilidades
            if (queryLower.Contains("habilidad") || queryLower.Contains("skill") ||
                queryLower.Contains("competencia") || queryLower.Contains("capacidad"))
            {
                var skills = await _skillService.GetAllSkillsAsync(organizationId);
                var profiles = await _profileService.GetAllProfilesWithSkillsAsync(organizationId);

                // Agregar estadísticas de habilidades desde los perfiles
                var allEmployeeSkills = profiles
                    .SelectMany(p => p.Skills.Select(s => new
                    {
                        UserId = p.UserId,
                        SkillName = s.SkillName,
                        Level = s.CurrentLevel
                    }))
                    .ToList();

                var skillStats = allEmployeeSkills
                    .GroupBy(es => es.SkillName)
                    .Select(g => new
                    {
                        SkillName = g.Key,
                        EmployeeCount = g.Select(x => x.UserId).Distinct().Count(),
                        AverageLevel = g.Average(x => (double)x.Level),
                        MaxLevel = g.Max(x => x.Level),
                        MinLevel = g.Min(x => x.Level),
                        TotalOccurrences = g.Count()
                    })
                    .OrderByDescending(s => s.EmployeeCount)
                    .ThenByDescending(s => s.AverageLevel)
                    .ToList();

                contextData["Skills"] = skills;
                contextData["EmployeeSkills"] = allEmployeeSkills;
                contextData["SkillStatistics"] = skillStats;
            }

            // Detectar si la query es sobre proyectos
            if (queryLower.Contains("proyecto") || queryLower.Contains("project"))
            {
                var projects = await _projectService.GetAllProjectsAsync(organizationId);
                contextData["Projects"] = projects;
            }

            // Detectar si la query es sobre empleados/usuarios
            if (queryLower.Contains("empleado") || queryLower.Contains("usuario") ||
                queryLower.Contains("candidato") || queryLower.Contains("desarrollador") ||
                queryLower.Contains("equipo") || queryLower.Contains("team") ||
                queryLower.Contains("persona"))
            {
                var profiles = await _profileService.GetAllProfilesWithSkillsAsync(organizationId);
                var users = await _userService.GetAllAsync(organizationId);

                contextData["EmployeeProfiles"] = profiles;
                contextData["Users"] = users;
            }

            // Detectar si la query es sobre el usuario actual
            if (isCurrentUserQuery)
            {
                try
                {
                    var currentUserProfile = await _profileService.GetMyProfileAsync(userId, organizationId);
                    var currentUser = await _userService.GetByIdAsync(userId, organizationId);
                    
                    var profilesWithSkills = await _profileService.GetAllProfilesWithSkillsAsync(organizationId);
                    var currentProfileWithSkills = profilesWithSkills.FirstOrDefault(p => p.UserId == userId);

                    if (currentUserProfile != null)
                    {
                        List<object> skillsList = new List<object>();
                        if (currentProfileWithSkills != null)
                        {
                            skillsList = currentProfileWithSkills.Skills.Select(s => (object)new
                            {
                                skillName = s.SkillName,
                                level = s.CurrentLevel,
                                validated = s.LastValidatedAt.HasValue
                            }).ToList();
                        }

                        List<object> certsList = currentUserProfile.Certifications.Select(c => (object)new
                        {
                            name = c.Name,
                            issuer = c.Issuer,
                            issueDate = c.IssueDate
                        }).ToList();

                        contextData["CurrentUserProfile"] = new
                        {
                            userId = currentUserProfile.UserId,
                            fullName = currentUser != null ? $"{currentUser.FirstName} {currentUser.LastName}" : "Usuario",
                            email = currentUser?.Email ?? "",
                            bio = currentUserProfile.Bio,
                            yearsExperience = currentUserProfile.YearsExperience,
                            skills = skillsList,
                            certifications = certsList
                        };
                    }

                    _logger.LogInformation("Se obtuvo contexto del usuario actual {UserId}", userId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error obteniendo perfil del usuario actual {UserId}", userId);
                }
            }

            // Detectar si la query es sobre certificaciones
            if (queryLower.Contains("certificac") || queryLower.Contains("certificado") ||
                queryLower.Contains("cert") || queryLower.Contains("título") ||
                queryLower.Contains("titulo") || queryLower.Contains("acreditac"))
            {
                try
                {
                    if (isCurrentUserQuery)
                    {
                        var myCerts = await _certificationService.GetCertificationsByUserIdAsync(userId, organizationId);
                        contextData["MyCertifications"] = myCerts;
                    }
                    else
                    {
                        // Org-wide: obtener certs de todos los usuarios activos
                        var users = await _userService.GetAllAsync(organizationId);
                        var allCerts = new List<object>();
                        foreach (var user in users.Where(u => u.IsActive).Take(20))
                        {
                            var userCerts = await _certificationService.GetCertificationsByUserIdAsync(user.Id, organizationId);
                            allCerts.AddRange(userCerts.Select(c => new
                            {
                                userId = user.Id,
                                userName = $"{user.FirstName} {user.LastName}",
                                certName = c.Name,
                                issuer = c.Issuer,
                                issueDate = c.IssueDate,
                                expirationDate = c.ExpirationDate
                            }));
                        }
                        contextData["OrgCertifications"] = allCerts;
                    }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Error obteniendo certificaciones"); }
            }

            // Detectar si la query es sobre postulaciones / aplicaciones a proyectos
            if (queryLower.Contains("postulac") || queryLower.Contains("aplica") ||
                queryLower.Contains("candid") || queryLower.Contains("solicitud") ||
                queryLower.Contains("vacante") || queryLower.Contains("interesad") ||
                queryLower.Contains("revision") || queryLower.Contains("revisión") ||
                queryLower.Contains("pendiente"))
            {
                try
                {
                    var projects = contextData.ContainsKey("Projects")
                        ? (contextData["Projects"] as IEnumerable<Application.DTOs.Projects.ProjectResponse>)?.ToList()
                        ?? (await _projectService.GetAllProjectsAsync(organizationId)).ToList()
                        : (await _projectService.GetAllProjectsAsync(organizationId)).ToList();

                    var allApplications = new List<object>();
                    foreach (var project in projects.Take(15))
                    {
                        var apps = await _applicationService.GetProjectApplicationsAsync(project.Id, organizationId);
                        allApplications.AddRange(apps.Select(a => new
                        {
                            projectId = project.Id,
                            projectName = project.Name,
                            applicationId = a.Id,
                            applicantName = a.UserName,
                            status = a.Status.ToString(),
                            message = a.Message,
                            appliedAt = a.CreatedAt,
                            reviewedAt = a.ReviewedAt
                        }));
                    }
                    contextData["ProjectApplications"] = allApplications;
                    if (!contextData.ContainsKey("Projects"))
                        contextData["Projects"] = projects;
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Error obteniendo postulaciones"); }
            }

            // Detectar si la query es sobre roles, permisos o control de acceso (RBAC)
            if (queryLower.Contains(" rol") || queryLower.Contains("role") ||
                queryLower.Contains("permiso") || queryLower.Contains("acceso") ||
                queryLower.Contains("rbac") || queryLower.Contains("administrador") ||
                queryLower.Contains("quién tiene") || queryLower.Contains("autorización") ||
                queryLower.Contains("autorizac"))
            {
                try
                {
                    var roles = await _rolePermissionService.GetAllRolesAsync(organizationId);
                    var userRoleAssignments = await _rolePermissionService.GetUserRoleAssignmentsAsync(organizationId);
                    contextData["Roles"] = roles;
                    contextData["UserRoleAssignments"] = userRoleAssignments;
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Error obteniendo roles/permisos"); }
            }

            // Detectar si la query es sobre brechas de capacitación / formación
            if (queryLower.Contains("brecha") || queryLower.Contains("capacitac") ||
                queryLower.Contains("formac") || queryLower.Contains("entrenamiento") ||
                queryLower.Contains("gap") || queryLower.Contains("aprender") ||
                queryLower.Contains("mejorar") || queryLower.Contains("desarrollarse") ||
                queryLower.Contains("necesita") || queryLower.Contains("falta"))
            {
                try
                {
                    var profiles = await _profileService.GetAllProfilesWithSkillsAsync(organizationId);
                    var projects = await _projectService.GetAllProjectsAsync(organizationId);
                    var skills = await _skillService.GetAllSkillsAsync(organizationId);
                    var allUsers = await _userService.GetAllAsync(organizationId);
                    var userNameDict = allUsers.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}");

                    var trainingGapData = new
                    {
                        totalEmployees = profiles.Count(),
                        employeesWithNoSkills = profiles.Count(p => !p.Skills.Any()),
                        skillCoverage = profiles
                            .SelectMany(p => p.Skills.Select(s => s.SkillName))
                            .GroupBy(s => s)
                            .Select(g => new { skill = g.Key, employeeCount = g.Count() })
                            .OrderByDescending(x => x.employeeCount)
                            .Take(15)
                            .ToList(),
                        employeesWithLowLevelSkills = profiles
                            .SelectMany(p => p.Skills
                                .Where(s => s.CurrentLevel < 3)
                                .Select(s => new
                                {
                                    employeeName = userNameDict.TryGetValue(p.UserId, out var n) ? n : p.UserId.ToString(),
                                    skill = s.SkillName,
                                    currentLevel = s.CurrentLevel
                                }))
                            .Take(20)
                            .ToList(),
                        projectsNeedingStaff = projects
                            .Where(p => p.Status == Domain.Enums.ProjectStatus.Draft ||
                                        p.Status == Domain.Enums.ProjectStatus.Open ||
                                        p.Status == Domain.Enums.ProjectStatus.InProgress)
                            .Select(p => new { name = p.Name, status = p.Status.ToString() })
                            .ToList()
                    };
                    contextData["TrainingGapAnalysis"] = trainingGapData;
                    if (!contextData.ContainsKey("Skills")) contextData["Skills"] = skills;
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Error calculando brechas de capacitación"); }
            }

            // Catch-all: si ningún bloque específico cargó datos, cargar resumen general de la organización
            if (!contextData.Any())
            {
                try
                {
                    var users = await _userService.GetAllAsync(organizationId);
                    var projects = await _projectService.GetAllProjectsAsync(organizationId);
                    var skills = await _skillService.GetAllSkillsAsync(organizationId);
                    var profiles = await _profileService.GetAllProfilesWithSkillsAsync(organizationId);

                    contextData["OrgOverview"] = new
                    {
                        totalUsers = users.Count(),
                        activeUsers = users.Count(u => u.IsActive),
                        totalProjects = projects.Count(),
                        projectsByStatus = projects
                            .GroupBy(p => p.Status)
                            .Select(g => new { status = g.Key.ToString(), count = g.Count() })
                            .ToList(),
                        totalSkills = skills.Count(),
                        employeesWithProfile = profiles.Count(),
                        topSkills = profiles
                            .SelectMany(p => p.Skills.Select(s => s.SkillName))
                            .GroupBy(s => s)
                            .OrderByDescending(g => g.Count())
                            .Take(8)
                            .Select(g => g.Key)
                            .ToList()
                    };
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Error obteniendo overview de la organización"); }
            }

            _logger.LogInformation("Se obtuvieron {Count} conjuntos de datos para el contexto", contextData.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error obteniendo datos de contexto");
        }

        return contextData;
    }

    private static string BuildDataContext(Dictionary<string, object> contextData)
    {
        if (!contextData.Any())
        {
            return "No se encontraron datos relevantes en la base de datos.";
        }

        var context = new System.Text.StringBuilder();

        if (contextData.ContainsKey("SkillStatistics"))
        {
            var stats = contextData["SkillStatistics"];
            context.AppendLine("### ESTADÍSTICAS DE HABILIDADES:");
            context.AppendLine(JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true }));
            context.AppendLine();
        }

        if (contextData.ContainsKey("Skills"))
        {
            var skills = contextData["Skills"];
            context.AppendLine("### CATÁLOGO DE HABILIDADES:");
            context.AppendLine(JsonSerializer.Serialize(skills, new JsonSerializerOptions { WriteIndented = true }));
            context.AppendLine();
        }

        if (contextData.ContainsKey("Projects"))
        {
            var projects = contextData["Projects"];
            context.AppendLine("### PROYECTOS:");
            context.AppendLine(JsonSerializer.Serialize(projects, new JsonSerializerOptions { WriteIndented = true }));
            context.AppendLine();
        }

        if (contextData.ContainsKey("EmployeeProfiles"))
        {
            var profiles = contextData["EmployeeProfiles"];
            context.AppendLine("### PERFILES DE EMPLEADOS:");
            context.AppendLine(JsonSerializer.Serialize(profiles, new JsonSerializerOptions { WriteIndented = true }));
            context.AppendLine();
        }

        if (contextData.ContainsKey("CurrentUserProfile"))
        {
            var currentUser = contextData["CurrentUserProfile"];
            context.AppendLine("### PERFIL DEL USUARIO ACTUAL:");
            context.AppendLine(JsonSerializer.Serialize(currentUser, new JsonSerializerOptions { WriteIndented = true }));
            context.AppendLine();
        }

        if (contextData.ContainsKey("MyCertifications"))
        {
            var certs = contextData["MyCertifications"];
            context.AppendLine("### MIS CERTIFICACIONES:");
            context.AppendLine(JsonSerializer.Serialize(certs, new JsonSerializerOptions { WriteIndented = true }));
            context.AppendLine();
        }

        if (contextData.ContainsKey("OrgCertifications"))
        {
            var certs = contextData["OrgCertifications"];
            context.AppendLine("### CERTIFICACIONES DE LA ORGANIZACIÓN:");
            context.AppendLine(JsonSerializer.Serialize(certs, new JsonSerializerOptions { WriteIndented = true }));
            context.AppendLine();
        }

        if (contextData.ContainsKey("ProjectApplications"))
        {
            var apps = contextData["ProjectApplications"];
            context.AppendLine("### POSTULACIONES A PROYECTOS:");
            context.AppendLine(JsonSerializer.Serialize(apps, new JsonSerializerOptions { WriteIndented = true }));
            context.AppendLine();
        }

        if (contextData.ContainsKey("Roles"))
        {
            var roles = contextData["Roles"];
            context.AppendLine("### ROLES DE LA ORGANIZACIÓN:");
            context.AppendLine(JsonSerializer.Serialize(roles, new JsonSerializerOptions { WriteIndented = true }));
            context.AppendLine();
        }

        if (contextData.ContainsKey("UserRoleAssignments"))
        {
            var assignments = contextData["UserRoleAssignments"];
            context.AppendLine("### ASIGNACIONES DE ROL POR USUARIO:");
            context.AppendLine(JsonSerializer.Serialize(assignments, new JsonSerializerOptions { WriteIndented = true }));
            context.AppendLine();
        }

        if (contextData.ContainsKey("TrainingGapAnalysis"))
        {
            var gaps = contextData["TrainingGapAnalysis"];
            context.AppendLine("### ANÁLISIS DE BRECHAS DE CAPACITACIÓN:");
            context.AppendLine(JsonSerializer.Serialize(gaps, new JsonSerializerOptions { WriteIndented = true }));
            context.AppendLine();
        }

        if (contextData.ContainsKey("OrgOverview"))
        {
            var overview = contextData["OrgOverview"];
            context.AppendLine("### RESUMEN GENERAL DE LA ORGANIZACIÓN:");
            context.AppendLine(JsonSerializer.Serialize(overview, new JsonSerializerOptions { WriteIndented = true }));
            context.AppendLine();
        }

        return context.ToString();
    }

    public async Task<SkillValidationResponse> ValidateSkillAsync(
        Guid organizationId,
        SkillValidationRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Validando skill {SkillId} para usuario {UserId}",
            request.SkillId, request.UserId);

        try
        {
            // 1. Obtener datos del empleado
            var profile = await _profileService.GetMyProfileAsync(request.UserId, organizationId);

            if (profile == null)
            {
                throw new InvalidOperationException($"Perfil de usuario {request.UserId} no encontrado");
            }

            // 2. Obtener skill details
            var skills = await _skillService.GetAllSkillsAsync(organizationId);
            var skill = skills.FirstOrDefault(s => s.Id == request.SkillId);

            if (skill == null)
            {
                throw new InvalidOperationException($"Skill {request.SkillId} no encontrada");
            }

            // 3. Obtener certificaciones relacionadas si existen
            var certifications = new List<string>();
            // Las certificaciones se pueden agregar en una fase futura
            // Por ahora validamos solo con skills y evidencia

            // 4. Construir prompt para validación semántica
            var validationPrompt = $@"
Actúa como un experto validador de competencias técnicas.

Analiza si el siguiente nivel de habilidad es coherente con la evidencia proporcionada:

**Habilidad:** {skill.Name} (Categoría: {skill.Category})
**Nivel declarado:** {request.Level}/5
**Usuario:** {profile.Bio ?? "Sin biografía"}
**Años de experiencia:** {profile.YearsExperience ?? 0}

**Certificaciones relacionadas:**
{(certifications.Any() ? string.Join("\n", certifications) : "Ninguna")}

**Evidencia URL:** {request.EvidenceUrl ?? "No proporcionada"}
**Descripción de experiencia:** {request.ExperienceDescription ?? "No proporcionada"}

Evalúa:
1. ¿El nivel declarado es coherente con la experiencia y certificaciones?
2. ¿Se requiere certificación formal para validar esta habilidad?
3. ¿Qué tan confiable es esta declaración (0-100)?

Responde en formato JSON:
{{
    ""isValid"": true/false,
    ""confidenceScore"": 0-100,
    ""validationReasoning"": ""explicación detallada"",
    ""recommendations"": [""recomendación 1"", ""recomendación 2""],
    ""warnings"": [""advertencia 1""],
    ""requiresCertification"": true/false
}}";

            var validationResult = await _geminiService.AnalyzeStructuredDataAsync<SkillValidationResponse>(
                validationPrompt,
                new { request, profile, skill, certifications },
                cancellationToken);

            // Registrar la validación
            await _agentRepository.CreateActionAsync(
                organizationId,
                "SKILL_VALIDATION",
                $"Validación de {skill.Name} nivel {request.Level}",
                JsonSerializer.Serialize(request),
                JsonSerializer.Serialize(validationResult),
                validationResult.IsValid ? "SUCCESS" : "FAILED");

            return validationResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validando skill");
            throw;
        }
    }

    // MATCH CANDIDATES — algoritmo local primero, Gemini solo para narrativa
    public async Task<SkillMatchResponse> MatchCandidatesForProjectAsync(
        Guid organizationId,
        SkillMatchRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Matching candidatos para proyecto {ProjectId}", request.ProjectId);

        try
        {
            // ── 1. Obtener datos en paralelo para reducir latencia ──────────────
            var projectTask      = _projectService.GetProjectByIdAsync(request.ProjectId, organizationId);
            var requirementsTask = _projectService.GetSkillRequirementsAsync(request.ProjectId, organizationId);
            var profilesTask     = _profileService.GetAllProfilesWithSkillsAsync(organizationId);
            var usersTask        = _userService.GetAllAsync(organizationId);

            await Task.WhenAll(projectTask, requirementsTask, profilesTask, usersTask);

            var project      = await projectTask;
            var requirements = (await requirementsTask).ToList();
            var profiles     = (await profilesTask).ToList();
            var allUsers     = await usersTask;

            if (project == null)
                throw new InvalidOperationException($"Proyecto {request.ProjectId} no encontrado");

            if (!profiles.Any())
            {
                return new SkillMatchResponse
                {
                    ProjectId       = request.ProjectId,
                    ProjectName     = project.Name,
                    Candidates      = new List<CandidateMatch>(),
                    AnalysisNarrative = "No se encontraron candidatos con habilidades registradas en esta organización."
                };
            }

            // ── 2. Índice de usuarios para O(1) lookup ─────────────────────────
            var userIndex = allUsers.ToDictionary(
                u => u.Id,
                u => (FullName: $"{u.FirstName} {u.LastName}", Email: u.Email));

            // ── 3. Scoring local determinístico (sin Gemini) ───────────────────
            //   Mandatory skills  = 60 %
            //   Optional skills   = 20 %
            //   Experience        = 10 %
            //   Level surplus     = 10 %
            var mandatoryReqs = requirements.Where(r => r.IsMandatory).ToList();
            var optionalReqs  = requirements.Where(r => !r.IsMandatory).ToList();

            var scoredCandidates = profiles
                .Select(profile =>
                {
                    var skillMap = profile.Skills
                        .ToDictionary(s => s.SkillName.ToLowerInvariant(), s => s.CurrentLevel);

                    // Mandatory (60 %)
                    double mandatoryScore = 0;
                    if (mandatoryReqs.Any())
                    {
                        var met = mandatoryReqs.Count(r =>
                            skillMap.TryGetValue(r.SkillName.ToLowerInvariant(), out var lvl) &&
                            lvl >= r.RequiredLevel);
                        mandatoryScore = (double)met / mandatoryReqs.Count * 60.0;
                    }
                    else
                    {
                        mandatoryScore = 60.0; // sin obligatorios → score lleno en esa banda
                    }

                    // Optional (20 %)
                    double optionalScore = 0;
                    if (optionalReqs.Any())
                    {
                        var met = optionalReqs.Count(r =>
                            skillMap.TryGetValue(r.SkillName.ToLowerInvariant(), out var lvl) &&
                            lvl >= r.RequiredLevel);
                        optionalScore = (double)met / optionalReqs.Count * 20.0;
                    }

                    // Experience (10 %)
                    var years = profile.YearsExperience ?? 0;
                    double expScore = Math.Min(years / 10.0, 1.0) * 10.0;

                    // Skill surplus (10 %) — nivel real - nivel requerido en skills que sí cumple
                    var metReqs = requirements.Where(r =>
                        skillMap.TryGetValue(r.SkillName.ToLowerInvariant(), out var lvl) &&
                        lvl >= r.RequiredLevel).ToList();

                    double surplusScore = 0;
                    if (metReqs.Any())
                    {
                        var avgSurplus = metReqs
                            .Average(r => skillMap[r.SkillName.ToLowerInvariant()] - r.RequiredLevel);
                        surplusScore = Math.Min(avgSurplus / 2.0, 1.0) * 10.0;
                    }

                    var total = Math.Round(mandatoryScore + optionalScore + expScore + surplusScore, 1);

                    // Construir alignments
                    var alignments = requirements.Select(r =>
                    {
                        skillMap.TryGetValue(r.SkillName.ToLowerInvariant(), out var currentLevel);
                        return new SkillAlignment
                        {
                            SkillName     = r.SkillName,
                            RequiredLevel = r.RequiredLevel,
                            CurrentLevel  = currentLevel,
                            IsMandatory   = r.IsMandatory,
                            Meets         = currentLevel >= r.RequiredLevel
                        };
                    }).ToList();

                    userIndex.TryGetValue(profile.UserId, out var userInfo);

                    return new
                    {
                        Profile    = profile,
                        FullName   = userInfo.FullName ?? "Usuario desconocido",
                        Email      = userInfo.Email ?? "",
                        Score      = total,
                        Alignments = alignments,
                        MandatoryMet = mandatoryReqs.All(r =>
                            skillMap.TryGetValue(r.SkillName.ToLowerInvariant(), out var lvl) &&
                            lvl >= r.RequiredLevel)
                    };
                })
                .Where(c => c.Score >= request.MinScore)
                .OrderByDescending(c => c.Score)
                .Take(request.MaxCandidates ?? 10)
                .ToList();

            _logger.LogInformation(
                "Pre-scoring local: {Total} candidatos después de filtro (minScore={Min})",
                scoredCandidates.Count, request.MinScore);

            // ── 4. Si no pasó el filtro, responds sin llamar a Gemini ──────────
            if (!scoredCandidates.Any())
            {
                return new SkillMatchResponse
                {
                    ProjectId         = request.ProjectId,
                    ProjectName       = project.Name,
                    Candidates        = new List<CandidateMatch>(),
                    AnalysisNarrative = $"Ningún candidato supera el score mínimo de {request.MinScore}. " +
                                        "Considera reducir el filtro o revisar los requisitos del proyecto."
                };
            }

            // ── 5. Gemini solo genera narrativa para los top-N ya filtrados ────
            //    Prompt mínimo: sin JSON masivo, solo datos compactos
            var topSummary = string.Join("\n", scoredCandidates.Select((c, i) =>
            {
                var gaps = c.Alignments.Where(a => !a.Meets).Select(a =>
                    $"{a.SkillName}(tiene {a.CurrentLevel}, necesita {a.RequiredLevel})");
                var matched = c.Alignments.Where(a => a.Meets).Select(a => a.SkillName);
                return $"{i+1}. {c.FullName} — score {c.Score}/100 | exp:{c.Profile.YearsExperience ?? 0}a" +
                       $" | cumple:[{string.Join(",", matched)}]" +
                       (gaps.Any() ? $" | gaps:[{string.Join(",", gaps)}]" : "");
            }));

            var narrativePrompt = $@"Analiza este matching de candidatos para el proyecto ""{project.Name}"":

Requisitos obligatorios: {string.Join(", ", mandatoryReqs.Select(r => $"{r.SkillName} niv.{r.RequiredLevel}"))}
Requisitos opcionales: {string.Join(", ", optionalReqs.Select(r => $"{r.SkillName} niv.{r.RequiredLevel}"))}

Top candidatos (ya puntuados):
{topSummary}

Escribe en 3-5 líneas: ¿quién es el mejor candidato y por qué? ¿hay brechas críticas en el equipo?
Sé concreto, menciona nombres reales. No inventes datos.";

            string narrative;
            try
            {
                narrative = await _geminiService.QueryAsync(narrativePrompt, cancellationToken);
                // Limpiar markdown si viene con bloques de código
                narrative = narrative.Trim().Replace("```", "").Trim();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Gemini no pudo generar narrativa, usando resumen local");
                narrative = scoredCandidates.Count > 0
                    ? $"Mejor candidato: {scoredCandidates[0].FullName} con {scoredCandidates[0].Score}/100. " +
                      $"Total evaluados: {scoredCandidates.Count}."
                    : "No se encontraron candidatos adecuados.";
            }

            // ── 6. Construir respuesta final ───────────────────────────────────
            var candidateMatches = scoredCandidates.Select(c =>
            {
                var mandatoryMet = c.Alignments.Where(a => a.IsMandatory).Count(a => a.Meets);
                var totalMandatory = c.Alignments.Count(a => a.IsMandatory);
                var gaps = c.Alignments.Where(a => !a.Meets).Select(a =>
                    $"{a.SkillName}: tiene nivel {a.CurrentLevel}, requiere {a.RequiredLevel}").ToList();

                var reason = c.MandatoryMet
                    ? $"Cumple todos los requisitos obligatorios ({mandatoryMet}/{totalMandatory}). " +
                      $"Experiencia: {c.Profile.YearsExperience ?? 0} años."
                    : $"Cumple {mandatoryMet}/{totalMandatory} requisitos obligatorios. " +
                      (gaps.Any() ? $"Gaps: {string.Join("; ", gaps.Take(3))}." : "");

                return new CandidateMatch
                {
                    UserId               = c.Profile.UserId,
                    FullName             = c.FullName,
                    Email                = c.Email,
                    MatchScore           = c.Score,
                    SkillAlignments      = c.Alignments,
                    RecommendationReason = reason
                };
            }).ToList();

            var response = new SkillMatchResponse
            {
                ProjectId         = request.ProjectId,
                ProjectName       = project.Name,
                Candidates        = candidateMatches,
                AnalysisNarrative = narrative
            };

            await _agentRepository.CreateActionAsync(
                organizationId,
                "PROJECT_MATCHING",
                $"Matching para proyecto {project.Name}",
                JsonSerializer.Serialize(request),
                JsonSerializer.Serialize(new { candidateCount = candidateMatches.Count, topScore = candidateMatches.FirstOrDefault()?.MatchScore }),
                "SUCCESS");

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en matching de candidatos para proyecto {ProjectId}", request.ProjectId);
            throw;
        }
    }

    public async Task ApproveAgentActionAsync(
        Guid organizationId,
        Guid actionId,
        Guid approvedByUserId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Aprobando acción {ActionId} por usuario {UserId}",
            actionId, approvedByUserId);

        var action = await _agentRepository.GetActionByIdAsync(actionId, organizationId);

        if (action == null)
            throw new InvalidOperationException($"Acción {actionId} no encontrada");

        if (action.Status != "PENDING_APPROVAL")
            throw new InvalidOperationException($"Acción {actionId} no está pendiente de aprobación");

        await _agentRepository.UpdateActionStatusAsync(
            actionId, "APPROVED", approvedByUserId);

        _logger.LogInformation("Acción {ActionId} aprobada exitosamente", actionId);
    }

    public async Task RejectAgentActionAsync(
        Guid organizationId,
        Guid actionId,
        Guid rejectedByUserId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Rechazando acción {ActionId} por usuario {UserId}: {Reason}",
            actionId, rejectedByUserId, reason);

        var action = await _agentRepository.GetActionByIdAsync(actionId, organizationId);

        if (action == null)
            throw new InvalidOperationException($"Acción {actionId} no encontrada");

        if (action.Status != "PENDING_APPROVAL")
            throw new InvalidOperationException($"Acción {actionId} no está pendiente de aprobación");

        await _agentRepository.UpdateActionStatusAsync(
            actionId, "REJECTED", rejectedByUserId);

        _logger.LogInformation("Acción {ActionId} rechazada", actionId);
    }

    /// <summary>
    /// Detecta si la consulta no tiene relación con el dominio de gestión de talento/proyectos.
    /// Devuelve true cuando NINGUNA keyword de dominio aparece en la query.
    /// </summary>
    private static bool IsOffTopicQuery(string query)
    {
        var q = query.ToLowerInvariant();

        // Keywords que SÍ pertenecen al dominio — si alguna aparece, la query es válida
        var domainKeywords = new[]
        {
            // Talento / personas
            "empleado", "usuario", "candidato", "desarrollador", "equipo", "team", "persona", "perfil",
            "nombre", "contrat", "staff", "recurso", "talento",
            // Habilidades
            "habilidad", "skill", "competencia", "capacidad", "nivel", "tecnolog",
            // Proyectos
            "proyecto", "project", "asignacion", "asignación", "tarea", "sprint", "entregable",
            // Certificaciones
            "certificacion", "certificación", "certificado", "cert",
            // Postulaciones
            "postulacion", "postulación", "aplicacion", "aplicación", "postul", "aplicó",
            // Roles / permisos
            "rol", "role", "permiso", "acceso", "rbac", "admin", "manager",
            // Organización
            "organizacion", "organización", "empresa", "compañia", "compañía", "org",
            // Brechas / capacitación
            "brecha", "capacitacion", "capacitación", "entrenamiento", "training", "formacion", "formación",
            // Consultas reflexivas sobre el propio usuario
            "mis ", "mi ", " yo ", "para mí", "para mi", "que me ", "me recomi", "tengo",
            // Verbos típicos de consultas de negocio
            "cuantos", "cuántos", "cuales", "cuáles", "listar", "lista", "mostrar", "muestra",
            "analiza", "analizar", "recomienda", "recomendar", "busca", "buscar", "dame"
        };

        return !domainKeywords.Any(k => q.Contains(k));
    }

    private static string BuildSystemPrompt(Guid organizationId)
    {
        return $@"
Eres un Especialista en Orquestación de Talento para el sistema DevManager.

Tu misión es ayudar a optimizar la gestión de talento y asignación de proyectos mediante análisis inteligente de datos.

**Contexto de la organización:** {organizationId}

**Capacidades:**
- Validar coherencia de habilidades declaradas vs certificaciones
- Recomendar candidatos óptimos para proyectos
- Identificar brechas de capacitación
- Analizar tendencias de contribución
- Consultar postulaciones a proyectos y su estado
- Revisar roles y permisos RBAC de la organización
- Mostrar resúmenes generales de la organización

**Restricciones de seguridad:**
- Solo accedes a datos de la organización actual (multi-tenancy estricto)
- Respetas la privacidad de datos personales (Ley 1581 de 2012)
- Tus recomendaciones requieren aprobación humana para acciones críticas
- No tomas decisiones de contratación/despido

**Estilo de comunicación:**
- Profesional y basado en datos
- Explica tu razonamiento (Chain of Thought)
- Proporciona métricas cuantificables cuando sea posible
- Señala limitaciones y suposiciones

**REGLAS DE RESPUESTA CRÍTICAS:**
- Si los datos provistos SON suficientes: responde con información específica, nombres reales y métricas concretas.
- Si los datos NO son suficientes para responder la consulta exacta, responde EXACTAMENTE así:
  'No cuento con la información necesaria para responder esta consulta con los datos disponibles.'
  Luego sugiere qué sí puedes responder: 'Puedo ayudarte con: habilidades del equipo, proyectos activos, perfiles de empleados, certificaciones, postulaciones a proyectos, roles y permisos, o un resumen general de la organización.'
- NUNCA inventes datos, nombres o métricas que no estén en los datos proporcionados.
- NUNCA uses placeholders como [nombre], [cantidad], [skill].

Responde siempre de manera estructurada y accionable.";
    }
}