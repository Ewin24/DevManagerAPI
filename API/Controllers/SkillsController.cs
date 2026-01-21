namespace API.Controllers;

using Application.Common.Exceptions;
using Application.Common.Models;
using Application.DTOs.Skills;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/// <summary>
/// Controller para gestión de catálogo de habilidades
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SkillsController : ControllerBase
{
    private readonly ISkillService _skillService;
    private readonly ILogger<SkillsController> _logger;

    public SkillsController(ISkillService skillService, ILogger<SkillsController> logger)
    {
        _skillService = skillService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las habilidades disponibles (catálogo global + organizacional)
    /// </summary>
    /// <remarks>
    /// Retorna el catálogo completo de habilidades: skills globales (disponibles para todas las organizaciones) y skills específicas de la organización.
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": null,
    ///         "data": [
    ///             {
    ///                 "id": "aaaaaaaa-0000-0000-0000-000000000001",
    ///                 "name": "C#",
    ///                 "category": "Backend",
    ///                 "skillType": 0,
    ///                 "organizationId": null
    ///             },
    ///             {
    ///                 "id": "bbbbbbbb-0000-0000-0000-000000000001",
    ///                 "name": "Sistema Legacy Interno",
    ///                 "category": "Plataformas Internas",
    ///                 "skillType": 1,
    ///                 "organizationId": "11111111-1111-1111-1111-111111111111"
    ///             }
    ///         ]
    ///     }
    /// 
    /// **Tipos de Skills (SkillType):**
    /// - 0 = Global (disponible para todas las organizaciones)
    /// - 1 = Organizational (específico de la organización)
    /// 
    /// **Casos de uso:**
    /// - Selector de habilidades en formularios
    /// - Filtros de búsqueda
    /// - Dashboard de habilidades disponibles
    /// </remarks>
    /// <returns>Lista de habilidades</returns>
    /// <response code="200">Catálogo de habilidades obtenido exitosamente</response>
    /// <response code="401">No autenticado</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SkillDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var skills = await _skillService.GetAllSkillsAsync(organizationId);

        return Ok(ApiResponse<IEnumerable<SkillDto>>.SuccessResponse(skills));
    }

    /// <summary>
    /// Crea una nueva habilidad organizacional en el catálogo
    /// </summary>
    /// <remarks>
    /// Permite a administradores/managers agregar habilidades específicas de la organización (tecnologías propietarias, herramientas internas, etc.).
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     POST /api/skills
    ///     {
    ///         "name": "SAP ERP",
    ///         "category": "Sistemas Empresariales",
    ///         "skillType": 1
    ///     }
    /// 
    /// **Ejemplo de Response (201 Created):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Habilidad creada exitosamente",
    ///         "data": "cccccccc-0000-0000-0000-000000000001"
    ///     }
    /// 
    /// **Validaciones:**
    /// - name: Requerido, máximo 100 caracteres, único dentro de la organización
    /// - category: Requerido, máximo 100 caracteres
    /// - skillType: Solo puede crear skills tipo 1 (Organizational)
    /// 
    /// **Nota importante:**
    /// - Solo se pueden crear skills organizacionales vía API
    /// - Las skills globales se agregan vía seeder/migración
    /// - OrganizationId se toma del JWT automáticamente
    /// 
    /// **Casos de uso:**
    /// - Agregar tecnologías propietarias
    /// - Habilidades específicas del negocio
    /// - Herramientas internas
    /// </remarks>
    /// <param name="skillDto">Datos de la habilidad (nombre, categoría, tipo)</param>
    /// <returns>ID de la habilidad creada</returns>
    /// <response code="201">Habilidad creada exitosamente</response>
    /// <response code="400">Datos inválidos o habilidad duplicada</response>
    /// <response code="401">No autenticado</response>
    /// <response code="403">Sin permisos (requiere rol Admin o Manager)</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] SkillDto skillDto)
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        try
        {
            var skillId = await _skillService.CreateSkillAsync(skillDto, organizationId);

            return CreatedAtAction(
                nameof(Create), 
                new { id = skillId }, 
                ApiResponse<Guid>.SuccessResponse(skillId, "Habilidad creada exitosamente"));
        }
        catch (BusinessValidationException ex)
        {
            return BadRequest();
        }
    }
}
