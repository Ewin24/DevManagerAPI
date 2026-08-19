namespace API.Controllers.Bienestar;

using Application.Common.Models;
using Application.DTOs.Bienestar;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller para bienestar y compensación social
/// Programas, solicitudes, auxilios y fondo de solidaridad
/// </summary>
[ApiController]
[Route("api/bienestar")]
[Authorize]
public class BienestarController : ControllerBase
{
    private readonly IBienestarService _bienestarService;
    private readonly ILogger<BienestarController> _logger;

    public BienestarController(
        IBienestarService bienestarService,
        ILogger<BienestarController> logger)
    {
        _bienestarService = bienestarService;
        _logger = logger;
    }

    // ===== Programas =====

    /// <summary>
    /// Obtiene los programas de bienestar de una organización
    /// </summary>
    [HttpGet("programas/{organizationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<ProgramaBienestarDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProgramas(Guid organizationId)
    {
        _logger.LogInformation("Obteniendo programas de bienestar de organización {OrgId}", organizationId);

        var result = await _bienestarService.GetProgramasAsync(organizationId);
        return Ok(ApiResponse<List<ProgramaBienestarDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Crea un nuevo programa de bienestar
    /// </summary>
    [HttpPost("programas")]
    [ProducesResponseType(typeof(ApiResponse<ProgramaBienestarDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreatePrograma([FromBody] CreateProgramaBienestarDto dto)
    {
        _logger.LogInformation("Creando programa de bienestar '{Nombre}'", dto.Nombre);

        var result = await _bienestarService.CreateProgramaAsync(dto);
        return CreatedAtAction(
            nameof(GetProgramas),
            new { organizationId = dto.OrganizationId },
            ApiResponse<ProgramaBienestarDto>.SuccessResponse(result));
    }

    // ===== Solicitudes =====

    /// <summary>
    /// Crea una solicitud de bienestar
    /// </summary>
    [HttpPost("solicitudes")]
    [ProducesResponseType(typeof(ApiResponse<SolicitudBienestarDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSolicitud([FromBody] CreateSolicitudBienestarDto dto)
    {
        _logger.LogInformation(
            "Creando solicitud de bienestar para asociado {AsociadoId}",
            dto.AsociadoId);

        var result = await _bienestarService.CreateSolicitudAsync(dto);
        return CreatedAtAction(
            nameof(GetSolicitudesByAsociado),
            new { asociadoId = dto.AsociadoId },
            ApiResponse<SolicitudBienestarDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Obtiene las solicitudes de un asociado
    /// </summary>
    [HttpGet("solicitudes/{asociadoId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<SolicitudBienestarDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSolicitudesByAsociado(Guid asociadoId)
    {
        _logger.LogInformation("Obteniendo solicitudes del asociado {AsociadoId}", asociadoId);

        var result = await _bienestarService.GetSolicitudesByAsociadoAsync(asociadoId);
        return Ok(ApiResponse<List<SolicitudBienestarDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Aprueba una solicitud de bienestar
    /// </summary>
    [HttpPost("solicitudes/{solicitudId:guid}/aprobar")]
    [ProducesResponseType(typeof(ApiResponse<SolicitudBienestarDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AprobarSolicitud(
        Guid solicitudId,
        [FromQuery] decimal montoAprobado,
        [FromQuery] Guid resueltoPorUserId)
    {
        _logger.LogInformation("Aprobando solicitud {SolicitudId}", solicitudId);

        try
        {
            var result = await _bienestarService.AprobarSolicitudAsync(
                solicitudId, montoAprobado, resueltoPorUserId);
            return Ok(ApiResponse<SolicitudBienestarDto>.SuccessResponse(result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<SolicitudBienestarDto>.SuccessResponse(ex.Message));
        }
    }

    /// <summary>
    /// Rechaza una solicitud de bienestar
    /// </summary>
    [HttpPost("solicitudes/{solicitudId:guid}/rechazar")]
    [ProducesResponseType(typeof(ApiResponse<SolicitudBienestarDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RechazarSolicitud(
        Guid solicitudId,
        [FromQuery] string observaciones,
        [FromQuery] Guid resueltoPorUserId)
    {
        _logger.LogInformation("Rechazando solicitud {SolicitudId}", solicitudId);

        try
        {
            var result = await _bienestarService.RechazarSolicitudAsync(
                solicitudId, observaciones, resueltoPorUserId);
            return Ok(ApiResponse<SolicitudBienestarDto>.SuccessResponse(result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<SolicitudBienestarDto>.SuccessResponse(ex.Message));
        }
    }

    // ===== Fondo de Solidaridad =====

    /// <summary>
    /// Calcula y registra el aporte al fondo de solidaridad (10% excedentes)
    /// </summary>
    [HttpPost("fondo/calcular")]
    [ProducesResponseType(typeof(ApiResponse<FondoSolidaridadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CalcularAporteFondo(
        [FromQuery] Guid organizationId,
        [FromQuery] DateTime periodo,
        [FromQuery] decimal totalExcedentes)
    {
        _logger.LogInformation(
            "Calculando aporte al fondo: organización {OrgId}, excedentes {Excedentes}",
            organizationId, totalExcedentes);

        var result = await _bienestarService.CalcularAporteFondoAsync(
            organizationId, periodo, totalExcedentes);
        return Ok(ApiResponse<FondoSolidaridadDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Obtiene el estado actual del fondo de solidaridad
    /// </summary>
    [HttpGet("fondo/{organizationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<FondoSolidaridadDto?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFondoActual(Guid organizationId)
    {
        _logger.LogInformation(
            "Obteniendo fondo de solidaridad actual para organización {OrgId}",
            organizationId);

        var result = await _bienestarService.GetFondoActualAsync(organizationId);
        return Ok(ApiResponse<FondoSolidaridadDto?>.SuccessResponse(result));
    }

    // ===== Auxilios =====

    /// <summary>
    /// Obtiene los auxilios de un asociado
    /// </summary>
    [HttpGet("auxilios/{asociadoId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<AuxilioDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuxiliosByAsociado(Guid asociadoId)
    {
        _logger.LogInformation("Obteniendo auxilios del asociado {AsociadoId}", asociadoId);

        var result = await _bienestarService.GetAuxiliosByAsociadoAsync(asociadoId);
        return Ok(ApiResponse<List<AuxilioDto>>.SuccessResponse(result));
    }
}
