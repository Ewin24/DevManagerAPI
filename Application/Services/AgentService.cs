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
    private readonly IAgentRepository _agentRepository;
    private readonly ILogger<AgentService> _logger;

    public AgentService(
        IGeminiService geminiService,
        IProfileService profileService,
        ISkillService skillService,
        IEmployeeSkillService employeeSkillService,
        IProjectService projectService,
        IAgentRepository agentRepository,
        ILogger<AgentService> logger)
    {
        _geminiService = geminiService;
        _profileService = profileService;
        _skillService = skillService;
        _employeeSkillService = employeeSkillService;
        _projectService = projectService;
        _agentRepository = agentRepository;
        _logger = logger;
    }

    public async Task<AgentQueryResponse> QueryAsync(
        Guid organizationId,
        AgentQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Procesando query del agente para org {OrgId}: {Query}",
            organizationId, request.Query);

        var toolsExecuted = new List<ToolExecutionResult>();

        try
        {
            // Construir prompt con contexto del sistema
            var systemPrompt = BuildSystemPrompt(organizationId);
            var fullPrompt = $"{systemPrompt}\n\nConsulta del usuario: {request.Query}";

            if (!string.IsNullOrEmpty(request.Context))
            {
                fullPrompt += $"\n\nContexto adicional: {request.Context}";
            }

            // Obtener respuesta con razonamiento (Chain of Thought)
            var (response, reasoning) = await _geminiService.QueryWithReasoningAsync(
                fullPrompt, cancellationToken);

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
                Response = response,
                ReasoningSteps = reasoning,
                ToolsExecuted = toolsExecuted,
                RequiresHumanApproval = request.RequireApproval,
                ActionId = actionId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando query del agente");
            throw;
        }
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
            
            // Construir estructura de candidatos para el análisis
            var candidates = profiles.Select(p => new
            {
                userId = p.UserId,
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

            var matchResult = await _geminiService.AnalyzeStructuredDataAsync<Dictionary<string, object>>(
                matchingPrompt,
                new { project, requirements, candidates },
                cancellationToken);

            // Parsear resultado
            var candidatesList = JsonSerializer.Deserialize<List<CandidateMatch>>(
                matchResult["candidates"].ToString()!);

            var narrative = matchResult["analysisNarrative"].ToString() 
                ?? "Análisis de matching completado";

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