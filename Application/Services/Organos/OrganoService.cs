namespace Application.Services.Organos;

using Application.DTOs.Organos;
using Application.Interfaces;
using Domain.Entities.Organos;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación de IOrganoService con persistencia EF Core
/// Gestión de órganos de administración, asambleas y votación
/// </summary>
public class OrganoService : IOrganoService
{
    private readonly IOrganoRepository _organosRepository;
    private readonly IMiembroOrganoRepository _miembrosRepository;
    private readonly IActaRepository _actasRepository;
    private readonly IAsambleaRepository _asambleasRepository;
    private readonly IVotoRepository _votosRepository;
    private readonly ILogger<OrganoService> _logger;

    public OrganoService(
        IOrganoRepository organosRepository,
        IMiembroOrganoRepository miembrosRepository,
        IActaRepository actasRepository,
        IAsambleaRepository asambleasRepository,
        IVotoRepository votosRepository,
        ILogger<OrganoService> logger)
    {
        _organosRepository = organosRepository;
        _miembrosRepository = miembrosRepository;
        _actasRepository = actasRepository;
        _asambleasRepository = asambleasRepository;
        _votosRepository = votosRepository;
        _logger = logger;
    }

    // ========= Órganos =========

    public async Task<OrganoDto> CreateOrganoAsync(CreateOrganoDto dto)
    {
        var organo = new Organo
        {
            Id = Guid.NewGuid(),
            Tipo = dto.Tipo,
            Nombre = dto.Nombre,
            OrganizationId = dto.OrganizationId,
            FechaConstitucion = dto.FechaConstitucion,
            Descripcion = dto.Descripcion,
            Activo = true,
            CreatedAt = DateTime.UtcNow
        };
        var creado = await _organosRepository.CreateAsync(organo);
        return await MapToDtoAsync(creado);
    }

    public async Task<OrganoDto?> GetOrganoByIdAsync(Guid id)
    {
        var organo = await _organosRepository.GetByIdAsync(id);
        return organo != null ? await MapToDtoAsync(organo) : null;
    }

    public async Task<List<OrganoDto>> GetOrganosByOrganizationAsync(Guid organizationId)
    {
        var organos = await _organosRepository.GetByOrganizationAsync(organizationId);
        var result = new List<OrganoDto>();
        foreach (var o in organos)
        {
            result.Add(await MapToDtoAsync(o));
        }
        return result;
    }

    public async Task<List<OrganoDto>> GetOrganosByTypeAsync(Guid organizationId, TipoOrgano tipo)
    {
        var organos = await _organosRepository.GetByTypeAsync(organizationId, tipo);
        var result = new List<OrganoDto>();
        foreach (var o in organos)
        {
            result.Add(await MapToDtoAsync(o));
        }
        return result;
    }

    public async Task<OrganoDto> UpdateOrganoAsync(Guid id, UpdateOrganoDto dto)
    {
        var organo = await _organosRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Órgano {id} no encontrado");

        if (dto.Nombre != null) organo.Nombre = dto.Nombre;
        if (dto.Descripcion != null) organo.Descripcion = dto.Descripcion;
        if (dto.Activo.HasValue) organo.Activo = dto.Activo.Value;

        var updated = await _organosRepository.UpdateAsync(organo);
        return await MapToDtoAsync(updated);
    }

    public async Task<bool> DeleteOrganoAsync(Guid id)
    {
        return await _organosRepository.DeleteAsync(id);
    }

    // ========= Miembros =========

    public async Task<MiembroOrganoDto> AsignarMiembroAsync(AsignarMiembroDto dto)
    {
        var miembro = new MiembroOrgano
        {
            Id = Guid.NewGuid(),
            OrganoId = dto.OrganoId,
            AsociadoId = dto.AsociadoId,
            Cargo = dto.Cargo,
            FechaInicio = dto.FechaInicio,
            Activo = true,
            CreatedAt = DateTime.UtcNow
        };
        var creado = await _miembrosRepository.CreateAsync(miembro);
        return MapToDto(creado);
    }

    public async Task<List<MiembroOrganoDto>> GetMiembrosByOrganoAsync(Guid organoId)
    {
        var miembros = await _miembrosRepository.GetByOrganoAsync(organoId);
        return miembros.Select(MapToDto).ToList();
    }

