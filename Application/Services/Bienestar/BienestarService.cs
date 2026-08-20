namespace Application.Services.Bienestar;

using Application.DTOs.Bienestar;
using Application.Interfaces;
using Domain.Entities.Bienestar;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de bienestar y compensación social
/// Gestiona auxilios, becas, créditos blandos y fondo de solidaridad
/// Fondo: 10% de excedentes según Ley 79 art.54
/// </summary>
public class BienestarService : IBienestarService
{
    private readonly IProgramaBienestarRepository _programasRepository;
    private readonly ISolicitudBienestarRepository _solicitudesRepository;
    private readonly IAuxilioRepository _auxiliosRepository;
    private readonly IFondoSolidaridadRepository _fondosRepository;
    private readonly ILogger<BienestarService> _logger;

    private const decimal PorcentajeFondo = 0.10m; // 10% Ley 79 art.54

    public BienestarService(
        IProgramaBienestarRepository programasRepository,
        ISolicitudBienestarRepository solicitudesRepository,
        IAuxilioRepository auxiliosRepository,
        IFondoSolidaridadRepository fondosRepository,
        ILogger<BienestarService> logger)
    {
        _programasRepository = programasRepository;
        _solicitudesRepository = solicitudesRepository;
        _auxiliosRepository = auxiliosRepository;
        _fondosRepository = fondosRepository;
        _logger = logger;
    }

    // ===== Programas de Bienestar =====

    /// <inheritdoc/>
    public async Task<List<ProgramaBienestarDto>> GetProgramasAsync(Guid organizationId)
    {
        _logger.LogInformation("Obteniendo programas de bienestar de organización {OrgId}", organizationId);

        var programas = await _programasRepository.GetByOrganizationAsync(organizationId);
        return programas.Select(MapToDto).ToList();
    }

    /// <inheritdoc/>
    public async Task<ProgramaBienestarDto> CreateProgramaAsync(CreateProgramaBienestarDto dto)
    {
        _logger.LogInformation("Creando programa de bienestar '{Nombre}'", dto.Nombre);

        var programa = new ProgramaBienestar
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

        var creado = await _programasRepository.CreateAsync(programa);
        return MapToDto(creado);
    }

    // ===== Solicitudes =====

    /// <inheritdoc/>
    public async Task<SolicitudBienestarDto> CreateSolicitudAsync(CreateSolicitudBienestarDto dto)
    {
        _logger.LogInformation(
            "Creando solicitud de bienestar para asociado {AsociadoId}, tipo {Tipo}, monto {Monto}",
            dto.AsociadoId, dto.TipoAuxilio, dto.MontoSolicitado);

        var programa = dto.ProgramaBienestarId.HasValue
            ? await _programasRepository.GetByIdAsync(dto.ProgramaBienestarId.Value)
            : null;

        var solicitud = new SolicitudBienestar
        {
            Id = Guid.NewGuid(),
            AsociadoId = dto.AsociadoId,
            OrganizationId = dto.OrganizationId,
            ProgramaBienestarId = dto.ProgramaBienestarId,
            TipoAuxilio = dto.TipoAuxilio,
            MontoSolicitado = dto.MontoSolicitado,
            Estado = EstadoSolicitudBienestar.Pendiente,
            Motivo = dto.Motivo,
            FechaRequerida = dto.FechaRequerida,
            CreatedAt = DateTime.UtcNow
        };

        var creada = await _solicitudesRepository.CreateAsync(solicitud);
        return MapToDto(creada, programa);
    }

    /// <inheritdoc/>
    public async Task<List<SolicitudBienestarDto>> GetSolicitudesByAsociadoAsync(Guid asociadoId)
    {
        _logger.LogInformation("Obteniendo solicitudes del asociado {AsociadoId}", asociadoId);

        var solicitudes = await _solicitudesRepository.GetByAsociadoAsync(asociadoId);
        return solicitudes.Select(s => MapToDto(s, s.Programa)).ToList();
    }

