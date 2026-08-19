namespace Application.Services.Organos;

using Application.DTOs.Organos;
using Application.Interfaces;
using Domain.Entities.Organos;
using Domain.Enums;

/// <summary>
/// Implementación en memoria de IOrganoService
/// Gestión de órganos de administración, asambleas y votación
/// </summary>
public class OrganoService : IOrganoService
{
    private readonly List<Organo> _organos = new();
    private readonly List<MiembroOrgano> _miembros = new();
    private readonly List<Acta> _actas = new();
    private readonly List<Asamblea> _asambleas = new();
    private readonly List<Voto> _votos = new();

    // ========= Órganos =========

    public Task<OrganoDto> CreateOrganoAsync(CreateOrganoDto dto)
    {
        var organo = new Organo
        {
            Id = Guid.NewGuid(),
            Tipo = dto.Tipo,
            Nombre = dto.Nombre,
            OrganizationId = dto.OrganizationId,
            FechaConstitucion = dto.FechaConstitucion,
            Descripcion = dto.Descripcion,
            Activo = true
        };
        _organos.Add(organo);
        return Task.FromResult(MapToDto(organo));
    }

    public Task<OrganoDto?> GetOrganoByIdAsync(Guid id)
    {
        var organo = _organos.FirstOrDefault(o => o.Id == id);
        return Task.FromResult(organo != null ? MapToDto(organo) : null);
    }

    public Task<List<OrganoDto>> GetOrganosByOrganizationAsync(Guid organizationId)
    {
        var result = _organos
            .Where(o => o.OrganizationId == organizationId)
            .Select(MapToDto)
            .ToList();
        return Task.FromResult(result);
    }

    public Task<List<OrganoDto>> GetOrganosByTypeAsync(Guid organizationId, TipoOrgano tipo)
    {
        var result = _organos
            .Where(o => o.OrganizationId == organizationId && o.Tipo == tipo)
            .Select(MapToDto)
            .ToList();
        return Task.FromResult(result);
    }

    public Task<OrganoDto> UpdateOrganoAsync(Guid id, UpdateOrganoDto dto)
    {
        var organo = _organos.FirstOrDefault(o => o.Id == id)
            ?? throw new KeyNotFoundException($"Órgano {id} no encontrado");

        if (dto.Nombre != null) organo.Nombre = dto.Nombre;
        if (dto.Descripcion != null) organo.Descripcion = dto.Descripcion;
        if (dto.Activo.HasValue) organo.Activo = dto.Activo.Value;

        return Task.FromResult(MapToDto(organo));
    }

    public Task<bool> DeleteOrganoAsync(Guid id)
    {
        var organo = _organos.FirstOrDefault(o => o.Id == id);
        if (organo == null) return Task.FromResult(false);
        _organos.Remove(organo);
        return Task.FromResult(true);
    }

    // ========= Miembros =========

    public Task<MiembroOrganoDto> AsignarMiembroAsync(AsignarMiembroDto dto)
    {
        var miembro = new MiembroOrgano
        {
            Id = Guid.NewGuid(),
            OrganoId = dto.OrganoId,
            AsociadoId = dto.AsociadoId,
            Cargo = dto.Cargo,
            FechaInicio = dto.FechaInicio,
            Activo = true
        };
        _miembros.Add(miembro);
        return Task.FromResult(MapToDto(miembro));
    }

    public Task<List<MiembroOrganoDto>> GetMiembrosByOrganoAsync(Guid organoId)
    {
        var result = _miembros
            .Where(m => m.OrganoId == organoId)
            .Select(MapToDto)
            .ToList();
        return Task.FromResult(result);
    }

    public Task<MiembroOrganoDto> UpdateMiembroAsync(Guid id, UpdateMiembroDto dto)
    {
        var miembro = _miembros.FirstOrDefault(m => m.Id == id)
            ?? throw new KeyNotFoundException($"Miembro {id} no encontrado");

        if (dto.Cargo != null) miembro.Cargo = dto.Cargo;
        if (dto.FechaFin.HasValue) miembro.FechaFin = dto.FechaFin;
        if (dto.Activo.HasValue) miembro.Activo = dto.Activo.Value;

        return Task.FromResult(MapToDto(miembro));
    }

    public Task<bool> RemoveMiembroAsync(Guid id)
    {
        var miembro = _miembros.FirstOrDefault(m => m.Id == id);
        if (miembro == null) return Task.FromResult(false);
        _miembros.Remove(miembro);
        return Task.FromResult(true);
    }

    // ========= Actas =========

