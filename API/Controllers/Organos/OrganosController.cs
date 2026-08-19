namespace API.Controllers.Organos;

using Application.DTOs.Organos;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrganosController : ControllerBase
{
    private readonly IOrganoService _organoService;

    public OrganosController(IOrganoService organoService)
    {
        _organoService = organoService;
    }

    // ========= Órganos =========

    [HttpPost]
    public async Task<IActionResult> CreateOrgano([FromBody] CreateOrganoDto dto)
    {
        var result = await _organoService.CreateOrganoAsync(dto);
        return CreatedAtAction(nameof(GetOrganoById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrganoById(Guid id)
    {
        var result = await _organoService.GetOrganoByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("organization/{organizationId:guid}")]
    public async Task<IActionResult> GetOrganosByOrganization(Guid organizationId)
    {
        var result = await _organoService.GetOrganosByOrganizationAsync(organizationId);
        return Ok(result);
    }

    [HttpGet("organization/{organizationId:guid}/tipo/{tipo}")]
    public async Task<IActionResult> GetOrganosByType(Guid organizationId, TipoOrgano tipo)
    {
        var result = await _organoService.GetOrganosByTypeAsync(organizationId, tipo);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateOrgano(Guid id, [FromBody] UpdateOrganoDto dto)
    {
        try
        {
            var result = await _organoService.UpdateOrganoAsync(id, dto);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteOrgano(Guid id)
    {
        var deleted = await _organoService.DeleteOrganoAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // ========= Miembros =========

    [HttpPost("miembros")]
    public async Task<IActionResult> AsignarMiembro([FromBody] AsignarMiembroDto dto)
    {
        var result = await _organoService.AsignarMiembroAsync(dto);
        return CreatedAtAction(nameof(GetMiembrosByOrgano), new { organoId = result.OrganoId }, result);
    }

    [HttpGet("{organoId:guid}/miembros")]
    public async Task<IActionResult> GetMiembrosByOrgano(Guid organoId)
    {
        var result = await _organoService.GetMiembrosByOrganoAsync(organoId);
        return Ok(result);
    }

    [HttpPut("miembros/{id:guid}")]
    public async Task<IActionResult> UpdateMiembro(Guid id, [FromBody] UpdateMiembroDto dto)
    {
        try
        {
            var result = await _organoService.UpdateMiembroAsync(id, dto);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("miembros/{id:guid}")]
    public async Task<IActionResult> RemoveMiembro(Guid id)
    {
        var removed = await _organoService.RemoveMiembroAsync(id);
        if (!removed) return NotFound();
        return NoContent();
    }

    // ========= Actas =========

    [HttpPost("actas")]
    public async Task<IActionResult> CreateActa([FromBody] CreateActaDto dto)
    {
        var result = await _organoService.CreateActaAsync(dto);
        return CreatedAtAction(nameof(GetActaById), new { id = result.Id }, result);
    }

    [HttpGet("actas/{id:guid}")]
    public async Task<IActionResult> GetActaById(Guid id)
    {
        var result = await _organoService.GetActaByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("{organoId:guid}/actas")]
    public async Task<IActionResult> GetActasByOrgano(Guid organoId)
    {
        var result = await _organoService.GetActasByOrganoAsync(organoId);
        return Ok(result);
    }

    // ========= Asambleas =========

    [HttpPost("asambleas")]
    public async Task<IActionResult> ConvocarAsamblea([FromBody] ConvocarAsambleaDto dto)
    {
        var result = await _organoService.ConvocarAsambleaAsync(dto);
        return CreatedAtAction(nameof(GetAsambleaById), new { id = result.Id }, result);
    }

    [HttpGet("asambleas/{id:guid}")]
    public async Task<IActionResult> GetAsambleaById(Guid id)
    {
        var result = await _organoService.GetAsambleaByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("asambleas/organization/{organizationId:guid}")]
    public async Task<IActionResult> GetAsambleasByOrganization(Guid organizationId)
    {
        var result = await _organoService.GetAsambleasByOrganizationAsync(organizationId);
        return Ok(result);
    }

    [HttpPut("asambleas/{id:guid}/asistencia")]
    public async Task<IActionResult> RegistrarAsistencia(Guid id, [FromBody] RegistrarAsistenciaDto dto)
    {
        try
        {
            var result = await _organoService.RegistrarAsistenciaAsync(id, dto);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("asambleas/{id:guid}/cerrar")]
    public async Task<IActionResult> CerrarAsamblea(Guid id, [FromBody] CerrarAsambleaDto dto)
    {
        try
        {
            var result = await _organoService.CerrarAsambleaAsync(id, dto);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // ========= Voto =========

    [HttpPost("votos")]
    public async Task<IActionResult> EmitirVoto([FromBody] EmitirVotoDto dto)
    {
        try
        {
            var result = await _organoService.EmitirVotoAsync(dto);
            return CreatedAtAction(nameof(GetResultados), new { asambleaId = result.AsambleaId }, result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpGet("votos/resultados/{asambleaId:guid}")]
    public async Task<IActionResult> GetResultados(Guid asambleaId)
    {
        var result = await _organoService.GetResultadosAsync(asambleaId);
        return Ok(result);
    }

    [HttpGet("votos/ha-votado/{asambleaId:guid}/{asociadoId:guid}")]
    public async Task<IActionResult> HaVotado(Guid asambleaId, Guid asociadoId)
    {
        var haVotado = await _organoService.HaVotadoAsync(asambleaId, asociadoId);
        return Ok(new { haVotado });
    }
}
