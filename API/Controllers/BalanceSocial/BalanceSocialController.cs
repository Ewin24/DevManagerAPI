namespace API.Controllers.BalanceSocial;

using Application.Common.Models;
using Application.DTOs.BalanceSocial;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller para balance social — indicadores y reportes de gestión social
/// </summary>
[ApiController]
[Route("api/balance-social")]
[Authorize]
public class BalanceSocialController : ControllerBase
{
    private readonly IBalanceSocialService _balanceSocialService;
    private readonly ILogger<BalanceSocialController> _logger;

    public BalanceSocialController(
        IBalanceSocialService balanceSocialService,
        ILogger<BalanceSocialController> logger)
    {
        _balanceSocialService = balanceSocialService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene el indicador de balance social de un asociado en un año
    /// </summary>
    [HttpGet("indicador/{asociadoId:guid}/{anio:int}")]
    [ProducesResponseType(typeof(ApiResponse<IndicadorBalanceSocialDto?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIndicador(Guid asociadoId, int anio)
    {
        _logger.LogInformation(
            "Obteniendo indicador de balance social para asociado {AsociadoId}, año {Anio}",
            asociadoId, anio);

        var result = await _balanceSocialService.GetIndicadorAsync(asociadoId, anio);
        return Ok(ApiResponse<IndicadorBalanceSocialDto?>.SuccessResponse(result));
    }

    /// <summary>
    /// Calcula y registra el indicador de balance social de un asociado
    /// </summary>
    [HttpPost("indicador/calcular")]
    [ProducesResponseType(typeof(ApiResponse<IndicadorBalanceSocialDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CalcularIndicador(
        [FromQuery] Guid asociadoId,
        [FromQuery] Guid organizationId,
        [FromQuery] int anio)
    {
        _logger.LogInformation(
            "Calculando indicador de balance social para asociado {AsociadoId}, año {Anio}",
            asociadoId, anio);

        var result = await _balanceSocialService.CalcularIndicadorAsync(asociadoId, organizationId, anio);
        return Ok(ApiResponse<IndicadorBalanceSocialDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Obtiene los indicadores de una organización en un año
    /// </summary>
    [HttpGet("organizacion/{organizationId:guid}/{anio:int}")]
    [ProducesResponseType(typeof(ApiResponse<List<IndicadorBalanceSocialDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIndicadoresByOrganizacion(Guid organizationId, int anio)
    {
        _logger.LogInformation(
            "Obteniendo indicadores de organización {OrgId}, año {Anio}",
            organizationId, anio);

        var result = await _balanceSocialService.GetIndicadoresByOrganizacionAsync(organizationId, anio);
        return Ok(ApiResponse<List<IndicadorBalanceSocialDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Obtiene los asociados que NO cumplen con las horas mínimas de educación
    /// </summary>
    [HttpGet("no-cumplen/{organizationId:guid}/{anio:int}")]
    [ProducesResponseType(typeof(ApiResponse<List<IndicadorBalanceSocialDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNoCumplenEducacion(Guid organizationId, int anio)
    {
        _logger.LogInformation(
            "Obteniendo asociados que no cumplen educación en organización {OrgId}, año {Anio}",
            organizationId, anio);

        var result = await _balanceSocialService.GetNoCumplenEducacionAsync(organizationId, anio);
        return Ok(ApiResponse<List<IndicadorBalanceSocialDto>>.SuccessResponse(result));
    }
}
