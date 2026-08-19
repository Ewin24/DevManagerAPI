namespace Application.Services.HabeasData;

using Application.DTOs.HabeasData;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de Habeas Data
/// Ley 1581/2012 — Autorizaciones y derechos ARCO
/// </summary>
public class HabeasDataService : IHabeasDataService
{
    private readonly ILogger<HabeasDataService> _logger;
    private readonly List<AutorizacionDto> _autorizacionesStore = new();
    private readonly List<SolicitudARCODto> _solicitudesARCOStore = new();

    public HabeasDataService(ILogger<HabeasDataService> logger)
    {
        _logger = logger;
    }

    // ===== Autorizaciones =====

    /// <inheritdoc/>
    public Task<AutorizacionDto> RegistrarAutorizacionAsync(CreateAutorizacionDto dto)
    {
        _logger.LogInformation(
            "Registrando autorización habeas data para asociado {AsociadoId}",
            dto.AsociadoId);

        // Revocar autorizaciones anteriores del mismo asociado
        var anteriores = _autorizacionesStore
            .Where(a => a.AsociadoId == dto.AsociadoId && !a.Revocada)
            .ToList();

        foreach (var ant in anteriores)
        {
            var idx = _autorizacionesStore.IndexOf(ant);
            _autorizacionesStore[idx] = ant with
            {
                Revocada = true,
                FechaRevocacion = DateTime.UtcNow
            };
        }

        var autorizacion = new AutorizacionDto
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

        _autorizacionesStore.Add(autorizacion);
        return Task.FromResult(autorizacion);
    }

    /// <inheritdoc/>
    public Task<AutorizacionDto> RevocarAutorizacionAsync(Guid autorizacionId)
    {
        _logger.LogInformation("Revocando autorización {AutorizacionId}", autorizacionId);

        var existing = _autorizacionesStore.FirstOrDefault(a => a.Id == autorizacionId);
        if (existing == null)
            throw new KeyNotFoundException($"Autorización {autorizacionId} no encontrada");

        var updated = existing with
        {
            Revocada = true,
            FechaRevocacion = DateTime.UtcNow
        };

        var index = _autorizacionesStore.IndexOf(existing);
        _autorizacionesStore[index] = updated;

        return Task.FromResult(updated);
    }

    /// <inheritdoc/>
    public Task<AutorizacionDto?> GetAutorizacionVigenteAsync(Guid asociadoId)
    {
        var result = _autorizacionesStore
            .Where(a => a.AsociadoId == asociadoId && a.Vigente)
            .OrderByDescending(a => a.FechaAutorizacion)
            .FirstOrDefault();

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<bool> TieneAutorizacionVigenteAsync(Guid asociadoId)
    {
        var tiene = _autorizacionesStore.Any(a =>
            a.AsociadoId == asociadoId && a.Vigente);

        return Task.FromResult(tiene);
    }

    // ===== Solicitudes ARCO =====

    /// <inheritdoc/>
    public Task<SolicitudARCODto> CrearSolicitudARCOAsync(CreateSolicitudARCODto dto)
    {
        _logger.LogInformation(
            "Creando solicitud ARCO tipo {Tipo} para asociado {AsociadoId}",
            dto.Tipo, dto.AsociadoId);

        var solicitud = new SolicitudARCODto
        {
            Id = Guid.NewGuid(),
            AsociadoId = dto.AsociadoId,
            OrganizationId = dto.OrganizationId,
            Tipo = dto.Tipo,
            TipoNombre = dto.Tipo.ToString(),
            Fecha = DateTime.UtcNow,
            Estado = EstadoSolicitudARCO.Pendiente,
            EstadoNombre = EstadoSolicitudARCO.Pendiente.ToString(),
            Descripcion = dto.Descripcion,
            CreatedAt = DateTime.UtcNow
        };

        _solicitudesARCOStore.Add(solicitud);
        return Task.FromResult(solicitud);
    }

    /// <inheritdoc/>
    public Task<SolicitudARCODto> AtenderSolicitudARCOAsync(Guid solicitudId, string respuesta)
    {
        _logger.LogInformation("Atendiendo solicitud ARCO {SolicitudId}", solicitudId);

        var existing = _solicitudesARCOStore.FirstOrDefault(s => s.Id == solicitudId);
        if (existing == null)
            throw new KeyNotFoundException($"Solicitud ARCO {solicitudId} no encontrada");

        var updated = existing with
        {
            Estado = EstadoSolicitudARCO.Atendida,
            EstadoNombre = EstadoSolicitudARCO.Atendida.ToString(),
            Respuesta = respuesta,
            FechaRespuesta = DateTime.UtcNow
        };

        var index = _solicitudesARCOStore.IndexOf(existing);
        _solicitudesARCOStore[index] = updated;

        return Task.FromResult(updated);
    }

    /// <inheritdoc/>
    public Task<SolicitudARCODto> RechazarSolicitudARCOAsync(Guid solicitudId, string motivoRechazo)
    {
        _logger.LogInformation("Rechazando solicitud ARCO {SolicitudId}", solicitudId);

        var existing = _solicitudesARCOStore.FirstOrDefault(s => s.Id == solicitudId);
        if (existing == null)
            throw new KeyNotFoundException($"Solicitud ARCO {solicitudId} no encontrada");

        var updated = existing with
        {
            Estado = EstadoSolicitudARCO.Rechazada,
            EstadoNombre = EstadoSolicitudARCO.Rechazada.ToString(),
            Respuesta = motivoRechazo,
            FechaRespuesta = DateTime.UtcNow
        };

        var index = _solicitudesARCOStore.IndexOf(existing);
        _solicitudesARCOStore[index] = updated;

        return Task.FromResult(updated);
    }

    /// <inheritdoc/>
    public Task<List<SolicitudARCODto>> GetSolicitudesARCOByAsociadoAsync(Guid asociadoId)
    {
        var result = _solicitudesARCOStore
            .Where(s => s.AsociadoId == asociadoId)
            .OrderByDescending(s => s.Fecha)
            .ToList();

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<List<SolicitudARCODto>> GetSolicitudesARCOPendientesAsync(Guid organizationId)
    {
        var result = _solicitudesARCOStore
            .Where(s => s.OrganizationId == organizationId && s.Estado == EstadoSolicitudARCO.Pendiente)
            .OrderByDescending(s => s.Fecha)
            .ToList();

        return Task.FromResult(result);
    }
}