    public Task<ActaDto> CreateActaAsync(CreateActaDto dto)
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
            Observaciones = dto.Observaciones
        };
        _actas.Add(acta);
        return Task.FromResult(MapToDto(acta));
    }

    public Task<ActaDto?> GetActaByIdAsync(Guid id)
    {
        var acta = _actas.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(acta != null ? MapToDto(acta) : null);
    }

    public Task<List<ActaDto>> GetActasByOrganoAsync(Guid organoId)
    {
        var result = _actas
            .Where(a => a.OrganoId == organoId)
            .Select(MapToDto)
            .ToList();
        return Task.FromResult(result);
    }

    // ========= Asambleas =========

    public Task<AsambleaDto> ConvocarAsambleaAsync(ConvocarAsambleaDto dto)
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
            Cerrada = false
        };
        _asambleas.Add(asamblea);
        return Task.FromResult(MapToDto(asamblea));
    }

    public Task<AsambleaDto?> GetAsambleaByIdAsync(Guid id)
    {
        var asamblea = _asambleas.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(asamblea != null ? MapToDto(asamblea) : null);
    }

    public Task<List<AsambleaDto>> GetAsambleasByOrganizationAsync(Guid organizationId)
    {
        var result = _asambleas
            .Where(a => a.OrganizationId == organizationId)
            .Select(MapToDto)
            .ToList();
        return Task.FromResult(result);
    }

    public Task<AsambleaDto> RegistrarAsistenciaAsync(Guid id, RegistrarAsistenciaDto dto)
    {
        var asamblea = _asambleas.FirstOrDefault(a => a.Id == id)
            ?? throw new KeyNotFoundException($"Asamblea {id} no encontrada");

        asamblea.Asistentes = dto.Asistentes;
        return Task.FromResult(MapToDto(asamblea));
    }

    public Task<AsambleaDto> CerrarAsambleaAsync(Guid id, CerrarAsambleaDto dto)
    {
        var asamblea = _asambleas.FirstOrDefault(a => a.Id == id)
            ?? throw new KeyNotFoundException($"Asamblea {id} no encontrada");

        asamblea.Cerrada = true;
        asamblea.FechaCierre = DateTime.UtcNow;
        asamblea.Resultados = dto.Resultados;
        return Task.FromResult(MapToDto(asamblea));
    }

    // ========= Voto =========

    public Task<VotoDto> EmitirVotoAsync(EmitirVotoDto dto)
    {
        var asamblea = _asambleas.FirstOrDefault(a => a.Id == dto.AsambleaId)
            ?? throw new KeyNotFoundException($"Asamblea {dto.AsambleaId} no encontrada");

        if (asamblea.Cerrada)
            throw new InvalidOperationException("La asamblea ya está cerrada, no se pueden recibir votos");

        if (_votos.Any(v => v.AsambleaId == dto.AsambleaId && v.AsociadoId == dto.AsociadoId))
            throw new InvalidOperationException("El asociado ya ha votado en esta asamblea");

        var voto = new Voto
        {
            Id = Guid.NewGuid(),
            AsambleaId = dto.AsambleaId,
            AsociadoId = dto.AsociadoId,
            VotoEmitido = dto.VotoEmitido,
            Fecha = DateTime.UtcNow,
            Observaciones = dto.Observaciones
        };
        _votos.Add(voto);
        return Task.FromResult(MapToDto(voto));
    }

    public Task<ResultadoVotacionDto> GetResultadosAsync(Guid asambleaId)
    {
        var votos = _votos.Where(v => v.AsambleaId == asambleaId).ToList();
        return Task.FromResult(new ResultadoVotacionDto
        {
            AsambleaId = asambleaId,
            TotalVotos = votos.Count,
            Aprobados = votos.Count(v => v.VotoEmitido == TipoVoto.Aprobado),
            Rechazados = votos.Count(v => v.VotoEmitido == TipoVoto.Rechazado),
            Abstenciones = votos.Count(v => v.VotoEmitido == TipoVoto.Abstencion),
            Blancos = votos.Count(v => v.VotoEmitido == TipoVoto.Blanco)
        });
    }

    public Task<bool> HaVotadoAsync(Guid asambleaId, Guid asociadoId)
    {
        var haVotado = _votos.Any(v => v.AsambleaId == asambleaId && v.AsociadoId == asociadoId);
        return Task.FromResult(haVotado);
    }

    // ========= Mapping =========

    private OrganoDto MapToDto(Organo o) => new()
    {
        Id = o.Id,
        Tipo = o.Tipo,
        Nombre = o.Nombre,
        OrganizationId = o.OrganizationId,
        FechaConstitucion = o.FechaConstitucion,
        Descripcion = o.Descripcion,
        Activo = o.Activo,
        MiembrosCount = _miembros.Count(m => m.OrganoId == o.Id),
        ActasCount = _actas.Count(a => a.OrganoId == o.Id)
    };

    private MiembroOrganoDto MapToDto(MiembroOrgano m) => new()
    {
        Id = m.Id,
        OrganoId = m.OrganoId,
        AsociadoId = m.AsociadoId,
        Cargo = m.Cargo,
        FechaInicio = m.FechaInicio,
        FechaFin = m.FechaFin,
        Activo = m.Activo
    };

    private ActaDto MapToDto(Acta a) => new()
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

    private AsambleaDto MapToDto(Asamblea a) => new()
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
        VotosCount = _votos.Count(v => v.AsambleaId == a.Id)
    };

    private VotoDto MapToDto(Voto v) => new()
    {
        Id = v.Id,
        AsambleaId = v.AsambleaId,
        AsociadoId = v.AsociadoId,
        VotoEmitido = v.VotoEmitido,
        Fecha = v.Fecha,
        Observaciones = v.Observaciones
    };
}
