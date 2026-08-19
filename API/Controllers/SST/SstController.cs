namespace API.Controllers.SST;

using Application.Common.Models;
using Application.DTOs.SST;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller para Salud Ocupacional y SST
/// SG-SST per Decreto 1072/2015 + Res. 0312/2019
/// Exámenes médicos, ARL, accidentes y matriz de riesgos
/// </summary>
[ApiController]
[Route("api/sst")]
[Authorize]
public class SstController : ControllerBase
{
    private readonly ISstService _sstService;
    private readonly ILogger<SstController> _logger;

    public SstController(ISstService sstService, ILogger<SstController> logger)
    {
        _sstService = sstService;
        _logger = logger;
    }

    // ===== Exámenes Médicos =====

    /// <summary>Programa un examen médico ocupacional</summary>
    [HttpPost("examenes")]
    [ProducesResponseType(typeof(ApiResponse<ExamenMedicoDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> ProgramarExamen([FromBody] CreateExamenMedicoDto dto)
    {
        _logger.LogInformation("Programando examen médico para asociado {AsociadoId}", dto.AsociadoId);

        var result = await _sstService.ProgramarExamenAsync(dto);
        return CreatedAtAction(
            nameof(GetExamenesByAsociado),
            new { asociadoId = dto.AsociadoId },
            ApiResponse<ExamenMedicoDto>.SuccessResponse(result));
    }

    /// <summary>Registra el resultado de un examen realizado</summary>
    [HttpPost("examenes/{examenId:guid}/registrar")]
    [ProducesResponseType(typeof(ApiResponse<ExamenMedicoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegistrarExamen(
        Guid examenId,
        [FromQuery] string resultado,
        [FromQuery] string? archivoUrl,
        [FromQuery] string? observaciones)
    {
        try
        {
            var result = await _sstService.RegistrarExamenAsync(examenId, resultado, archivoUrl, observaciones);
            return Ok(ApiResponse<ExamenMedicoDto>.SuccessResponse(result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ExamenMedicoDto>.SuccessResponse(ex.Message));
        }
    }

    /// <summary>Obtiene los exámenes de un asociado</summary>
    [HttpGet("examenes/asociado/{asociadoId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<ExamenMedicoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExamenesByAsociado(Guid asociadoId)
    {
        var result = await _sstService.GetExamenesByAsociadoAsync(asociadoId);
        return Ok(ApiResponse<List<ExamenMedicoDto>>.SuccessResponse(result));
    }

    /// <summary>Obtiene exámenes pendientes de una organización</summary>
    [HttpGet("examenes/pendientes/{organizationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<ExamenMedicoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExamenesPendientes(Guid organizationId)
    {
        var result = await _sstService.GetExamenesPendientesAsync(organizationId);
        return Ok(ApiResponse<List<ExamenMedicoDto>>.SuccessResponse(result));
    }

    // ===== ARL =====

    /// <summary>Verifica la vigencia de la ARL (alerta 30 días antes de expiry)</summary>
    [HttpGet("arl/vigencia/{organizationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerificarVigenciaArl(Guid organizationId)
    {
        var (vigente, diasRestantes, alerta) = await _sstService.VerificarVigenciaArlAsync(organizationId);

        var result = new
        {
            vigente,
            diasRestantes,
            alerta
        };

        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    // ===== Accidentes =====

    /// <summary>Reporta un accidente de trabajo (FURAT)</summary>
    [HttpPost("accidentes")]
    [ProducesResponseType(typeof(ApiResponse<AccidenteDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> ReportarAccidente([FromBody] CreateAccidenteDto dto)
    {
        _logger.LogInformation("Reportando accidente para asociado {AsociadoId}", dto.AsociadoId);

        var result = await _sstService.ReportarAccidenteAsync(dto);
        return CreatedAtAction(
            nameof(GetAccidentesByOrganizacion),
            new { organizationId = dto.OrganizationId },
            ApiResponse<AccidenteDto>.SuccessResponse(result));
    }

    /// <summary>Obtiene accidentes pendientes de investigación</summary>
    [HttpGet("accidentes/pendientes-investigacion/{organizationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<AccidenteDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccidentesPendientesInvestigacion(Guid organizationId)
    {
        var result = await _sstService.GetAccidentesPendientesInvestigacionAsync(organizationId);
        return Ok(ApiResponse<List<AccidenteDto>>.SuccessResponse(result));
    }

    /// <summary>Registra la investigación de un accidente</summary>
    [HttpPost("accidentes/{accidenteId:guid}/investigar")]
    [ProducesResponseType(typeof(ApiResponse<AccidenteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegistrarInvestigacion(
        Guid accidenteId,
        [FromQuery] DateTime fechaInvestigacion,
        [FromQuery] string conclusiones,
        [FromQuery] string causas,
        [FromQuery] string medidasCorrectivas)
    {
        try
        {
            var result = await _sstService.RegistrarInvestigacionAsync(
                accidenteId, fechaInvestigacion, conclusiones, causas, medidasCorrectivas);
            return Ok(ApiResponse<AccidenteDto>.SuccessResponse(result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<AccidenteDto>.SuccessResponse(ex.Message));
        }
    }

    /// <summary>Obtiene todos los accidentes de una organización</summary>
    [HttpGet("accidentes/{organizationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<AccidenteDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccidentesByOrganizacion(Guid organizationId)
    {
        var result = await _sstService.GetAccidentesByOrganizacionAsync(organizationId);
        return Ok(ApiResponse<List<AccidenteDto>>.SuccessResponse(result));
    }

    // ===== Riesgos =====

    /// <summary>Obtiene la matriz de riesgos de una organización</summary>
    [HttpGet("riesgos/{organizationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<RiesgoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRiesgos(Guid organizationId)
    {
        var result = await _sstService.GetRiesgosAsync(organizationId);
        return Ok(ApiResponse<List<RiesgoDto>>.SuccessResponse(result));
    }

    /// <summary>Agrega un riesgo a la matriz</summary>
    [HttpPost("riesgos")]
    [ProducesResponseType(typeof(ApiResponse<RiesgoDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CrearRiesgo([FromBody] CreateRiesgoDto dto)
    {
        var result = await _sstService.CrearRiesgoAsync(dto);
        return CreatedAtAction(
            nameof(GetRiesgos),
            new { organizationId = dto.OrganizationId },
            ApiResponse<RiesgoDto>.SuccessResponse(result));
    }
}