    public async Task<MiembroOrganoDto> UpdateMiembroAsync(Guid id, UpdateMiembroDto dto)
    {
        var miembro = await _miembrosRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Miembro {id} no encontrado");

        if (dto.Cargo != null) miembro.Cargo = dto.Cargo;
        if (dto.FechaFin.HasValue) miembro.FechaFin = dto.FechaFin;
        if (dto.Activo.HasValue) miembro.Activo = dto.Activo.Value;

        var updated = await _miembrosRepository.UpdateAsync(miembro);
        return MapToDto(updated);
    }

    public async Task<bool> RemoveMiembroAsync(Guid id)
    {
        return await _miembrosRepository.DeleteAsync(id);
    }

    // ========= Actas =========

    public async Task<ActaDto> CreateActaAsync(CreateActaDto dto)
    {
        var acta = new Acta
        {
            Id = Guid.NewGuid(),
            OrganoId = dto.OrganoId,
            AsambleaId = dto.AsambleaId,
            Fecha = dto.Fecha,
            TipoSesion = dto.TipoSesion,
            Quorum = dto.Quorum,
            Decisiones = dto.Decisiones,
            ConvocatoriaUrl = dto.ConvocatoriaUrl,
            ActaUrl = dto.ActaUrl,
            Observaciones = dto.Observaciones,
            CreatedAt = DateTime.UtcNow
        };
        var creada = await _actasRepository.CreateAsync(acta);
        return MapToDto(creada);
    }

    public async Task<ActaDto?> GetActaByIdAsync(Guid id)
    {
        var acta = await _actasRepository.GetByIdAsync(id);
        return acta != null ? MapToDto(acta) : null;
    }

    public async Task<List<ActaDto>> GetActasByOrganoAsync(Guid organoId)
    {
        var actas = await _actasRepository.GetByOrganoAsync(organoId);
        return actas.Select(MapToDto).ToList();
    }

    // ========= Asambleas =========

    public async Task<AsambleaDto> ConvocarAsambleaAsync(ConvocarAsambleaDto dto)
    {
        var asamblea = new Asamblea
        {
            Id = Guid.NewGuid(),
            OrganizationId = dto.OrganizationId,
            OrganoId = dto.OrganoId,
            Fecha = dto.Fecha,
            Tipo = dto.Tipo,
            Convocatoria = dto.Convocatoria,
            QuorumMinimo = dto.QuorumMinimo,
            Cerrada = false,
            CreatedAt = DateTime.UtcNow
        };
        var creada = await _asambleasRepository.CreateAsync(asamblea);
        return await MapToDtoAsync(creada);
    }

    public async Task<AsambleaDto?> GetAsambleaByIdAsync(Guid id)
    {
        var asamblea = await _asambleasRepository.GetByIdAsync(id);
        return asamblea != null ? await MapToDtoAsync(asamblea) : null;
    }

    public async Task<List<AsambleaDto>> GetAsambleasByOrganizationAsync(Guid organizationId)
    {
        var asambleas = await _asambleasRepository.GetByOrganizationAsync(organizationId);
        var result = new List<AsambleaDto>();
        foreach (var a in asambleas)
        {
            result.Add(await MapToDtoAsync(a));
        }
        return result;
    }

    public async Task<AsambleaDto> RegistrarAsistenciaAsync(Guid id, RegistrarAsistenciaDto dto)
    {
        var asamblea = await _asambleasRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Asamblea {id} no encontrada");

        asamblea.Asistentes = dto.Asistentes;

        var updated = await _asambleasRepository.UpdateAsync(asamblea);
        return await MapToDtoAsync(updated);
    }

    public async Task<AsambleaDto> CerrarAsambleaAsync(Guid id, CerrarAsambleaDto dto)
    {
        var asamblea = await _asambleasRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Asamblea {id} no encontrada");

        asamblea.Cerrada = true;
        asamblea.FechaCierre = DateTime.UtcNow;
        asamblea.Resultados = dto.Resultados;

        var updated = await _asambleasRepository.UpdateAsync(asamblea);
        return await MapToDtoAsync(updated);
    }

    // ========= Voto =========

