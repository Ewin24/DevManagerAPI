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
    /// Obtiene las habilidades de un empleado
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <returns>Lista de habilidades del empleado</returns>
    [HttpGet("{id}/skills")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeSkillResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeeSkills(Guid id)
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var skills = await _employeeSkillService.GetEmployeeSkillsAsync(id, organizationId);

        return Ok(ApiResponse<IEnumerable<EmployeeSkillResponse>>.SuccessResponse(skills));
    }

    /// <summary>
    /// Crea o actualiza una habilidad del empleado autenticado
    /// </summary>
    /// <param name="request">Datos de la habilidad</param>
    /// <returns>Resultado de la operación</returns>
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
    /// Valida una habilidad de un empleado
    /// </summary>
    /// <param name="id">ID de la habilidad del empleado</param>
    /// <param name="request">Datos de validación</param>
    /// <returns>Resultado de la operación</returns>
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
