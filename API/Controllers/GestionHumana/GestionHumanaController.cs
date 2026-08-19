namespace API.Controllers.GestionHumana;

using Application.Common.Models;
using Application.DTOs.GestionHumana;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller para gestión humana solidaria — competencias y perfiles
/// </summary>
[ApiController]
[Route("api/gestion-humana/competencias")]
[Authorize]
public class GestionHumanaController : ControllerBase
{
    private readonly IGestionHumanaService _gestionHumanaService;
    private readonly ILogger<GestionHumanaController> _logger;

    public GestionHumanaController(
        IGestionHumanaService gestionHumanaService,
        ILogger<GestionHumanaController> logger)
    {
        _gestionHumanaService = gestionHumanaService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene las competencias de un asociado
    /// </summary>
    [HttpGet("{asociadoId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<CompetenciaAsociadoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCompetencias(Guid asociadoId)
    {
        _logger.LogInformation("Obteniendo competencias del asociado {AsociadoId}", asociadoId);

        var result = await _gestionHumanaService.GetCompetenciasAsync(asociadoId);
        return Ok(ApiResponse<List<CompetenciaAsociadoDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Registra una nueva competencia para un asociado
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CompetenciaAsociadoDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCompetencia(
        [FromQuery] Guid asociadoId,
        [FromQuery] Guid organizationId,
        [FromQuery] string competencia,
        [FromQuery] int nivel)
    {
        _logger.LogInformation(
            "Registrando competencia '{Competencia}' nivel {Nivel} para asociado {AsociadoId}",
            competencia, nivel, asociadoId);

        var result = await _gestionHumanaService.CreateCompetenciaAsync(
            asociadoId, organizationId, competencia, nivel);

        return CreatedAtAction(
            nameof(GetCompetencias),
            new { asociadoId },
            ApiResponse<CompetenciaAsociadoDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Actualiza la disponibilidad de una competencia
    /// </summary>
    [HttpPatch("{competenciaId:guid}/disponibilidad")]
    [ProducesResponseType(typeof(ApiResponse<CompetenciaAsociadoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDisponibilidad(Guid competenciaId, [FromBody] bool disponible)
    {
        _logger.LogInformation(
            "Actualizando disponibilidad de competencia {CompetenciaId} a {Disponible}",
            competenciaId, disponible);

        try
        {
            var result = await _gestionHumanaService.UpdateDisponibilidadAsync(competenciaId, disponible);
            return Ok(ApiResponse<CompetenciaAsociadoDto>.SuccessResponse(result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CompetenciaAsociadoDto>.SuccessResponse(ex.Message));
        }
    }
}
