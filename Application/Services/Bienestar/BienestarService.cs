namespace Application.Services.Bienestar;

using Application.DTOs.Bienestar;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de bienestar y compensación social
/// Gestiona auxilios, becas, créditos blandos y fondo de solidaridad
/// Fondo: 10% de excedentes según Ley 79 art.54
/// </summary>
public class BienestarService : IBienestarService
{
    private readonly ILogger<BienestarService> _logger;

    // Almacenes en memoria para simulación
    private readonly List<ProgramaBienestarDto> _programasStore = new();
    private readonly List<SolicitudBienestarDto> _solicitudesStore = new();
    private readonly List<AuxilioDto> _auxiliosStore = new();
    private readonly List<FondoSolidaridadDto> _fondosStore = new();

    private const decimal PorcentajeFondo = 0.10m; // 10% Ley 79 art.54

    public BienestarService(ILogger<BienestarService> logger)
    {
        _logger = logger;
    }

    // ===== Programas de Bienestar =====

    /// <inheritdoc/>
    public Task<List<ProgramaBienestarDto>> GetProgramasAsync(Guid organizationId)
    {
        _logger.LogInformation("Obteniendo programas de bienestar de organización {OrgId}", organizationId);

        var result = _programasStore
            .Where(p => p.OrganizationId == organizationId)
            .ToList();

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<ProgramaBienestarDto> CreateProgramaAsync(CreateProgramaBienestarDto dto)
    {
        _logger.LogInformation("Creando programa de bienestar '{Nombre}'", dto.Nombre);

        var programa = new ProgramaBienestarDto
        {
            Id = Guid.NewGuid(),
            OrganizationId = dto.OrganizationId,
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Presupuesto = dto.Presupuesto,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            Activo = true,
            MaxBeneficiarios = dto.MaxBeneficiarios,
            CreatedAt = DateTime.UtcNow
        };

        _programasStore.Add(programa);
        return Task.FromResult(programa);
    }

    // ===== Solicitudes =====

    /// <inheritdoc/>
    public Task<SolicitudBienestarDto> CreateSolicitudAsync(CreateSolicitudBienestarDto dto)
    {
        _logger.LogInformation(
            "Creando solicitud de bienestar para asociado {AsociadoId}, tipo {Tipo}, monto {Monto}",
            dto.AsociadoId, dto.TipoAuxilio, dto.MontoSolicitado);

        var programa = dto.ProgramaBienestarId.HasValue
            ? _programasStore.FirstOrDefault(p => p.Id == dto.ProgramaBienestarId)
            : null;

        var solicitud = new SolicitudBienestarDto
        {
            Id = Guid.NewGuid(),
            AsociadoId = dto.AsociadoId,
            OrganizationId = dto.OrganizationId,
            ProgramaBienestarId = dto.ProgramaBienestarId,
            ProgramaNombre = programa?.Nombre,
            TipoAuxilio = dto.TipoAuxilio,
            TipoAuxilioNombre = dto.TipoAuxilio.ToString(),
            MontoSolicitado = dto.MontoSolicitado,
            Estado = EstadoSolicitudBienestar.Pendiente,
            EstadoNombre = EstadoSolicitudBienestar.Pendiente.ToString(),
            Motivo = dto.Motivo,
            FechaRequerida = dto.FechaRequerida,
            CreatedAt = DateTime.UtcNow
        };

        _solicitudesStore.Add(solicitud);
        return Task.FromResult(solicitud);
    }

    /// <inheritdoc/>
    public Task<List<SolicitudBienestarDto>> GetSolicitudesByAsociadoAsync(Guid asociadoId)
    {
        _logger.LogInformation("Obteniendo solicitudes del asociado {AsociadoId}", asociadoId);

        var result = _solicitudesStore
            .Where(s => s.AsociadoId == asociadoId)
            .ToList();

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<SolicitudBienestarDto> AprobarSolicitudAsync(Guid solicitudId, decimal montoAprobado, Guid resueltoPorUserId)
    {
        _logger.LogInformation(
            "Aprobando solicitud {SolicitudId} con monto {MontoAprobado}",
            solicitudId, montoAprobado);

        var existing = _solicitudesStore.FirstOrDefault(s => s.Id == solicitudId);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Solicitud {solicitudId} no encontrada");
        }

        var updated = existing with
        {
            Estado = EstadoSolicitudBienestar.Aprobada,
            EstadoNombre = EstadoSolicitudBienestar.Aprobada.ToString(),
            MontoAprobado = montoAprobado,
            FechaResolucion = DateTime.UtcNow,
            ObservacionesResolucion = $"Aprobada por ${montoAprobado:N2}"
        };

        var index = _solicitudesStore.IndexOf(existing);
        _solicitudesStore[index] = updated;

        return Task.FromResult(updated);
    }

    /// <inheritdoc/>
    public Task<SolicitudBienestarDto> RechazarSolicitudAsync(Guid solicitudId, string observaciones, Guid resueltoPorUserId)
    {
        _logger.LogInformation("Rechazando solicitud {SolicitudId}", solicitudId);

        var existing = _solicitudesStore.FirstOrDefault(s => s.Id == solicitudId);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Solicitud {solicitudId} no encontrada");
        }

        var updated = existing with
        {
            Estado = EstadoSolicitudBienestar.Rechazada,
            EstadoNombre = EstadoSolicitudBienestar.Rechazada.ToString(),
            FechaResolucion = DateTime.UtcNow,
            ObservacionesResolucion = observaciones
        };

        var index = _solicitudesStore.IndexOf(existing);
        _solicitudesStore[index] = updated;

        return Task.FromResult(updated);
    }

