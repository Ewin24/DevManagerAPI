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
    private readonly ILogger<AgentService> _logger;

    public AgentService(
        IGeminiService geminiService,
        IProfileService profileService,
        ISkillService skillService,
        IEmployeeSkillService employeeSkillService,
        IProjectService projectService,
        IUserService userService,
        IAgentRepository agentRepository,
        ILogger<AgentService> logger)
    {
        _geminiService = geminiService;
        _profileService = profileService;
        _skillService = skillService;
        _employeeSkillService = employeeSkillService;
        _projectService = projectService;
        _userService = userService;
        _agentRepository = agentRepository;
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
            contextData.ContainsKey("Projects"))
        {
            if (responseLower.Contains("tabla") || responseLower.Contains("tabla de") ||
                responseLower.Contains("nivel") || responseLower.Contains("empleado"))
            {
                return "table";
            }
            if (responseLower.Contains("lista") || responseLower.Contains("listado") ||
                responseLower.Contains("recomendaciones"))
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

        if (queryLower.Contains("habilidad") || queryLower.Contains("skill"))
        {
            actions.Add(new SuggestedAction
            {
                Label = "Ver empleados con esta habilidad",
                Query = "dame la lista de empleados por habilidad"
            });
            actions.Add(new SuggestedAction
            {
                Label = "Estadísticas de skills",
                Query = "muéstrame las estadísticas de habilidades"
            });
        }

        if (queryLower.Contains("proyecto") || queryLower.Contains("proyect"))
        {
            actions.Add(new SuggestedAction
            {
                Label = "Buscar candidatos",
                Query = "busca candidatos para este proyecto"
            });
        }

        if (!queryLower.Contains("mi perfil") && !queryLower.Contains("mis habilidades"))
        {
            actions.Add(new SuggestedAction
            {
                Label = "Mi perfil",
                Query = "muéstrame mi perfil"
            });
        }

        if (actions.Count == 0)
        {
            actions.Add(new SuggestedAction
            {
                Label = "Más detalles",
                Query = "dame más información"
            });
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

    public async Task<SkillMatchResponse> MatchCandidatesForProjectAsync(
        Guid organizationId,
        SkillMatchRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Matching candidatos para proyecto {ProjectId}", request.ProjectId);

        try
        {
            // 1. Obtener proyecto y requisitos
            var project = await _projectService.GetProjectByIdAsync(
                request.ProjectId, organizationId);

            if (project == null)
            {
                throw new InvalidOperationException($"Proyecto {request.ProjectId} no encontrado");
            }

            var requirements = await _projectService.GetSkillRequirementsAsync(
                request.ProjectId, organizationId);

            // 2. Obtener todos los perfiles con skills
            var profiles = await _profileService.GetAllProfilesWithSkillsAsync(organizationId);

            // 3. Obtener información de usuarios para completar los datos
            var userIds = profiles.Select(p => p.UserId).ToList();
            var users = new Dictionary<Guid, (string FullName, string Email)>();

            // Obtener información de todos los usuarios en batch
            try
            {
                var allUsers = await _userService.GetAllAsync(organizationId);
                foreach (var user in allUsers)
                {
                    users[user.Id] = ($"{user.FirstName} {user.LastName}", user.Email);
                }
                _logger.LogInformation("Se obtuvieron {Count} usuarios para matching", users.Count);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo obtener la lista de usuarios, intentando uno por uno");

                // Fallback: obtener uno por uno
                foreach (var userId in userIds)
                {
                    try
                    {
                        var user = await _userService.GetByIdAsync(userId, organizationId);
                        if (user != null)
                        {
                            users[userId] = ($"{user.FirstName} {user.LastName}", user.Email);
                        }
                    }
                    catch
                    {
                        // Silenciar errores individuales para continuar con otros usuarios
                        _logger.LogDebug("Usuario {UserId} no encontrado", userId);
                    }
                }
            }

            // Construir estructura de candidatos para el análisis
            var candidates = profiles.Select(p => new
            {
                userId = p.UserId,
                fullName = users.ContainsKey(p.UserId) ? users[p.UserId].FullName : "Usuario desconocido",
                email = users.ContainsKey(p.UserId) ? users[p.UserId].Email : "",
                bio = p.Bio ?? "Sin bio",
                yearsExperience = p.YearsExperience ?? 0,
                skills = p.Skills.Select(s => new
                {
                    skillName = s.SkillName,
                    level = s.CurrentLevel,
                    validated = s.LastValidatedAt.HasValue
                }).ToList()
            }).ToList();

            // Si no hay candidatos, retornar lista vacía
            if (!candidates.Any())
            {
                _logger.LogWarning("No se encontraron candidatos con skills en la organización");
                return new SkillMatchResponse
                {
                    ProjectId = request.ProjectId,
                    ProjectName = project.Name,
                    Candidates = new List<CandidateMatch>(),
                    AnalysisNarrative = "No se encontraron candidatos con habilidades registradas en esta organización."
                };
            }

            // 3. Construir prompt para matching inteligente
            var matchingPrompt = $@"
Actúa como un experto en matching de talento para proyectos tecnológicos.

**Proyecto:** {project.Name}
**Descripción:** {project.Description}
**Complejidad:** {project.ComplexityLevel}/3

**Requisitos de habilidades:**
{string.Join("\n", requirements.Select(r =>
    $"- {r.SkillName}: Nivel {r.RequiredLevel}/5 {(r.IsMandatory ? "(OBLIGATORIO)" : "(opcional)")}"))}

**Candidatos disponibles:**
{JsonSerializer.Serialize(candidates, new JsonSerializerOptions { WriteIndented = true })}

Analiza y calcula un match score (0-100) para cada candidato considerando:
1. Cumplimiento de skills obligatorias
2. Nivel de proficiencia en cada skill
3. Años de experiencia
4. Certificaciones relevantes
5. Historial de contribución en proyectos similares

Retorna los TOP {request.MaxCandidates ?? 10} candidatos en formato JSON:
{{
    ""candidates"": [
        {{
            ""userId"": ""guid"",
            ""fullName"": ""nombre"",
            ""email"": ""email"",
            ""matchScore"": 0-100,
            ""skillAlignments"": [
                {{
                    ""skillName"": ""nombre"",
                    ""requiredLevel"": 1-5,
                    ""currentLevel"": 1-5,
                    ""isMandatory"": true/false,
                    ""meets"": true/false
                }}
            ],
            ""recommendationReason"": ""explicación del por qué es buen candidato""
        }}
    ],
    ""analysisNarrative"": ""Narrativa general del análisis""
}}

Ordena por matchScore descendente.";

            // Llamar a Gemini con el prompt de matching
            var geminiResponse = await _geminiService.QueryAsync(matchingPrompt, cancellationToken);

            // Limpiar respuesta de markdown code blocks
            var cleanJson = geminiResponse.Trim()
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            // Intentar extraer JSON si está embebido en texto
            if (!cleanJson.StartsWith("{"))
            {
                var jsonStart = cleanJson.IndexOf("{");
                var jsonEnd = cleanJson.LastIndexOf("}");
                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    cleanJson = cleanJson.Substring(jsonStart, jsonEnd - jsonStart + 1);
                }
            }

            _logger.LogInformation("JSON limpio de Gemini ({Length} caracteres): {Json}", cleanJson.Length, cleanJson.Substring(0, Math.Min(500, cleanJson.Length)));

            // Deserializar la respuesta completa
            Dictionary<string, JsonElement>? matchResult;
            try
            {
                matchResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                    cleanJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializando JSON de Gemini. JSON: {Json}", cleanJson);
                throw new InvalidOperationException($"No se pudo deserializar la respuesta de Gemini: {ex.Message}");
            }

            if (matchResult == null)
            {
                throw new InvalidOperationException("Gemini devolvió null");
            }

            // Parsear candidatos con manejo de errores robusto
            List<CandidateMatch> candidatesList = new();
            if (matchResult.TryGetValue("candidates", out var candidatesElement))
            {
                _logger.LogDebug("Candidates JSON: {CandidatesJson}", candidatesElement.GetRawText());

                try
                {
                    // Intentar deserialización directa
                    candidatesList = JsonSerializer.Deserialize<List<CandidateMatch>>(
                        candidatesElement.GetRawText(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<CandidateMatch>();
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "No se pudo deserializar candidatos con el formato esperado. Intentando parseo manual...");

                    // Parseo manual como fallback
                    if (candidatesElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var candElement in candidatesElement.EnumerateArray())
                        {
                            try
                            {
                                var userId = candElement.TryGetProperty("userId", out var userIdProp)
                                    ? Guid.Parse(userIdProp.GetString() ?? Guid.Empty.ToString())
                                    : Guid.Empty;

                                var fullName = candElement.TryGetProperty("fullName", out var nameProp)
                                    ? nameProp.GetString() ?? "Unknown"
                                    : "Unknown";

                                var email = candElement.TryGetProperty("email", out var emailProp)
                                    ? emailProp.GetString() ?? ""
                                    : "";

                                var matchScore = candElement.TryGetProperty("matchScore", out var scoreProp)
                                    ? (scoreProp.ValueKind == JsonValueKind.Number ? scoreProp.GetDouble() : 0)
                                    : 0;

                                var reason = candElement.TryGetProperty("recommendationReason", out var reasonProp)
                                    ? reasonProp.GetString() ?? "Sin razón especificada"
                                    : "Sin razón especificada";

                                // Parsear alignments
                                var alignments = new List<SkillAlignment>();
                                if (candElement.TryGetProperty("skillAlignments", out var alignmentsElement)
                                    && alignmentsElement.ValueKind == JsonValueKind.Array)
                                {
                                    foreach (var alignElement in alignmentsElement.EnumerateArray())
                                    {
                                        alignments.Add(new SkillAlignment
                                        {
                                            SkillName = alignElement.TryGetProperty("skillName", out var sn) ? sn.GetString() ?? "" : "",
                                            RequiredLevel = alignElement.TryGetProperty("requiredLevel", out var rl) ? rl.GetInt32() : 0,
                                            CurrentLevel = alignElement.TryGetProperty("currentLevel", out var cl) ? cl.GetInt32() : 0,
                                            IsMandatory = alignElement.TryGetProperty("isMandatory", out var im) && im.GetBoolean(),
                                            Meets = alignElement.TryGetProperty("meets", out var m) && m.GetBoolean()
                                        });
                                    }
                                }

                                candidatesList.Add(new CandidateMatch
                                {
                                    UserId = userId,
                                    FullName = fullName,
                                    Email = email,
                                    MatchScore = matchScore,
                                    RecommendationReason = reason,
                                    SkillAlignments = alignments
                                });
                            }
                            catch (Exception parseEx)
                            {
                                _logger.LogWarning(parseEx, "Error parseando candidato individual");
                            }
                        }
                    }
                }
            }
            else
            {
                _logger.LogWarning("No se encontró la propiedad 'candidates' en la respuesta de Gemini");
            }

            // Parsear narrative
            var narrative = "Análisis de matching completado";
            if (matchResult.TryGetValue("analysisNarrative", out var narrativeElement))
            {
                narrative = narrativeElement.GetString() ?? narrative;
            }

            var response = new SkillMatchResponse
            {
                ProjectId = request.ProjectId,
                ProjectName = project.Name,
                Candidates = candidatesList ?? new List<CandidateMatch>(),
                AnalysisNarrative = narrative
            };

            // Registrar matching
            await _agentRepository.CreateActionAsync(
                organizationId,
                "PROJECT_MATCHING",
                $"Matching para proyecto {project.Name}",
                JsonSerializer.Serialize(request),
                JsonSerializer.Serialize(response),
                "SUCCESS");

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en matching de candidatos");
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

Responde siempre de manera estructurada y accionable.";
    }
}