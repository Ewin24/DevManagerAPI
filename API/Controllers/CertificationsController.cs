namespace API.Controllers;

using Application.Common.Models;
using Application.DTOs.Profiles;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/// <summary>
/// Controller para gestión de certificaciones de empleados
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CertificationsController : ControllerBase
{
    private readonly ICertificationService _certService;
    private readonly ILogger<CertificationsController> _logger;

    public CertificationsController(ICertificationService certService, ILogger<CertificationsController> logger)
    {
        _certService = certService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene mis certificaciones
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CertificationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMine()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var orgId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var certs = await _certService.GetMyCertificationsAsync(userId, orgId);
        return Ok(ApiResponse<IEnumerable<CertificationDto>>.SuccessResponse(certs));
    }

    /// <summary>
    /// Crea una certificación para el usuario autenticado
    /// </summary>
    [HttpPost("me")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CertificationRequest req)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var orgId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var id = await _certService.CreateCertificationAsync(userId, orgId, req);
        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<Guid>.SuccessResponse(id));
    }

    /// <summary>
    /// Obtiene certificación por ID (si me pertenece)
    /// </summary>
    [HttpGet("me/{id}")]
    [ProducesResponseType(typeof(ApiResponse<CertificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var orgId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);
        var cert = await _certService.GetCertificationByIdAsync(id, orgId);
        if (cert == null) return NotFound();
        return Ok(ApiResponse<CertificationDto>.SuccessResponse(cert));
    }

    /// <summary>
    /// Actualiza una certificación propia
    /// </summary>
    [HttpPut("me/{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CertificationRequest req)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var orgId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);
        var success = await _certService.UpdateCertificationAsync(id, userId, orgId, req);
        if (!success) return NotFound();
        return Ok(ApiResponse<object>.SuccessResponse("Certificación actualizada"));
    }

    /// <summary>
    /// Elimina una certificación propia
    /// </summary>
    [HttpDelete("me/{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var orgId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);
        var success = await _certService.DeleteCertificationAsync(id, userId, orgId);
        if (!success) return NotFound();
        return Ok(ApiResponse<object>.SuccessResponse("Certificación eliminada"));
    }
}