    // ===== Auxilios =====

    /// <inheritdoc/>
    public Task<AuxilioDto> EntregarAuxilioAsync(Guid asociadoId, Guid organizationId, Guid? solicitudId,
        TipoAuxilio tipo, decimal monto, string concepto, bool requiereReintegro)
    {
        _logger.LogInformation(
            "Entregando auxilio {Tipo} a asociado {AsociadoId} por {Monto}",
            tipo, asociadoId, monto);

        var auxilio = new AuxilioDto
        {
            Id = Guid.NewGuid(),
            AsociadoId = asociadoId,
            OrganizationId = organizationId,
            SolicitudBienestarId = solicitudId,
            Tipo = tipo,
            TipoNombre = tipo.ToString(),
            Monto = monto,
            FechaEntrega = DateTime.UtcNow,
            Concepto = concepto,
            RequiereReintegro = requiereReintegro
        };

        // Si hay un crédito blando, establecer fecha límite a 12 meses
        if (tipo == TipoAuxilio.CreditoBlando && requiereReintegro)
        {
            auxilio = auxilio with { FechaLimiteReintegro = DateTime.UtcNow.AddMonths(12) };
        }

        _auxiliosStore.Add(auxilio);
        return Task.FromResult(auxilio);
    }

    /// <inheritdoc/>
    public Task<List<AuxilioDto>> GetAuxiliosByAsociadoAsync(Guid asociadoId)
    {
        _logger.LogInformation("Obteniendo auxilios del asociado {AsociadoId}", asociadoId);

        var result = _auxiliosStore
            .Where(a => a.AsociadoId == asociadoId)
            .ToList();

        return Task.FromResult(result);
    }

    // ===== Fondo de Solidaridad =====

    /// <inheritdoc/>
    public Task<FondoSolidaridadDto> CalcularAporteFondoAsync(Guid organizationId, DateTime periodo, decimal totalExcedentes)
    {
        var aporte = Math.Round(totalExcedentes * PorcentajeFondo, 2, MidpointRounding.AwayFromZero);

        _logger.LogInformation(
            "Calculando aporte al fondo de solidaridad: excedentes {Excedentes}, 10% = {Aporte}",
            totalExcedentes, aporte);

        // Buscar fondo existente para el mismo período
        var existing = _fondosStore.FirstOrDefault(f =>
            f.OrganizationId == organizationId && f.Periodo == periodo);

        if (existing != null)
        {
            // Actualizar
            var updated = existing with
            {
                TotalExcedentes = totalExcedentes,
                AporteFondo = aporte,
                SaldoDisponible = existing.SaldoDisponible + (aporte - existing.AporteFondo)
            };

            var index = _fondosStore.IndexOf(existing);
            _fondosStore[index] = updated;
            return Task.FromResult(updated);
        }

        var fondo = new FondoSolidaridadDto
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            Periodo = periodo,
            TotalExcedentes = totalExcedentes,
            AporteFondo = aporte,
            SaldoDisponible = aporte,
            TotalDesembolsado = 0,
            Vigente = true,
            CreatedAt = DateTime.UtcNow
        };

        _fondosStore.Add(fondo);
        return Task.FromResult(fondo);
    }

    /// <inheritdoc/>
    public Task<FondoSolidaridadDto?> GetFondoActualAsync(Guid organizationId)
    {
        _logger.LogInformation("Obteniendo fondo de solidaridad actual para organización {OrgId}", organizationId);

        var fondo = _fondosStore
            .Where(f => f.OrganizationId == organizationId && f.Vigente)
            .OrderByDescending(f => f.Periodo)
            .FirstOrDefault();

        return Task.FromResult(fondo);
    }
}