    /// <inheritdoc/>
    public async Task<SolicitudBienestarDto> AprobarSolicitudAsync(Guid solicitudId, decimal montoAprobado, Guid resueltoPorUserId)
    {
        _logger.LogInformation(
            "Aprobando solicitud {SolicitudId} con monto {MontoAprobado}",
            solicitudId, montoAprobado);

        var existing = await _solicitudesRepository.GetByIdAsync(solicitudId);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Solicitud {solicitudId} no encontrada");
        }

        var programa = existing.Programa;
        existing.Estado = EstadoSolicitudBienestar.Aprobada;
        existing.MontoAprobado = montoAprobado;
        existing.FechaResolucion = DateTime.UtcNow;
        existing.ObservacionesResolucion = $"Aprobada por ${montoAprobado:N2}";
        existing.ResueltoPorUserId = resueltoPorUserId;

        var updated = await _solicitudesRepository.UpdateAsync(existing);
        return MapToDto(updated, programa);
    }

    /// <inheritdoc/>
    public async Task<SolicitudBienestarDto> RechazarSolicitudAsync(Guid solicitudId, string observaciones, Guid resueltoPorUserId)
    {
        _logger.LogInformation("Rechazando solicitud {SolicitudId}", solicitudId);

        var existing = await _solicitudesRepository.GetByIdAsync(solicitudId);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Solicitud {solicitudId} no encontrada");
        }

        var programa = existing.Programa;
        existing.Estado = EstadoSolicitudBienestar.Rechazada;
        existing.FechaResolucion = DateTime.UtcNow;
        existing.ObservacionesResolucion = observaciones;
        existing.ResueltoPorUserId = resueltoPorUserId;

        var updated = await _solicitudesRepository.UpdateAsync(existing);
        return MapToDto(updated, programa);
    }

    // ===== Auxilios =====

    /// <inheritdoc/>
    public async Task<AuxilioDto> EntregarAuxilioAsync(Guid asociadoId, Guid organizationId, Guid? solicitudId,
        TipoAuxilio tipo, decimal monto, string concepto, bool requiereReintegro)
    {
        _logger.LogInformation(
            "Entregando auxilio {Tipo} a asociado {AsociadoId} por {Monto}",
            tipo, asociadoId, monto);

        var auxilio = new Auxilio
        {
            Id = Guid.NewGuid(),
            AsociadoId = asociadoId,
            OrganizationId = organizationId,
            SolicitudBienestarId = solicitudId,
            Tipo = tipo,
            Monto = monto,
            FechaEntrega = DateTime.UtcNow,
            Concepto = concepto,
            RequiereReintegro = requiereReintegro,
            CreatedAt = DateTime.UtcNow
        };

        // Si hay un crédito blando, establecer fecha límite a 12 meses
        if (tipo == TipoAuxilio.CreditoBlando && requiereReintegro)
        {
            auxilio.FechaLimiteReintegro = DateTime.UtcNow.AddMonths(12);
        }

        var creado = await _auxiliosRepository.CreateAsync(auxilio);
        return MapToDto(creado);
    }

    /// <inheritdoc/>
    public async Task<List<AuxilioDto>> GetAuxiliosByAsociadoAsync(Guid asociadoId)
    {
        _logger.LogInformation("Obteniendo auxilios del asociado {AsociadoId}", asociadoId);

        var auxilios = await _auxiliosRepository.GetByAsociadoAsync(asociadoId);
        return auxilios.Select(MapToDto).ToList();
    }

    // ===== Fondo de Solidaridad =====

    /// <inheritdoc/>
    public async Task<FondoSolidaridadDto> CalcularAporteFondoAsync(Guid organizationId, DateTime periodo, decimal totalExcedentes)
    {
        var aporte = Math.Round(totalExcedentes * PorcentajeFondo, 2, MidpointRounding.AwayFromZero);

        _logger.LogInformation(
            "Calculando aporte al fondo de solidaridad: excedentes {Excedentes}, 10% = {Aporte}",
            totalExcedentes, aporte);

        // Buscar fondo existente para el mismo período
        var existing = await _fondosRepository.GetByOrganizationAndPeriodoAsync(organizationId, periodo);

        if (existing != null)
        {
            // Actualizar
            existing.TotalExcedentes = totalExcedentes;
            existing.AporteFondo = aporte;
            existing.SaldoDisponible = existing.SaldoDisponible + (aporte - existing.AporteFondo);

            var updated = await _fondosRepository.UpdateAsync(existing);
            return MapToDto(updated);
        }

        var fondo = new FondoSolidaridad
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

        var creado = await _fondosRepository.CreateAsync(fondo);
        return MapToDto(creado);
    }

    /// <inheritdoc/>
    public async Task<FondoSolidaridadDto?> GetFondoActualAsync(Guid organizationId)
    {
        _logger.LogInformation("Obteniendo fondo de solidaridad actual para organización {OrgId}", organizationId);

        var fondo = await _fondosRepository.GetActualAsync(organizationId);
        return fondo != null ? MapToDto(fondo) : null;
    }

    // ===== Mapping =====

    private static ProgramaBienestarDto MapToDto(ProgramaBienestar p) => new()
    {
        Id = p.Id,
        OrganizationId = p.OrganizationId,
        Nombre = p.Nombre,
        Descripcion = p.Descripcion,
        Presupuesto = p.Presupuesto,
        FechaInicio = p.FechaInicio,
        FechaFin = p.FechaFin,
        Activo = p.Activo,
        MaxBeneficiarios = p.MaxBeneficiarios,
        CreatedAt = p.CreatedAt
    };

    private static SolicitudBienestarDto MapToDto(SolicitudBienestar s, ProgramaBienestar? programa) => new()
    {
        Id = s.Id,
        AsociadoId = s.AsociadoId,
        OrganizationId = s.OrganizationId,
        ProgramaBienestarId = s.ProgramaBienestarId,
        ProgramaNombre = programa?.Nombre,
        TipoAuxilio = s.TipoAuxilio,
        TipoAuxilioNombre = s.TipoAuxilio.ToString(),
        MontoSolicitado = s.MontoSolicitado,
        MontoAprobado = s.MontoAprobado,
        Estado = s.Estado,
        EstadoNombre = s.Estado.ToString(),
        Motivo = s.Motivo,
        FechaRequerida = s.FechaRequerida,
        FechaResolucion = s.FechaResolucion,
        ObservacionesResolucion = s.ObservacionesResolucion,
        CreatedAt = s.CreatedAt
    };

    private static AuxilioDto MapToDto(Auxilio a) => new()
    {
        Id = a.Id,
        AsociadoId = a.AsociadoId,
        OrganizationId = a.OrganizationId,
        SolicitudBienestarId = a.SolicitudBienestarId,
        Tipo = a.Tipo,
        TipoNombre = a.Tipo.ToString(),
        Monto = a.Monto,
        FechaEntrega = a.FechaEntrega,
        Concepto = a.Concepto,
        RequiereReintegro = a.RequiereReintegro,
        FechaLimiteReintegro = a.FechaLimiteReintegro
    };

    private static FondoSolidaridadDto MapToDto(FondoSolidaridad f) => new()
    {
        Id = f.Id,
        OrganizationId = f.OrganizationId,
        Periodo = f.Periodo,
        TotalExcedentes = f.TotalExcedentes,
        AporteFondo = f.AporteFondo,
        SaldoDisponible = f.SaldoDisponible,
        TotalDesembolsado = f.TotalDesembolsado,
        Vigente = f.Vigente,
        Observaciones = f.Observaciones,
        CreatedAt = f.CreatedAt
    };
}