namespace API.Controllers.HabeasData;

using Application.Common.Models;
using Application.DTOs.HabeasData;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller para Habeas Data
/// Ley 1581/2012 — Autorizaciones y derechos ARCO
/// </summary>
[ApiController]
[Route("api/habeas-data")]
[Authorize]
public class HabeasDataController : ControllerBase
{
    private readonly IHabeasDataService _habeasDataService;
    private readonly ILogger<HabeasDataController> _logger;

    public HabeasDataController(
        IHabeasDataService habeasDataService,
        ILogger<HabeasDataController> logger)
    {
        _habeasDataService = habeasDataService;
        _logger = logger;
    }

    // ===== Autorizaciones =====

    /// <summary>Registra una autorización de tratamiento de datos</summary>
    [HttpPost("autorizaciones")]
    [ProducesResponseType(typeof(ApiResponse<AutorizacionDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> RegistrarAutorizacion([FromBody] CreateAutorizacionDto dto)
    {
        _logger.LogInformation(
            "Registrando autorización habeas data para asociado {AsociadoId}",
            dto.AsociadoId);

        var result = await _habeasDataService.RegistrarAutorizacionAsync(dto);
        return CreatedAtAction(
            nameof(GetAutorizacionVigente),
            new { asociadoId = dto.AsociadoId },
            ApiResponse<AutorizacionDto>.SuccessResponse(result));
    }

    /// <summary>Revoca una autorización de tratamiento de datos</summary>
    [HttpPost("autorizaciones/{autorizacionId:guid}/revocar")]
    [ProducesResponseType(typeof(ApiResponse<AutorizacionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevocarAutorizacion(Guid autorizacionId)
    {
        try
        {
            var result = await _habeasDataService.RevocarAutorizacionAsync(autorizacionId);
            return Ok(ApiResponse<AutorizacionDto>.SuccessResponse(result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<AutorizacionDto>.SuccessResponse(ex.Message));
        }
    }

    /// <summary>Obtiene la autorización vigente de un asociado</summary>
    [HttpGet("autorizaciones/{asociadoId:guid}/vigente")]
    [ProducesResponseType(typeof(ApiResponse<AutorizacionDto?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAutorizacionVigente(Guid asociadoId)
    {
        var result = await _habeasDataService.GetAutorizacionVigenteAsync(asociadoId);
        return Ok(ApiResponse<AutorizacionDto?>.SuccessResponse(result));
    }

    /// <summary>Verifica si un asociado tiene autorización vigente</summary>
    [HttpGet("autorizaciones/{asociadoId:guid}/tiene-vigente")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> TieneAutorizacionVigente(Guid asociadoId)
    {
        var result = await _habeasDataService.TieneAutorizacionVigenteAsync(asociadoId);
        return Ok(ApiResponse<bool>.SuccessResponse(result, result ? "Tiene autorización vigente" : "No tiene autorización vigente"));
    }

    // ===== Solicitudes ARCO =====

    /// <summary>Registra una solicitud ARCO</summary>
    [HttpPost("arco")]
    [ProducesResponseType(typeof(ApiResponse<SolicitudARCODto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CrearSolicitudARCO([FromBody] CreateSolicitudARCODto dto)
    {
        _logger.LogInformation(
            "Creando solicitud ARCO tipo {Tipo} para asociado {AsociadoId}",
            dto.Tipo, dto.AsociadoId);

        var result = await _habeasDataService.CrearSolicitudARCOAsync(dto);
        return CreatedAtAction(
            nameof(GetSolicitudesARCOByAsociado),
            new { asociadoId = dto.AsociadoId },
            ApiResponse<SolicitudARCODto>.SuccessResponse(result));
    }

    /// <summary>Atiende una solicitud ARCO</summary>
    [HttpPost("arco/{solicitudId:guid}/atender")]
    [ProducesResponseType(typeof(ApiResponse<SolicitudARCODto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtenderSolicitudARCO(
        Guid solicitudId,
        [FromQuery] string respuesta)
    {
        try
        {
            var result = await _habeasDataService.AtenderSolicitudARCOAsync(solicitudId, respuesta);
            return Ok(ApiResponse<SolicitudARCODto>.SuccessResponse(result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<SolicitudARCODto>.SuccessResponse(ex.Message));
        }
    }

    /// <summary>Rechaza una solicitud ARCO</summary>
    [HttpPost("arco/{solicitudId:guid}/rechazar")]
    [ProducesResponseType(typeof(ApiResponse<SolicitudARCODto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RechazarSolicitudARCO(
        Guid solicitudId,
        [FromQuery] string motivoRechazo)
    {
        try
        {
            var result = await _habeasDataService.RechazarSolicitudARCOAsync(solicitudId, motivoRechazo);
            return Ok(ApiResponse<SolicitudARCODto>.SuccessResponse(result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<SolicitudARCODto>.SuccessResponse(ex.Message));
        }
    }

    /// <summary>Obtiene las solicitudes ARCO de un asociado</summary>
    [HttpGet("arco/asociado/{asociadoId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<SolicitudARCODto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSolicitudesARCOByAsociado(Guid asociadoId)
    {
        var result = await _habeasDataService.GetSolicitudesARCOByAsociadoAsync(asociadoId);
        return Ok(ApiResponse<List<SolicitudARCODto>>.SuccessResponse(result));
    }

    /// <summary>Obtiene solicitudes ARCO pendientes de una organización</summary>
    [HttpGet("arco/pendientes/{organizationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<SolicitudARCODto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSolicitudesARCOPendientes(Guid organizationId)
    {
        var result = await _habeasDataService.GetSolicitudesARCOPendientesAsync(organizationId);
        return Ok(ApiResponse<List<SolicitudARCODto>>.SuccessResponse(result));
    }
}
