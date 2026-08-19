namespace API.Controllers.Excedentes;

using Application.Common.Models;
using Application.DTOs.Excedentes;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller para distribución de excedentes
/// Ley 79 art. 54 — 20/20/10 distribution
/// </summary>
[ApiController]
[Route("api/excedentes")]
[Authorize]
public class ExcedentesController : ControllerBase
{
    private readonly IExcedenteService _excedenteService;
    private readonly ILogger<ExcedentesController> _logger;

    public ExcedentesController(
        IExcedenteService excedenteService,
        ILogger<ExcedentesController> logger)
    {
        _excedenteService = excedenteService;
        _logger = logger;
    }

    /// <summary>Calcula y registra la distribución de excedentes</summary>
    [HttpPost("calcular")]
    [ProducesResponseType(typeof(ApiResponse<ExcedenteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CalcularDistribucion([FromBody] CreateExcedenteDto dto)
    {
        _logger.LogInformation(
            "Calculando distribución: org {OrgId}, período {Periodo}, total {Total:N2}",
            dto.OrganizationId, dto.Periodo, dto.TotalExcedentes);

        try
        {
            var result = await _excedenteService.CalcularDistribucionAsync(dto);
            return Ok(ApiResponse<ExcedenteDto>.SuccessResponse(result));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ExcedenteDto>.SuccessResponse(ex.Message));
        }
    }

    /// <summary>Obtiene distribución por período</summary>
    [HttpGet("{organizationId:guid}/{periodo:regex(\\d{{4}}-\\d{{2}}-\\d{{2}})}")]
    [ProducesResponseType(typeof(ApiResponse<ExcedenteDto?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPeriodo(
        Guid organizationId, DateTime periodo)
    {
        var result = await _excedenteService.GetByPeriodoAsync(organizationId, periodo);
        return Ok(ApiResponse<ExcedenteDto?>.SuccessResponse(result));
    }

    /// <summary>Obtiene todas las distribuciones de una organización</summary>
    [HttpGet("{organizationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<ExcedenteDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByOrganizacion(Guid organizationId)
    {
        var result = await _excedenteService.GetByOrganizacionAsync(organizationId);
        return Ok(ApiResponse<List<ExcedenteDto>>.SuccessResponse(result));
    }

    /// <summary>Aprueba la distribución en Asamblea General</summary>
    [HttpPost("{excedenteId:guid}/aprobar")]
    [ProducesResponseType(typeof(ApiResponse<ExcedenteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AprobarDistribucion(
        Guid excedenteId,
        [FromQuery] decimal? revalorizacion,
        [FromQuery] decimal? retornoCooperativo)
    {
        try
        {
            var result = await _excedenteService.AprobarDistribucionAsync(
                excedenteId, revalorizacion, retornoCooperativo);
            return Ok(ApiResponse<ExcedenteDto>.SuccessResponse(result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ExcedenteDto>.SuccessResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ExcedenteDto>.SuccessResponse(ex.Message));
        }
    }
}
