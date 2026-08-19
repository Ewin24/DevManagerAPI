namespace API.Controllers.Reportes;

using Application.Common.Models;
using Application.DTOs.Reportes;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller para reportes de Supersolidaria
/// Compila Balance Social, asociados y cumplimiento normativo
/// </summary>
[ApiController]
[Route("api/reportes/supersolidaria")]
[Authorize]
public class SupersolidariaReportsController : ControllerBase
{
    private readonly IReportGeneratorService _reportGeneratorService;
    private readonly ILogger<SupersolidariaReportsController> _logger;

    public SupersolidariaReportsController(
        IReportGeneratorService reportGeneratorService,
        ILogger<SupersolidariaReportsController> logger)
    {
        _reportGeneratorService = reportGeneratorService;
        _logger = logger;
    }

    /// <summary>Genera un reporte integral para Supersolidaria</summary>
    [HttpPost("generar")]
    [ProducesResponseType(typeof(ApiResponse<ReporteSupersolidariaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerarReporte([FromBody] CreateReporteDto dto)
    {
        _logger.LogInformation(
            "Generando reporte {Tipo} para organización {OrgId}, período {Periodo}",
            dto.TipoReporte, dto.OrganizationId, dto.Periodo);

        var result = await _reportGeneratorService.GenerarReporteAsync(
            dto.OrganizationId, dto.Periodo, dto.TipoReporte);

        return Ok(ApiResponse<ReporteSupersolidariaDto>.SuccessResponse(result));
    }

    /// <summary>Obtiene un reporte por período</summary>
    [HttpGet("{organizationId:guid}/{periodo:regex(\\d{{4}}-\\d{{2}}-\\d{{2}})}")]
    [ProducesResponseType(typeof(ApiResponse<ReporteSupersolidariaDto?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReporteByPeriodo(
        Guid organizationId, DateTime periodo)
    {
        var result = await _reportGeneratorService.GetReporteByPeriodoAsync(organizationId, periodo);
        return Ok(ApiResponse<ReporteSupersolidariaDto?>.SuccessResponse(result));
    }

    /// <summary>Obtiene todos los reportes de una organización</summary>
    [HttpGet("{organizationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<ReporteSupersolidariaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReportesByOrganizacion(Guid organizationId)
    {
        var result = await _reportGeneratorService.GetReportesByOrganizacionAsync(organizationId);
        return Ok(ApiResponse<List<ReporteSupersolidariaDto>>.SuccessResponse(result));
    }

    /// <summary>Marca un reporte como enviado a Supersolidaria</summary>
    [HttpPost("{reporteId:guid}/enviar")]
    [ProducesResponseType(typeof(ApiResponse<ReporteSupersolidariaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarcarEnviado(Guid reporteId)
    {
        try
        {
            var result = await _reportGeneratorService.MarcarEnviadoAsync(reporteId);
            return Ok(ApiResponse<ReporteSupersolidariaDto>.SuccessResponse(result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<ReporteSupersolidariaDto>.SuccessResponse(ex.Message));
        }
    }
}
