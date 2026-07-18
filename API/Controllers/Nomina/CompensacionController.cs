namespace API.Controllers.Nomina;

using Application.Common.Models;
using Application.DTOs.Nomina;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller para gestión de compensación de asociados (CTAs)
/// </summary>
[ApiController]
[Route("api/nomina/compensacion")]
[Authorize]
public class CompensacionController : ControllerBase
{
    private readonly ICompensacionService _compensacionService;
    private readonly ILogger<CompensacionController> _logger;

    public CompensacionController(
        ICompensacionService compensacionService,
        ILogger<CompensacionController> logger)
    {
        _compensacionService = compensacionService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene las compensaciones de un asociado en un año
    /// </summary>
    [HttpGet("{asociadoId:guid}/{anio:int}")]
    [ProducesResponseType(typeof(ApiResponse<List<CompensacionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByAsociado(Guid asociadoId, int anio)
    {
        _logger.LogInformation("Obteniendo compensaciones para asociado {AsociadoId}, año {Anio}",
            asociadoId, anio);

        var result = await _compensacionService.GetByAsociadoAsync(asociadoId, anio);
        return Ok(ApiResponse<List<CompensacionDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Crea un nuevo registro de compensación
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CompensacionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCompensacionDto dto)
    {
        _logger.LogInformation("Creando compensación para asociado {AsociadoId}", dto.AsociadoId);

        var result = await _compensacionService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetByAsociado),
            new { asociadoId = dto.AsociadoId, anio = dto.Periodo.Year },
            ApiResponse<CompensacionDto>.SuccessResponse(result));
    }
}
