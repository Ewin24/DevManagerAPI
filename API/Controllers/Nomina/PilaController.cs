namespace API.Controllers.Nomina;

using Application.Common.Models;
using Application.DTOs.Nomina;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller para gestión de aportes PILA (Planilla Integrada de Liquidación de Aportes)
/// </summary>
[ApiController]
[Route("api/nomina/pila")]
[Authorize]
public class PilaController : ControllerBase
{
    private readonly IPilaService _pilaService;
    private readonly ILogger<PilaController> _logger;

    public PilaController(
        IPilaService pilaService,
        ILogger<PilaController> logger)
    {
        _pilaService = pilaService;
        _logger = logger;
    }

    /// <summary>
    /// Genera la planilla PILA para una organización en un período
    /// </summary>
    [HttpGet("{organizationId:guid}/{anio:int}/{mes:int}")]
    [ProducesResponseType(typeof(ApiResponse<List<PilaAporteDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanilla(Guid organizationId, int anio, int mes)
    {
        _logger.LogInformation("Generando planilla PILA para organización {OrgId}, {Mes}/{Anio}",
            organizationId, mes, anio);

        var result = await _pilaService.GenerarPlanillaAsync(mes, anio, organizationId);
        return Ok(ApiResponse<List<PilaAporteDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Calcula los aportes PILA para un asociado
    /// </summary>
    [HttpPost("calcular")]
    [ProducesResponseType(typeof(ApiResponse<PilaAporteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Calcular(
        [FromQuery] Guid asociadoId,
        [FromQuery] decimal ingresos,
        [FromQuery] int nivelRiesgoARL = 1)
    {
        _logger.LogInformation(
            "Calculando aportes PILA para asociado {AsociadoId}, ingresos {Ingresos}, riesgo {Riesgo}",
            asociadoId, ingresos, nivelRiesgoARL);

        var result = await _pilaService.CalcularAportesAsync(asociadoId, ingresos, nivelRiesgoARL);
        return Ok(ApiResponse<PilaAporteDto>.SuccessResponse(result));
    }
}
