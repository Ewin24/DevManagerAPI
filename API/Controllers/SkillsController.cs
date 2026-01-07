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
    /// Obtiene todas las habilidades disponibles
    /// </summary>
    /// <returns>Lista de habilidades</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SkillDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var skills = await _skillService.GetAllSkillsAsync(organizationId);

        return Ok(ApiResponse<IEnumerable<SkillDto>>.SuccessResponse(skills));
    }

    /// <summary>
    /// Crea una nueva habilidad en el catálogo
    /// </summary>
    /// <param name="skillDto">Datos de la habilidad</param>
    /// <returns>ID de la habilidad creada</returns>
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
