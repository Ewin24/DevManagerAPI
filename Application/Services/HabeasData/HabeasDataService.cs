namespace Application.Services.HabeasData;

using Application.DTOs.HabeasData;
using Application.Interfaces;
using Domain.Entities.HabeasData;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de Habeas Data
/// Ley 1581/2012 — Autorizaciones y derechos ARCO
/// </summary>
public class HabeasDataService : IHabeasDataService
{
    private readonly IAutorizacionRepository _autorizacionesRepository;
    private readonly ISolicitudARCORepository _solicitudesARCORepository;
    private readonly ILogger<HabeasDataService> _logger;

    public HabeasDataService(
        IAutorizacionRepository autorizacionesRepository,
        ISolicitudARCORepository solicitudesARCORepository,
        ILogger<HabeasDataService> logger)
    {
        _autorizacionesRepository = autorizacionesRepository;
        _solicitudesARCORepository = solicitudesARCORepository;
        _logger = logger;
    }

    // ===== Autorizaciones =====

    /// <inheritdoc/>
    public async Task<AutorizacionDto> RegistrarAutorizacionAsync(CreateAutorizacionDto dto)
    {
        _logger.LogInformation(
            "Registrando autorización habeas data para asociado {AsociadoId}",
            dto.AsociadoId);

        // Revocar autorizaciones anteriores del mismo asociado
        var anteriores = await _autorizacionesRepository.GetActiveByAsociadoAsync(dto.AsociadoId);

        foreach (var ant in anteriores)
        {
            ant.Revocada = true;
            ant.FechaRevocacion = DateTime.UtcNow;
            await _autorizacionesRepository.UpdateAsync(ant);
        }

        var autorizacion = new Autorizacion
        {
            Id = Guid.NewGuid(),
            AsociadoId = dto.AsociadoId,
            OrganizationId = dto.OrganizationId,
            FechaAutorizacion = DateTime.UtcNow,
            Vigencia = null, // indefinida por defecto
            Revocada = false,
            Finalidad = dto.Finalidad,
            MedioAutorizacion = dto.MedioAutorizacion,
            DireccionIp = dto.DireccionIp,
            CreatedAt = DateTime.UtcNow
        };

        var creada = await _autorizacionesRepository.CreateAsync(autorizacion);
        return MapToDto(creada);
    }

    /// <inheritdoc/>
    public async Task<AutorizacionDto> RevocarAutorizacionAsync(Guid autorizacionId)
    {
        _logger.LogInformation("Revocando autorización {AutorizacionId}", autorizacionId);

        var existing = await _autorizacionesRepository.GetByIdAsync(autorizacionId);
        if (existing == null)
            throw new KeyNotFoundException($"Autorización {autorizacionId} no encontrada");

        existing.Revocada = true;
        existing.FechaRevocacion = DateTime.UtcNow;

        var updated = await _autorizacionesRepository.UpdateAsync(existing);
        return MapToDto(updated);
    }

    /// <inheritdoc/>
    public async Task<AutorizacionDto?> GetAutorizacionVigenteAsync(Guid asociadoId)
    {
        var result = await _autorizacionesRepository.GetVigenteByAsociadoAsync(asociadoId);
        return result != null ? MapToDto(result) : null;
    }

    /// <inheritdoc/>
    public async Task<bool> TieneAutorizacionVigenteAsync(Guid asociadoId)
    {
        return await _autorizacionesRepository.TieneVigenteAsync(asociadoId);
    }

    // ===== Solicitudes ARCO =====

