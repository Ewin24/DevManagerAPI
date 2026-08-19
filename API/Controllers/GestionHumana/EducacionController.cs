namespace API.Controllers.GestionHumana;

using Application.Common.Models;
using Application.DTOs.GestionHumana;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller para educación cooperativa — programas, inscripciones, cumplimiento
/// </summary>
[ApiController]
[Route("api/gestion-humana/educacion")]
[Authorize]
public class EducacionController : ControllerBase
{
    private readonly IEducacionService _educacionService;
    private readonly ILogger<EducacionController> _logger;

    public EducacionController(
        IEducacionService educacionService,
        ILogger<EducacionController> logger)
    {
        _educacionService = educacionService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene los programas educativos de una organización
    /// </summary>
    [HttpGet("programas/{organizationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<ProgramaEducacionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProgramas(Guid organizationId)
    {
        _logger.LogInformation("Obteniendo programas educativos de organización {OrgId}", organizationId);

        var result = await _educacionService.GetProgramasAsync(organizationId);
        return Ok(ApiResponse<List<ProgramaEducacionDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Crea un nuevo programa educativo
    /// </summary>
    [HttpPost("programas")]
    [ProducesResponseType(typeof(ApiResponse<ProgramaEducacionDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreatePrograma([FromBody] CreateProgramaEducacionDto dto)
    {
        _logger.LogInformation("Creando programa educativo '{Nombre}'", dto.Nombre);

        var result = await _educacionService.CreateProgramaAsync(dto);
        return CreatedAtAction(
            nameof(GetProgramas),
            new { organizationId = dto.OrganizationId },
            ApiResponse<ProgramaEducacionDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Inscribe un asociado en un programa educativo
    /// </summary>
    [HttpPost("inscribir")]
    [ProducesResponseType(typeof(ApiResponse<AsociadoEducacionDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Inscribir([FromBody] CreateAsociadoEducacionDto dto)
    {
        _logger.LogInformation(
            "Inscribiendo asociado {AsociadoId} en programa {ProgramaId}",
            dto.AsociadoId, dto.ProgramaEducacionId);

        var result = await _educacionService.InscribirAsync(dto);
        return CreatedAtAction(
            nameof(GetHistorial),
            new { asociadoId = dto.AsociadoId },
            ApiResponse<AsociadoEducacionDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Obtiene el historial educativo de un asociado
    /// </summary>
    [HttpGet("historial/{asociadoId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<AsociadoEducacionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistorial(Guid asociadoId)
    {
        _logger.LogInformation("Obteniendo historial educativo del asociado {AsociadoId}", asociadoId);

        var result = await _educacionService.GetHistorialAsync(asociadoId);
        return Ok(ApiResponse<List<AsociadoEducacionDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Verifica si un asociado cumple con las 20 horas mínimas anuales de educación
    /// </summary>
    [HttpGet("cumplimiento/{asociadoId:guid}/{anio:int}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CumpleMinimoHoras(Guid asociadoId, int anio)
    {
        _logger.LogInformation(
            "Verificando horas mínimas para asociado {AsociadoId}, año {Anio}",
            asociadoId, anio);

        var result = await _educacionService.CumpleMinimoHorasAsync(asociadoId, anio);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }
}
