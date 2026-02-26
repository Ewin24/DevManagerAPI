using Application.Common.Models;
using Application.DTOs.Agent;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador del Agente de Orquestación de Talento (AI-powered)
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class AgentController : ControllerBase
{
    private readonly IAgentService _agentService;
    private readonly ILogger<AgentController> _logger;

    public AgentController(
        IAgentService agentService,
        ILogger<AgentController> logger)
    {
        _agentService = agentService;
        _logger = logger;
    }

    /// <summary>
    /// Consulta general al agente IA en lenguaje natural
    /// </summary>
    /// <remarks>
    /// Permite hacer preguntas al agente sobre talento, proyectos, skills, etc. El agente responderá con análisis inteligente y recomendaciones usando Google Gemini.
    /// 
    /// **Ejemplos de Queries - Consultas Organizacionales:**
    /// 
    ///     POST /agent/query
    ///     {
    ///         "query": "¿Cuántos desarrolladores tenemos con Java nivel 4 o superior?",
    ///         "requireApproval": false
    ///     }
    /// 
    ///     POST /agent/query
    ///     {
    ///         "query": "Analiza las brechas de capacitación en el equipo de frontend",
    ///         "requireApproval": false
    ///     }
    /// 
    ///     POST /agent/query
    ///     {
    ///         "query": "¿Qué skills están más demandadas en los proyectos activos?",
    ///         "requireApproval": false
    ///     }
    /// 
    /// **Ejemplos de Queries - Contexto del Usuario Actual:**
    /// El agente ahora puede responder preguntas personalizadas sobre TI usando pronombres como "yo", "mi", "mis". No necesitas especificar tu userId.
    /// 
    ///     POST /agent/query
    ///     {
    ///         "query": "¿Qué habilidades me recomiendan aprender?",
    ///         "requireApproval": false
    ///     }
    /// 
    ///     POST /agent/query
    ///     {
    ///         "query": "¿Qué proyectos encajan con mis habilidades?",
    ///         "requireApproval": false
    ///     }
    /// 
    ///     POST /agent/query
    ///     {
    ///         "query": "Analiza mi perfil y dime en qué proyectos puedo contribuir",
    ///         "requireApproval": false
    ///     }
    /// 
    ///     POST /agent/query
    ///     {
    ///         "query": "Dame recomendaciones para mejorar mi carrera profesional",
    ///         "requireApproval": false
    ///     }
    /// 
    /// **Detección Automática de Contexto:**
    /// El sistema detecta automáticamente cuando la consulta se refiere al usuario actual mediante pronombres:
    /// - "yo", "mi", "mis", "mí"
    /// - "para mí", "me recomi", "me适合" (chino)
    /// 
    /// Cuando se detecta, el agente obtiene automáticamente:
    /// - Perfil del empleado (bio, años de experiencia)
    /// - Habilidades declaradas con niveles
    /// - Certificaciones obtenidas
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Consulta procesada exitosamente",
    ///         "data": {
    ///             "answer": "Tenemos 2 desarrolladores con Java nivel 4+:\n\n1. Juan Martínez (Java: 4, Spring Boot: 3)\n2. Ana López (Java: 5, Spring Boot: 5, PostgreSQL: 4)\n\nAna López es el perfil más senior en Java dentro de la organización.",
    ///             "reasoning": "Analicé la tabla talent.EmployeeSkills filtrando por OrganizationId y skillName='Java' con level >= 4. Encontré 2 matches.",
    ///             "requiresApproval": false,
    ///             "actionId": null,
    ///             "confidence": 95
    ///         }
    ///     }
    /// 
    /// **Parámetro requireApproval:**
    /// - false (default): Consulta de solo lectura, respuesta inmediata
    /// - true: Si el agente determina que requiere acción crítica, retorna actionId para flujo HITL
    /// 
    /// **Notas importantes:**
    /// - El agente SOLO accede a datos de la OrganizationId del JWT (multi-tenancy)
    /// - Cada consulta se registra en reporting.AgentActions (auditoría)
    /// - El campo reasoning muestra el proceso de razonamiento del agente (Chain of Thought)
    /// - El userId se extrae automáticamente del token JWT para consultas personales
    /// 
    /// **Casos de uso:**
    /// - Consultas ad-hoc sobre talento
    /// - Análisis de brechas de skills
    /// - Reportes conversacionales
    /// - Recomendaciones personalizadas al usuario actual
    /// - Análisis de compatibilidad con proyectos
    /// - Recomendaciones de capacitación
    /// </remarks>
    /// <param name="request">Consulta en lenguaje natural con opción de requerir aprobación</param>
    /// <response code="200">Consulta procesada exitosamente con respuesta del agente</response>
    /// <response code="401">No autenticado</response>
    [HttpPost("query")]
    [ProducesResponseType(typeof(ApiResponse<AgentQueryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Query([FromBody] AgentQueryRequest request)
    {
        var organizationId = GetOrganizationId();
        var userId = GetUserId();

        _logger.LogInformation(
            "Usuario {UserId} consultando agente: {Query}",
            userId, request.Query);

        var response = await _agentService.QueryAsync(organizationId, userId, request);

        return Ok(new ApiResponse<AgentQueryResponse>
        {
            Success = true,
            Message = "Consulta procesada exitosamente",
            Data = response
        });
    }

    /// <summary>
    /// Validación semántica de habilidades con evidencia usando IA
    /// </summary>
    /// <remarks>
    /// Valida si el nivel declarado de una habilidad es coherente con la experiencia y evidencia proporcionada usando análisis de Google Gemini.
    /// 
    /// **Criterios de Validación:**
    /// - Coherencia entre años de experiencia y nivel declarado
    /// - Análisis semántico de evidencia (si se proporciona URL)
    /// - Análisis de descripción textual de experiencia
    /// - Comparación con perfiles similares en la organización
    /// - Detección de patrones avanzados vs básicos
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     POST /agent/validate-skill
    ///     {
    ///         "userId": "11111111-0000-0000-0000-000000000003",
    ///         "skillId": "aaaaaaaa-0000-0000-0000-000000000001",
    ///         "level": 5,
    ///         "evidenceUrl": "https://github.com/juan/dotnet-microservices-framework",
    ///         "experienceDescription": "He desarrollado aplicaciones enterprise con .NET Core por más de 5 años, incluyendo microservicios y arquitecturas serverless. He liderado equipos de 10+ desarrolladores y definido estándares de código..."
    ///     }
    /// 
    /// **Ejemplo de Response (200 OK - Válido):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Skill validado exitosamente",
    ///         "data": {
    ///             "isValid": true,
    ///             "confidence": 92,
    ///             "reasoning": "El nivel 5 declarado es coherente con:\n- 5+ años de experiencia en el campo\n- Evidencia de repositorio con framework completo de microservicios\n- Liderazgo de equipo mencionado en la descripción\n- Patrones avanzados: CQRS, Event Sourcing, DDD",
    ///             "recommendations": [
    ///                 "Considerar certificación Microsoft Certified: Azure Solutions Architect Expert",
    ///                 "Potencial mentor para desarrolladores junior en .NET"
    ///             ]
    ///         }
    ///     }
    /// 
    /// **Ejemplo de Response (200 OK - Requiere Revisión):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Skill requiere revisión",
    ///         "data": {
    ///             "isValid": false,
    ///             "confidence": 45,
    ///             "reasoning": "El nivel 5 (Experto) parece sobreestimado:\n- Solo 2 años de experiencia\n- Evidencia muestra proyectos básicos CRUD\n- Falta documentación técnica",
    ///             "recommendations": [
    ///                 "Solicitar evidencia adicional de proyectos complejos",
    ///                 "Considerar nivel 3-4 como más apropiado"
    ///             ]
    ///         }
    ///     }
    /// 
    /// **Notas importantes:**
    /// - NO modifica datos automáticamente
    /// - Solo proporciona análisis y recomendaciones
    /// - El manager debe hacer la validación final: PUT /api/employees/skills/{id}/validate
    /// 
    /// **Casos de uso:**
    /// - Validación automática durante onboarding
    /// - Segunda opinión para managers
    /// - Detección de sobreestimación/subestimación
    /// - Pre-filtro antes de validación humana
    /// </remarks>
    /// <param name="request">Datos de la habilidad a validar (userId, skillId, nivel, evidencia, años de experiencia)</param>
    /// <response code="200">Validación completada con resultado y recomendaciones</response>
    /// <response code="401">No autenticado</response>
    [HttpPost("validate-skill")]
    [ProducesResponseType(typeof(ApiResponse<SkillValidationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateSkill([FromBody] SkillValidationRequest request)
    {
        var organizationId = GetOrganizationId();

        _logger.LogInformation(
            "Validando skill {SkillId} nivel {Level} para usuario {UserId}",
            request.SkillId, request.Level, request.UserId);

        var response = await _agentService.ValidateSkillAsync(organizationId, request);

        return Ok(new ApiResponse<SkillValidationResponse>
        {
            Success = true,
            Message = response.IsValid
                ? "Skill validado exitosamente"
                : "Skill requiere revisión",
            Data = response
        });
    }

    /// <summary>
    /// Matching inteligente de candidatos para un proyecto usando IA
    /// </summary>
    /// <remarks>
    /// Analiza todos los empleados disponibles y calcula un score de matching (0-100) basado en múltiples criterios. Usa Google Gemini para análisis avanzado.
    /// 
    /// **Algoritmo de Matching:**
    /// 
    /// 1. **Mandatory Skills (60% del score):**
    ///    - Debe tener TODAS las skills obligatorias (isMandatory = true)
    ///    - Nivel >= requiredLevel
    ///    - Si falta alguna → score máximo 40%
    /// 
    /// 2. **Optional Skills (20% del score):**
    ///    - Skills deseables (isMandatory = false)
    ///    - Bonus por tenerlas al nivel requerido
    /// 
    /// 3. **Experience (10% del score):**
    ///    - Años de experiencia del empleado
    ///    - Más años = mayor score
    /// 
    /// 4. **Skill Level Surplus (10% del score):**
    ///    - Bonus si el nivel supera el requerido
    ///    - Ejemplo: Si requiere C#:4 y tiene C#:5 → bonus
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     POST /agent/match-candidates
    ///     {
    ///         "projectId": "eeeeeeee-0000-0000-0000-000000000001",
    ///         "requireApproval": true,
    ///         "minScore": 70
    ///     }
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Se encontraron 3 candidatos",
    ///         "data": {
    ///             "projectId": "eeeeeeee-0000-0000-0000-000000000001",
    ///             "projectName": "Sistema de Gestión Hospitalaria",
    ///             "totalCandidates": 3,
    ///             "requiresApproval": true,
    ///             "actionId": "kkkkkkkk-0000-0000-0000-000000000001",
    ///             "candidates": [
    ///                 {
    ///                     "userId": "11111111-0000-0000-0000-000000000004",
    ///                     "fullName": "Ana López",
    ///                     "matchScore": 95,
    ///                     "matchDetails": {
    ///                         "mandatorySkillsMatch": "4/4",
    ///                         "optionalSkillsMatch": "2/3",
    ///                         "averageSkillLevel": 4.5,
    ///                         "yearsExperience": 7
    ///                     },
    ///                     "strengths": [
    ///                         "Cumple todos los requisitos obligatorios",
    ///                         "Nivel experto en Java (5) y Spring Boot (5)"
    ///                     ],
    ///                     "gaps": [
    ///                         "No tiene skill en Docker (requerido nivel 3)"
    ///                     ],
    ///                     "recommendation": "HIGHLY RECOMMENDED - Candidata ideal"
    ///                 }
    ///             ],
    ///             "summary": {
    ///                 "bestMatch": "Ana López (95% match)",
    ///                 "averageScore": 82.67,
    ///                 "candidatesAbove90": 1
    ///             }
    ///         }
    ///     }
    /// 
    /// **Parámetros:**
    /// - minScore: Filtra candidatos con score >= minScore (default: 50, recomendado: 70 para proyectos críticos)
    /// - requireApproval: false (default) = solo retorna análisis; true = retorna actionId para aprobación HITL
    /// 
    /// **Casos de uso:**
    /// - Búsqueda automática de candidatos para proyecto
    /// - Recomendaciones inteligentes para asignación
    /// - Identificación de brechas de capacitación
    /// - Planificación de recursos
    /// </remarks>
    /// <param name="request">Datos del proyecto y configuración de matching (projectId, requireApproval, minScore)</param>
    /// <response code="200">Matching completado con candidatos ordenados por score</response>
    /// <response code="401">No autenticado</response>
    [HttpPost("match-candidates")]
    [ProducesResponseType(typeof(ApiResponse<SkillMatchResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MatchCandidates([FromBody] SkillMatchRequest request)
    {
        var organizationId = GetOrganizationId();

        _logger.LogInformation(
            "Matching candidatos para proyecto {ProjectId}",
            request.ProjectId);

        var response = await _agentService.MatchCandidatesForProjectAsync(
            organizationId, request);

        return Ok(new ApiResponse<SkillMatchResponse>
        {
            Success = true,
            Message = $"Se encontraron {response.Candidates.Count} candidatos",
            Data = response
        });
    }

    /// <summary>
    /// Aprobar una acción del agente (HITL - Human In The Loop)
    /// </summary>
    /// <remarks>
    /// Permite aprobar acciones críticas del agente que requieren validación humana, como asignaciones de proyectos, cambios de nivel de skills, etc.
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     POST /agent/approve/kkkkkkkk-0000-0000-0000-000000000001
    ///     Authorization: Bearer {token}
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Acción aprobada exitosamente",
    ///         "data": null
    ///     }
    /// 
    /// **Estado Actual (v1.0):**
    /// - ✓ Actualiza estado en reporting.AgentActions (Status: PENDING_APPROVAL → APPROVED)
    /// - ✓ Registra ApprovedByUserId y ApprovedAt
    /// - ✗ NO ejecuta acciones automáticamente (ver HITL_WORKFLOW_GUIDE.md)
    /// 
    /// **Flujo completo:**
    /// 1. Agente retorna recomendación con requiresApproval: true y actionId
    /// 2. Manager revisa la recomendación
    /// 3. Manager aprueba: POST /agent/approve/{actionId}
    /// 4. Sistema registra aprobación para auditoría
    /// 
    /// **Nota importante:**
    /// La implementación actual es SOLO auditoría. La ejecución automática post-aprobación está en el roadmap (Q2 2026). 
    /// Por ahora, tras aprobar, el manager debe ejecutar manualmente la acción recomendada (ej: crear assignment, actualizar skill level).
    /// 
    /// **Casos de uso:**
    /// - Aprobar asignación recomendada por agente
    /// - Confirmar cambios sugeridos en niveles de skills
    /// - Validar recomendaciones críticas
    /// </remarks>
    /// <param name="actionId">ID de la acción del agente a aprobar (GUID)</param>
    /// <response code="200">Acción aprobada y registrada en auditoría</response>
    /// <response code="404">Acción no encontrada</response>
    /// <response code="401">No autenticado</response>
    /// <response code="403">Sin permisos (requiere rol Manager o Admin)</response>
    [HttpPost("approve/{actionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ApproveAction(Guid actionId)
    {
        var organizationId = GetOrganizationId();
        var userId = GetUserId();

        _logger.LogInformation(
            "Usuario {UserId} aprobando acción {ActionId}",
            userId, actionId);

        await _agentService.ApproveAgentActionAsync(organizationId, actionId, userId);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Acción aprobada exitosamente",
            Data = null
        });
    }

    /// <summary>
    /// Rechazar una acción del agente con motivo
    /// </summary>
    /// <remarks>
    /// Permite rechazar recomendaciones del agente proporcionando un motivo para feedback y mejora continua.
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     POST /agent/reject/kkkkkkkk-0000-0000-0000-000000000001
    ///     {
    ///         "reason": "El candidato ya está asignado a otro proyecto de alta prioridad. Considerar segundo mejor match."
    ///     }
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Acción rechazada",
    ///         "data": null
    ///     }
    /// 
    /// **Comportamiento:**
    /// - Actualiza estado: PENDING_APPROVAL → REJECTED
    /// - Registra RejectedByUserId, RejectedAt y Reason
    /// - El motivo se usa para análisis y mejora del agente
    /// 
    /// **Métricas HITL:**
    /// Puedes consultar la tasa de aprobación con:
    /// ```sql
    /// SELECT 
    ///     COUNT(CASE WHEN Status = 'APPROVED' THEN 1 END) * 100.0 / COUNT(*) as ApprovalRate
    /// FROM reporting.AgentActions
    /// WHERE Status IN ('APPROVED', 'REJECTED');
    /// ```
    /// 
    /// **Casos de uso:**
    /// - Rechazar recomendación incorrecta
    /// - Feedback al agente para mejora
    /// - Documentar decisiones que difieren de la IA
    /// </remarks>
    /// <param name="actionId">ID de la acción del agente a rechazar (GUID)</param>
    /// <param name="request">Motivo del rechazo para análisis y mejora</param>
    /// <response code="200">Acción rechazada y motivo registrado</response>
    /// <response code="404">Acción no encontrada</response>
    /// <response code="401">No autenticado</response>
    /// <response code="403">Sin permisos (requiere rol Manager o Admin)</response>
    [HttpPost("reject/{actionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectAction(
        Guid actionId,
        [FromBody] RejectActionRequest request)
    {
        var organizationId = GetOrganizationId();
        var userId = GetUserId();

        _logger.LogInformation(
            "Usuario {UserId} rechazando acción {ActionId}: {Reason}",
            userId, actionId, request.Reason);

        await _agentService.RejectAgentActionAsync(
            organizationId, actionId, userId, request.Reason);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Acción rechazada",
            Data = null
        });
    }

    private Guid GetOrganizationId()
    {
        var claim = User.FindFirst("OrganizationId")?.Value;
        return Guid.Parse(claim!);
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(claim!);
    }
}

public record RejectActionRequest(string Reason);