    /// <inheritdoc/>
    public async Task<SolicitudARCODto> CrearSolicitudARCOAsync(CreateSolicitudARCODto dto)
    {
        _logger.LogInformation(
            "Creando solicitud ARCO tipo {Tipo} para asociado {AsociadoId}",
            dto.Tipo, dto.AsociadoId);

        var solicitud = new SolicitudARCO
        {
            Id = Guid.NewGuid(),
            AsociadoId = dto.AsociadoId,
            OrganizationId = dto.OrganizationId,
            Tipo = dto.Tipo,
            Fecha = DateTime.UtcNow,
            Estado = EstadoSolicitudARCO.Pendiente,
            Descripcion = dto.Descripcion,
            CreatedAt = DateTime.UtcNow
        };

        var creada = await _solicitudesARCORepository.CreateAsync(solicitud);
        return MapToDto(creada);
    }

    /// <inheritdoc/>
    public async Task<SolicitudARCODto> AtenderSolicitudARCOAsync(Guid solicitudId, string respuesta)
    {
        _logger.LogInformation("Atendiendo solicitud ARCO {SolicitudId}", solicitudId);

        var existing = await _solicitudesARCORepository.GetByIdAsync(solicitudId);
        if (existing == null)
            throw new KeyNotFoundException($"Solicitud ARCO {solicitudId} no encontrada");

        existing.Estado = EstadoSolicitudARCO.Atendida;
        existing.Respuesta = respuesta;
        existing.FechaRespuesta = DateTime.UtcNow;

        var updated = await _solicitudesARCORepository.UpdateAsync(existing);
        return MapToDto(updated);
    }

    /// <inheritdoc/>
    public async Task<SolicitudARCODto> RechazarSolicitudARCOAsync(Guid solicitudId, string motivoRechazo)
    {
        _logger.LogInformation("Rechazando solicitud ARCO {SolicitudId}", solicitudId);

        var existing = await _solicitudesARCORepository.GetByIdAsync(solicitudId);
        if (existing == null)
            throw new KeyNotFoundException($"Solicitud ARCO {solicitudId} no encontrada");

        existing.Estado = EstadoSolicitudARCO.Rechazada;
        existing.Respuesta = motivoRechazo;
        existing.FechaRespuesta = DateTime.UtcNow;

        var updated = await _solicitudesARCORepository.UpdateAsync(existing);
        return MapToDto(updated);
    }

    /// <inheritdoc/>
    public async Task<List<SolicitudARCODto>> GetSolicitudesARCOByAsociadoAsync(Guid asociadoId)
    {
        var solicitudes = await _solicitudesARCORepository.GetByAsociadoAsync(asociadoId);
        return solicitudes.Select(MapToDto).ToList();
    }

    /// <inheritdoc/>
    public async Task<List<SolicitudARCODto>> GetSolicitudesARCOPendientesAsync(Guid organizationId)
    {
        var solicitudes = await _solicitudesARCORepository.GetPendientesAsync(organizationId);
        return solicitudes.Select(MapToDto).ToList();
    }

    // ===== Mapping =====

    private static AutorizacionDto MapToDto(Autorizacion a) => new()
    {
        Id = a.Id,
        AsociadoId = a.AsociadoId,
        OrganizationId = a.OrganizationId,
        FechaAutorizacion = a.FechaAutorizacion,
        Vigencia = a.Vigencia,
        Revocada = a.Revocada,
        FechaRevocacion = a.FechaRevocacion,
        Finalidad = a.Finalidad,
        MedioAutorizacion = a.MedioAutorizacion,
        DireccionIp = a.DireccionIp,
        CreatedAt = a.CreatedAt
    };

    private static SolicitudARCODto MapToDto(SolicitudARCO s) => new()
    {
        Id = s.Id,
        AsociadoId = s.AsociadoId,
        OrganizationId = s.OrganizationId,
        Tipo = s.Tipo,
        TipoNombre = s.Tipo.ToString(),
        Fecha = s.Fecha,
        Estado = s.Estado,
        EstadoNombre = s.Estado.ToString(),
        Descripcion = s.Descripcion,
        Respuesta = s.Respuesta,
        FechaRespuesta = s.FechaRespuesta,
        Radicado = s.Radicado,
        CreatedAt = s.CreatedAt
    };
}