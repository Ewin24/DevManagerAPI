namespace API.Controllers;

using Application.Common.Exceptions;
using Application.Common.Models;
using Application.DTOs.Skills;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/// <summary>
/// Controller para gestión de habilidades de empleados
/// </summary>
[ApiController]
[Route("api/employees")]
[Authorize]
public class EmployeeSkillsController : ControllerBase
{
    private readonly IEmployeeSkillService _employeeSkillService;
    private readonly ILogger<EmployeeSkillsController> _logger;

    public EmployeeSkillsController(
        IEmployeeSkillService employeeSkillService, 
        ILogger<EmployeeSkillsController> logger)
    {
        _employeeSkillService = employeeSkillService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las habilidades de un empleado específico
    /// </summary>
    /// <remarks>
    /// Retorna el perfil técnico completo del empleado incluyendo nivel de proficiencia, evidencia y validaciones.
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     GET /api/employees/11111111-0000-0000-0000-000000000003/skills
    ///     Authorization: Bearer {token}
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": null,
    ///         "data": [
    ///             {
    ///                 "id": "dddddddd-0000-0000-0000-000000000001",
    ///                 "userId": "11111111-0000-0000-0000-000000000003",
    ///                 "skillId": "aaaaaaaa-0000-0000-0000-000000000001",
    ///                 "skillName": "C#",
    ///                 "level": 5,
    ///                 "evidenceUrl": "https://github.com/juan/dotnet-core",
    ///                 "lastValidatedAt": "2025-12-01T10:00:00Z",
    ///                 "validatedByUserId": "11111111-0000-0000-0000-000000000002",
    ///                 "validatedByName": "María García"
    ///             }
    ///         ]
    ///     }
    /// 
    /// **Niveles de Proficiencia:**
    /// - 1 = Básico (conocimiento teórico)
    /// - 2 = Intermedio (puede trabajar con supervisión)
    /// - 3 = Competente (trabajo autónomo)
    /// - 4 = Avanzado (puede enseñar a otros)
    /// - 5 = Experto (referente técnico)
    /// 
    /// **Casos de uso:**
    /// - Vista detallada de perfil técnico
    /// - Evaluación de candidatos para proyectos
    /// - Dashboard de skills del equipo
    /// </remarks>
    /// <param name="id">ID del usuario/empleado</param>
    /// <returns>Lista de habilidades del empleado</returns>
    /// <response code="200">Habilidades obtenidas exitosamente</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("{id}/skills")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeSkillResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeeSkills(Guid id)
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var skills = await _employeeSkillService.GetEmployeeSkillsAsync(id, organizationId);

        return Ok(ApiResponse<IEnumerable<EmployeeSkillResponse>>.SuccessResponse(skills));
    }

    /// <summary>
    /// Crea o actualiza una habilidad del empleado autenticado (auto-declaración)
    /// </summary>
    /// <remarks>
    /// Permite al usuario declarar sus habilidades con nivel y evidencia. Si la skill ya existe, la actualiza (upsert).
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     POST /api/employees/skills
    ///     {
    ///         "skillId": "aaaaaaaa-0000-0000-0000-000000000008",
    ///         "level": 3,
    ///         "evidenceUrl": "https://github.com/myuser/kubernetes-project"
    ///     }
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Habilidad guardada exitosamente",
    ///         "data": "Habilidad guardada exitosamente"
    ///     }
    /// 
    /// **Validaciones:**
    /// - skillId: Debe existir en el catálogo
    /// - level: Rango válido 1-5
    /// - evidenceUrl: URL válida (opcional)
    /// 
    /// **Comportamiento (Upsert):**
    /// - Si la skill NO existe para el usuario → CREA (INSERT)
    /// - Si la skill ya existe → ACTUALIZA nivel y evidenceUrl (UPDATE)
    /// 
    /// **Nota importante:**
    /// - El usuario solo puede gestionar SUS PROPIAS skills
    /// - UserId se extrae del JWT automáticamente
    /// - La validación (LastValidatedAt, ValidatedByUserId) queda NULL hasta que un manager valide
    /// 
    /// **Casos de uso:**
    /// - Auto-declaración de skills durante onboarding
    /// - Actualización tras completar capacitación
    /// - Agregar evidencia de proyectos personales
    /// </remarks>
    /// <param name="request">Datos de la habilidad (skillId, nivel, evidencia)</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="200">Habilidad guardada exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="404">Skill no encontrada en el catálogo</response>
    /// <response code="401">No autenticado</response>
    [HttpPost("skills")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpsertSkill([FromBody] UpsertEmployeeSkillRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        try
        {
            var result = await _employeeSkillService.UpsertEmployeeSkillAsync(userId, organizationId, request);

            if (!result)
            {
                return BadRequest();
            }

            return Ok(ApiResponse<object>.SuccessResponse("Habilidad guardada exitosamente"));
        }
        catch (NotFoundException ex)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Valida una habilidad de un empleado (solo managers/admins)
    /// </summary>
    /// <remarks>
    /// Permite a un manager/admin validar la habilidad declarada por un empleado, opcionalmente ajustando el nivel si es necesario.
    /// 
    /// **Ejemplo de Request - Validar sin cambiar nivel:**
    /// 
    ///     PUT /api/employees/skills/{id}/validate
    ///     {
    ///         "newLevel": null
    ///     }
    /// 
    /// **Ejemplo de Request - Validar y ajustar nivel:**
    /// 
    ///     PUT /api/employees/skills/{id}/validate
    ///     {
    ///         "newLevel": 3
    ///     }
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Habilidad validada exitosamente",
    ///         "data": "Habilidad validada exitosamente"
    ///     }
    /// 
    /// **Comportamiento:**
    /// - Establece LastValidatedAt = SYSUTCDATETIME()
    /// - Establece ValidatedByUserId = {userId del JWT}
    /// - Si se proporciona newLevel, actualiza el nivel
    /// - Si newLevel es null, solo valida sin modificar nivel
    /// 
    /// **Flujo recomendado:**
    /// 1. Empleado declara skill: POST /api/employees/skills (level: 4, validación NULL)
    /// 2. Manager revisa evidencia
    /// 3. Manager valida: PUT /api/employees/skills/{id}/validate
    ///    - Si está de acuerdo: { "newLevel": null } (confirma nivel 4)
    ///    - Si no: { "newLevel": 3 } (ajusta a nivel 3)
    /// 
    /// **Casos de uso:**
    /// - Manager valida skills tras revisar evidencia
    /// - Ajuste de nivel si el empleado sobreestimó/subestimó
    /// - Auditoría de skills del equipo
    /// </remarks>
    /// <param name="id">ID de la habilidad del empleado (EmployeeSkill ID, NO el skillId)</param>
    /// <param name="request">Nivel ajustado (opcional). Si es null, solo valida sin modificar nivel</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="200">Habilidad validada exitosamente</response>
    /// <response code="404">EmployeeSkill no encontrado</response>
    /// <response code="401">No autenticado</response>
    /// <response code="403">Sin permisos (requiere rol Manager o Admin)</response>
    [HttpPut("skills/{id}/validate")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ValidateSkill(Guid id, [FromBody] ValidateSkillRequest request)
    {
        var validatorUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var result = await _employeeSkillService.ValidateSkillAsync(id, organizationId, validatorUserId, request.NewLevel);

        if (!result)
        {
            return NotFound();
        }

        return Ok(ApiResponse<object>.SuccessResponse("Habilidad validada exitosamente"));
    }
}