    public async Task<VotoDto> EmitirVotoAsync(EmitirVotoDto dto)
    {
        var asamblea = await _asambleasRepository.GetByIdAsync(dto.AsambleaId)
            ?? throw new KeyNotFoundException($"Asamblea {dto.AsambleaId} no encontrada");

        if (asamblea.Cerrada)
            throw new InvalidOperationException("La asamblea ya está cerrada, no se pueden recibir votos");

        if (await _votosRepository.ExistsByAsambleaAndAsociadoAsync(dto.AsambleaId, dto.AsociadoId))
            throw new InvalidOperationException("El asociado ya ha votado en esta asamblea");

        var voto = new Voto
        {
            Id = Guid.NewGuid(),
            AsambleaId = dto.AsambleaId,
            AsociadoId = dto.AsociadoId,
            VotoEmitido = dto.VotoEmitido,
            Fecha = DateTime.UtcNow,
            Observaciones = dto.Observaciones,
            CreatedAt = DateTime.UtcNow
        };
        var creado = await _votosRepository.CreateAsync(voto);
        return MapToDto(creado);
    }

    public async Task<ResultadoVotacionDto> GetResultadosAsync(Guid asambleaId)
    {
        var votos = await _votosRepository.GetByAsambleaAsync(asambleaId);
        return new ResultadoVotacionDto
        {
            AsambleaId = asambleaId,
            TotalVotos = votos.Count,
            Aprobados = votos.Count(v => v.VotoEmitido == TipoVoto.Aprobado),
            Rechazados = votos.Count(v => v.VotoEmitido == TipoVoto.Rechazado),
            Abstenciones = votos.Count(v => v.VotoEmitido == TipoVoto.Abstencion),
            Blancos = votos.Count(v => v.VotoEmitido == TipoVoto.Blanco)
        };
    }

    public async Task<bool> HaVotadoAsync(Guid asambleaId, Guid asociadoId)
    {
        return await _votosRepository.ExistsByAsambleaAndAsociadoAsync(asambleaId, asociadoId);
    }

    // ========= Mapping =========

    private async Task<OrganoDto> MapToDtoAsync(Organo o) => new()
    {
        Id = o.Id,
        Tipo = o.Tipo,
        Nombre = o.Nombre,
        OrganizationId = o.OrganizationId,
        FechaConstitucion = o.FechaConstitucion,
        Descripcion = o.Descripcion,
        Activo = o.Activo,
        MiembrosCount = await _miembrosRepository.CountByOrganoAsync(o.Id),
        ActasCount = await _actasRepository.CountByOrganoAsync(o.Id)
    };

    private static MiembroOrganoDto MapToDto(MiembroOrgano m) => new()
    {
        Id = m.Id,
        OrganoId = m.OrganoId,
        AsociadoId = m.AsociadoId,
        Cargo = m.Cargo,
        FechaInicio = m.FechaInicio,
        FechaFin = m.FechaFin,
        Activo = m.Activo
    };

    private static ActaDto MapToDto(Acta a) => new()
    {
        Id = a.Id,
        OrganoId = a.OrganoId,
        AsambleaId = a.AsambleaId,
        Fecha = a.Fecha,
        TipoSesion = a.TipoSesion,
        Quorum = a.Quorum,
        Decisiones = a.Decisiones,
        ConvocatoriaUrl = a.ConvocatoriaUrl,
        ActaUrl = a.ActaUrl,
        Observaciones = a.Observaciones
    };

    private async Task<AsambleaDto> MapToDtoAsync(Asamblea a) => new()
    {
        Id = a.Id,
        OrganizationId = a.OrganizationId,
        OrganoId = a.OrganoId,
        Fecha = a.Fecha,
        Tipo = a.Tipo,
        Convocatoria = a.Convocatoria,
        QuorumMinimo = a.QuorumMinimo,
        Asistentes = a.Asistentes,
        Cerrada = a.Cerrada,
        FechaCierre = a.FechaCierre,
        Resultados = a.Resultados,
        VotosCount = await _votosRepository.CountByAsambleaAsync(a.Id)
    };

    private static VotoDto MapToDto(Voto v) => new()
    {
        Id = v.Id,
        AsambleaId = v.AsambleaId,
        AsociadoId = v.AsociadoId,
        VotoEmitido = v.VotoEmitido,
        Fecha = v.Fecha,
        Observaciones = v.Observaciones
    };
}