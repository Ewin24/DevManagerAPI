namespace Application.Tests.Services;

using Domain.Entities.BalanceSocial;
using Domain.Entities.Bienestar;
using Domain.Entities.Excedentes;
using Domain.Entities.GestionHumana;
using Domain.Entities.HabeasData;
using Domain.Entities.Nomina;
using Domain.Entities.Organos;
using Domain.Entities.Reportes;
using Domain.Entities.SST;
using Domain.Interfaces.Repositories;

/// <summary>
/// Contenedor compartido de almacenes en memoria para los dobles de repositorio.
/// Permite resolver navegaciones entre entidades (ej. SolicitudBienestar.Programa).
/// </summary>
public sealed class InMemoryStores
{
    public List<Compensacion> Compensaciones { get; } = new();
    public List<PilaAporte> PilaAportes { get; } = new();
    public List<ProgramaBienestar> Programas { get; } = new();
    public List<SolicitudBienestar> Solicitudes { get; } = new();
    public List<Auxilio> Auxilios { get; } = new();
    public List<FondoSolidaridad> Fondos { get; } = new();
    public List<CompetenciaAsociado> Competencias { get; } = new();
    public List<ProgramaEducacion> ProgramasEducacion { get; } = new();
    public List<AsociadoEducacion> Inscripciones { get; } = new();
    public List<IndicadorBalanceSocial> Indicadores { get; } = new();
    public List<ExamenMedico> Examenes { get; } = new();
    public List<Accidente> Accidentes { get; } = new();
    public List<Riesgo> Riesgos { get; } = new();
    public List<Excedente> Excedentes { get; } = new();
    public List<Autorizacion> Autorizaciones { get; } = new();
    public List<SolicitudARCO> SolicitudesARCO { get; } = new();
    public List<ReporteSupersolidaria> Reportes { get; } = new();
    public List<Organo> Organos { get; } = new();
    public List<MiembroOrgano> Miembros { get; } = new();
    public List<Acta> Actas { get; } = new();
    public List<Asamblea> Asambleas { get; } = new();
    public List<Voto> Votos { get; } = new();
}

// ========= Nomina =========

public class InMemoryCompensacionRepository(InMemoryStores stores) : ICompensacionRepository
{
    public Task<Compensacion> CreateAsync(Compensacion compensacion)
    {
        stores.Compensaciones.Add(compensacion);
        return Task.FromResult(compensacion);
    }

    public Task<List<Compensacion>> GetByAsociadoAsync(Guid asociadoId, int anio)
        => Task.FromResult(stores.Compensaciones
            .Where(c => c.AsociadoId == asociadoId && !c.IsDeleted && c.Periodo.Year == anio)
            .OrderByDescending(c => c.Periodo)
            .ToList());
}

public class InMemoryPilaAporteRepository(InMemoryStores stores) : IPilaAporteRepository
{
    public Task<PilaAporte> CreateAsync(PilaAporte aporte)
    {
        stores.PilaAportes.Add(aporte);
        return Task.FromResult(aporte);
    }

    public Task<List<PilaAporte>> GetByOrganizationAndPeriodoAsync(Guid organizationId, int mes, int anio)
        => Task.FromResult(stores.PilaAportes
            .Where(p => p.OrganizationId == organizationId
                        && !p.IsDeleted
                        && p.Periodo.Year == anio
                        && p.Periodo.Month == mes)
            .OrderByDescending(p => p.Periodo)
            .ToList());
}

// ========= Bienestar =========

public class InMemoryProgramaBienestarRepository(InMemoryStores stores) : IProgramaBienestarRepository
{
    public Task<ProgramaBienestar> CreateAsync(ProgramaBienestar programa)
    {
        stores.Programas.Add(programa);
        return Task.FromResult(programa);
    }

    public Task<ProgramaBienestar?> GetByIdAsync(Guid id)
        => Task.FromResult(stores.Programas.FirstOrDefault(p => p.Id == id && !p.IsDeleted));

    public Task<List<ProgramaBienestar>> GetByOrganizationAsync(Guid organizationId)
        => Task.FromResult(stores.Programas
            .Where(p => p.OrganizationId == organizationId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToList());
}

public class InMemorySolicitudBienestarRepository(InMemoryStores stores) : ISolicitudBienestarRepository
{
    public Task<SolicitudBienestar> CreateAsync(SolicitudBienestar solicitud)
    {
        stores.Solicitudes.Add(solicitud);
        return Task.FromResult(solicitud);
    }

    public Task<SolicitudBienestar?> GetByIdAsync(Guid id)
    {
        var s = stores.Solicitudes.FirstOrDefault(s => s.Id == id && !s.IsDeleted);
        if (s != null)
            s.Programa = stores.Programas.FirstOrDefault(p => p.Id == s.ProgramaBienestarId);
        return Task.FromResult(s);
    }

    public Task<List<SolicitudBienestar>> GetByAsociadoAsync(Guid asociadoId)
        => Task.FromResult(stores.Solicitudes
            .Where(s => s.AsociadoId == asociadoId && !s.IsDeleted)
            .Select(s =>
            {
                s.Programa = stores.Programas.FirstOrDefault(p => p.Id == s.ProgramaBienestarId);
                return s;
            })
            .OrderByDescending(s => s.CreatedAt)
            .ToList());

    public Task<SolicitudBienestar> UpdateAsync(SolicitudBienestar solicitud)
    {
        var idx = stores.Solicitudes.FindIndex(s => s.Id == solicitud.Id);
        if (idx >= 0)
            stores.Solicitudes[idx] = solicitud;
        return Task.FromResult(solicitud);
    }
}

public class InMemoryAuxilioRepository(InMemoryStores stores) : IAuxilioRepository
{
    public Task<Auxilio> CreateAsync(Auxilio auxilio)
    {
        stores.Auxilios.Add(auxilio);
        return Task.FromResult(auxilio);
    }

    public Task<List<Auxilio>> GetByAsociadoAsync(Guid asociadoId)
        => Task.FromResult(stores.Auxilios
            .Where(a => a.AsociadoId == asociadoId && !a.IsDeleted)
            .OrderByDescending(a => a.FechaEntrega)
            .ToList());
}

public class InMemoryFondoSolidaridadRepository(InMemoryStores stores) : IFondoSolidaridadRepository
{
    public Task<FondoSolidaridad> CreateAsync(FondoSolidaridad fondo)
    {
        stores.Fondos.Add(fondo);
        return Task.FromResult(fondo);
    }

    public Task<FondoSolidaridad> UpdateAsync(FondoSolidaridad fondo)
    {
        var idx = stores.Fondos.FindIndex(f => f.Id == fondo.Id);
        if (idx >= 0)
            stores.Fondos[idx] = fondo;
        return Task.FromResult(fondo);
    }

    public Task<FondoSolidaridad?> GetByOrganizationAndPeriodoAsync(Guid organizationId, DateTime periodo)
        => Task.FromResult(stores.Fondos
            .FirstOrDefault(f => f.OrganizationId == organizationId && f.Periodo == periodo && !f.IsDeleted));

    public Task<FondoSolidaridad?> GetActualAsync(Guid organizationId)
        => Task.FromResult(stores.Fondos
            .Where(f => f.OrganizationId == organizationId && f.Vigente && !f.IsDeleted)
            .OrderByDescending(f => f.Periodo)
            .FirstOrDefault());
}

// ========= Gestión Humana =========

public class InMemoryCompetenciaAsociadoRepository(InMemoryStores stores) : ICompetenciaAsociadoRepository
{
    public Task<CompetenciaAsociado> CreateAsync(CompetenciaAsociado competencia)
    {
        stores.Competencias.Add(competencia);
        return Task.FromResult(competencia);
    }

    public Task<CompetenciaAsociado?> GetByIdAsync(Guid id)
        => Task.FromResult(stores.Competencias.FirstOrDefault(c => c.Id == id && !c.IsDeleted));

    public Task<List<CompetenciaAsociado>> GetByAsociadoAsync(Guid asociadoId)
        => Task.FromResult(stores.Competencias
            .Where(c => c.AsociadoId == asociadoId && !c.IsDeleted)
            .OrderByDescending(c => c.FechaActualizacion)
            .ToList());

    public Task<CompetenciaAsociado> UpdateAsync(CompetenciaAsociado competencia)
    {
        var idx = stores.Competencias.FindIndex(c => c.Id == competencia.Id);
        if (idx >= 0)
            stores.Competencias[idx] = competencia;
        return Task.FromResult(competencia);
    }

    public Task<List<CompetenciaAsociado>> SearchByCompetenciaAsync(string competencia, bool soloDisponibles)
    {
        var query = stores.Competencias
            .Where(c => !c.IsDeleted && c.Competencia.Contains(competencia, StringComparison.OrdinalIgnoreCase));
        if (soloDisponibles)
            query = query.Where(c => c.Disponible);
        return Task.FromResult(query.ToList());
    }
}

public class InMemoryProgramaEducacionRepository(InMemoryStores stores) : IProgramaEducacionRepository
{
    public Task<ProgramaEducacion> CreateAsync(ProgramaEducacion programa)
    {
        stores.ProgramasEducacion.Add(programa);
        return Task.FromResult(programa);
    }

    public Task<ProgramaEducacion?> GetByIdAsync(Guid id)
        => Task.FromResult(stores.ProgramasEducacion.FirstOrDefault(p => p.Id == id && !p.IsDeleted));

    public Task<List<ProgramaEducacion>> GetByOrganizationAsync(Guid organizationId)
        => Task.FromResult(stores.ProgramasEducacion
            .Where(p => p.OrganizationId == organizationId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToList());
}

public class InMemoryAsociadoEducacionRepository(InMemoryStores stores) : IAsociadoEducacionRepository
{
    public Task<AsociadoEducacion> CreateAsync(AsociadoEducacion inscripcion)
    {
        stores.Inscripciones.Add(inscripcion);
        return Task.FromResult(inscripcion);
    }

    public Task<AsociadoEducacion?> GetByIdAsync(Guid id)
    {
        var i = stores.Inscripciones.FirstOrDefault(i => i.Id == id && !i.IsDeleted);
        if (i != null)
            i.Programa = stores.ProgramasEducacion.FirstOrDefault(p => p.Id == i.ProgramaEducacionId);
        return Task.FromResult(i);
    }

    public Task<List<AsociadoEducacion>> GetByAsociadoAsync(Guid asociadoId)
        => Task.FromResult(stores.Inscripciones
            .Where(i => i.AsociadoId == asociadoId && !i.IsDeleted)
            .Select(i =>
            {
                i.Programa = stores.ProgramasEducacion.FirstOrDefault(p => p.Id == i.ProgramaEducacionId);
                return i;
            })
            .OrderByDescending(i => i.FechaInscripcion)
            .ToList());

    public Task<AsociadoEducacion> UpdateAsync(AsociadoEducacion inscripcion)
    {
        var idx = stores.Inscripciones.FindIndex(i => i.Id == inscripcion.Id);
        if (idx >= 0)
            stores.Inscripciones[idx] = inscripcion;
        return Task.FromResult(inscripcion);
    }
}

// ========= Balance Social =========

public class InMemoryIndicadorBalanceSocialRepository(InMemoryStores stores) : IIndicadorBalanceSocialRepository
{
    public Task<IndicadorBalanceSocial> CreateAsync(IndicadorBalanceSocial indicador)
    {
        stores.Indicadores.Add(indicador);
        return Task.FromResult(indicador);
    }

    public Task<IndicadorBalanceSocial> UpdateAsync(IndicadorBalanceSocial indicador)
    {
        var idx = stores.Indicadores.FindIndex(i => i.Id == indicador.Id);
        if (idx >= 0)
            stores.Indicadores[idx] = indicador;
        return Task.FromResult(indicador);
    }

    public Task<IndicadorBalanceSocial?> GetByAsociadoAndAnioAsync(Guid asociadoId, int anio)
        => Task.FromResult(stores.Indicadores
            .FirstOrDefault(i => i.AsociadoId == asociadoId && i.Anio == anio && !i.IsDeleted));

    public Task<List<IndicadorBalanceSocial>> GetByOrganizationAndAnioAsync(Guid organizationId, int anio)
        => Task.FromResult(stores.Indicadores
            .Where(i => i.OrganizationId == organizationId && i.Anio == anio && !i.IsDeleted)
            .ToList());
}

// ========= SST =========

public class InMemoryExamenMedicoRepository(InMemoryStores stores) : IExamenMedicoRepository
{
    public Task<ExamenMedico> CreateAsync(ExamenMedico examen)
    {
        stores.Examenes.Add(examen);
        return Task.FromResult(examen);
    }

    public Task<ExamenMedico> UpdateAsync(ExamenMedico examen)
    {
        var idx = stores.Examenes.FindIndex(e => e.Id == examen.Id);
        if (idx >= 0)
            stores.Examenes[idx] = examen;
        return Task.FromResult(examen);
    }

    public Task<ExamenMedico?> GetByIdAsync(Guid id)
        => Task.FromResult(stores.Examenes.FirstOrDefault(e => e.Id == id && !e.IsDeleted));

    public Task<List<ExamenMedico>> GetByAsociadoAsync(Guid asociadoId)
        => Task.FromResult(stores.Examenes
            .Where(e => e.AsociadoId == asociadoId && !e.IsDeleted)
            .OrderByDescending(e => e.FechaProgramado)
            .ToList());

    public Task<List<ExamenMedico>> GetPendientesAsync(Guid organizationId)
        => Task.FromResult(stores.Examenes
            .Where(e => e.OrganizationId == organizationId && !e.IsDeleted && e.FechaRealizado == null)
            .OrderBy(e => e.FechaProgramado)
            .ToList());
}

public class InMemoryAccidenteRepository(InMemoryStores stores) : IAccidenteRepository
{
    public Task<Accidente> CreateAsync(Accidente accidente)
    {
        stores.Accidentes.Add(accidente);
        return Task.FromResult(accidente);
    }

    public Task<Accidente> UpdateAsync(Accidente accidente)
    {
        var idx = stores.Accidentes.FindIndex(a => a.Id == accidente.Id);
        if (idx >= 0)
            stores.Accidentes[idx] = accidente;
        return Task.FromResult(accidente);
    }

    public Task<Accidente?> GetByIdAsync(Guid id)
        => Task.FromResult(stores.Accidentes.FirstOrDefault(a => a.Id == id && !a.IsDeleted));

    public Task<List<Accidente>> GetPendientesInvestigacionAsync(Guid organizationId)
        => Task.FromResult(stores.Accidentes
            .Where(a => a.OrganizationId == organizationId && !a.IsDeleted && !a.InvestigacionCompletada)
            .OrderByDescending(a => a.Fecha)
            .ToList());

    public Task<List<Accidente>> GetByOrganizationAsync(Guid organizationId)
        => Task.FromResult(stores.Accidentes
            .Where(a => a.OrganizationId == organizationId && !a.IsDeleted)
            .OrderByDescending(a => a.Fecha)
            .ToList());
}

public class InMemoryRiesgoRepository(InMemoryStores stores) : IRiesgoRepository
{
    public Task<Riesgo> CreateAsync(Riesgo riesgo)
    {
        stores.Riesgos.Add(riesgo);
        return Task.FromResult(riesgo);
    }

    public Task<List<Riesgo>> GetByOrganizationAsync(Guid organizationId)
        => Task.FromResult(stores.Riesgos
            .Where(r => r.OrganizationId == organizationId && !r.IsDeleted && r.Activo)
            .ToList());
}

// ========= Excedentes =========

public class InMemoryExcedenteRepository(InMemoryStores stores) : IExcedenteRepository
{
    public Task<Excedente> CreateAsync(Excedente excedente)
    {
        stores.Excedentes.Add(excedente);
        return Task.FromResult(excedente);
    }

    public Task<Excedente> UpdateAsync(Excedente excedente)
    {
        var idx = stores.Excedentes.FindIndex(e => e.Id == excedente.Id);
        if (idx >= 0)
            stores.Excedentes[idx] = excedente;
        return Task.FromResult(excedente);
    }

    public Task<Excedente?> GetByIdAsync(Guid id)
        => Task.FromResult(stores.Excedentes.FirstOrDefault(e => e.Id == id && !e.IsDeleted));

    public Task<Excedente?> GetByOrganizationAndPeriodoAsync(Guid organizationId, DateTime periodo)
        => Task.FromResult(stores.Excedentes
            .FirstOrDefault(e => e.OrganizationId == organizationId && e.Periodo == periodo && !e.IsDeleted));

    public Task<List<Excedente>> GetByOrganizationAsync(Guid organizationId)
        => Task.FromResult(stores.Excedentes
            .Where(e => e.OrganizationId == organizationId && !e.IsDeleted)
            .OrderByDescending(e => e.Periodo)
            .ToList());
}

// ========= Habeas Data =========

public class InMemoryAutorizacionRepository(InMemoryStores stores) : IAutorizacionRepository
{
    public Task<Autorizacion> CreateAsync(Autorizacion autorizacion)
    {
        stores.Autorizaciones.Add(autorizacion);
        return Task.FromResult(autorizacion);
    }

    public Task<Autorizacion> UpdateAsync(Autorizacion autorizacion)
    {
        var idx = stores.Autorizaciones.FindIndex(a => a.Id == autorizacion.Id);
        if (idx >= 0)
            stores.Autorizaciones[idx] = autorizacion;
        return Task.FromResult(autorizacion);
    }

    public Task<Autorizacion?> GetByIdAsync(Guid id)
        => Task.FromResult(stores.Autorizaciones.FirstOrDefault(a => a.Id == id && !a.IsDeleted));

    public Task<List<Autorizacion>> GetActiveByAsociadoAsync(Guid asociadoId)
        => Task.FromResult(stores.Autorizaciones
            .Where(a => a.AsociadoId == asociadoId && !a.Revocada && !a.IsDeleted)
            .ToList());

    public Task<Autorizacion?> GetVigenteByAsociadoAsync(Guid asociadoId)
        => Task.FromResult(stores.Autorizaciones
            .Where(a => a.AsociadoId == asociadoId
                        && !a.Revocada
                        && !a.IsDeleted
                        && (!a.Vigencia.HasValue || a.Vigencia >= DateTime.UtcNow))
            .OrderByDescending(a => a.FechaAutorizacion)
            .FirstOrDefault());

    public Task<bool> TieneVigenteAsync(Guid asociadoId)
        => Task.FromResult(stores.Autorizaciones
            .Any(a => a.AsociadoId == asociadoId
                      && !a.Revocada
                      && !a.IsDeleted
                      && (!a.Vigencia.HasValue || a.Vigencia >= DateTime.UtcNow)));
}

public class InMemorySolicitudARCORepository(InMemoryStores stores) : ISolicitudARCORepository
{
    public Task<SolicitudARCO> CreateAsync(SolicitudARCO solicitud)
    {
        stores.SolicitudesARCO.Add(solicitud);
        return Task.FromResult(solicitud);
    }

    public Task<SolicitudARCO> UpdateAsync(SolicitudARCO solicitud)
    {
        var idx = stores.SolicitudesARCO.FindIndex(s => s.Id == solicitud.Id);
        if (idx >= 0)
            stores.SolicitudesARCO[idx] = solicitud;
        return Task.FromResult(solicitud);
    }

    public Task<SolicitudARCO?> GetByIdAsync(Guid id)
        => Task.FromResult(stores.SolicitudesARCO.FirstOrDefault(s => s.Id == id && !s.IsDeleted));

    public Task<List<SolicitudARCO>> GetByAsociadoAsync(Guid asociadoId)
        => Task.FromResult(stores.SolicitudesARCO
            .Where(s => s.AsociadoId == asociadoId && !s.IsDeleted)
            .OrderByDescending(s => s.Fecha)
            .ToList());

    public Task<List<SolicitudARCO>> GetPendientesAsync(Guid organizationId)
        => Task.FromResult(stores.SolicitudesARCO
            .Where(s => s.OrganizationId == organizationId && s.Estado == Domain.Enums.EstadoSolicitudARCO.Pendiente && !s.IsDeleted)
            .OrderByDescending(s => s.Fecha)
            .ToList());
}

// ========= Reportes Supersolidaria =========

public class InMemoryReporteSupersolidariaRepository(InMemoryStores stores) : IReporteSupersolidariaRepository
{
    public Task<ReporteSupersolidaria> CreateAsync(ReporteSupersolidaria reporte)
    {
        stores.Reportes.Add(reporte);
        return Task.FromResult(reporte);
    }

    public Task<ReporteSupersolidaria> UpdateAsync(ReporteSupersolidaria reporte)
    {
        var idx = stores.Reportes.FindIndex(r => r.Id == reporte.Id);
        if (idx >= 0)
            stores.Reportes[idx] = reporte;
        return Task.FromResult(reporte);
    }

    public Task<ReporteSupersolidaria?> GetByIdAsync(Guid id)
        => Task.FromResult(stores.Reportes.FirstOrDefault(r => r.Id == id && !r.IsDeleted));

    public Task<ReporteSupersolidaria?> GetByOrganizationAndPeriodoAsync(Guid organizationId, DateTime periodo)
        => Task.FromResult(stores.Reportes
            .FirstOrDefault(r => r.OrganizationId == organizationId && r.Periodo == periodo && !r.IsDeleted));

    public Task<List<ReporteSupersolidaria>> GetByOrganizationAsync(Guid organizationId)
        => Task.FromResult(stores.Reportes
            .Where(r => r.OrganizationId == organizationId && !r.IsDeleted)
            .OrderByDescending(r => r.Periodo)
            .ToList());
}

// ========= Órganos =========

public class InMemoryOrganoRepository(InMemoryStores stores) : IOrganoRepository
{
    public Task<Organo> CreateAsync(Organo organo)
    {
        stores.Organos.Add(organo);
        return Task.FromResult(organo);
    }

    public Task<Organo> UpdateAsync(Organo organo)
    {
        var idx = stores.Organos.FindIndex(o => o.Id == organo.Id);
        if (idx >= 0)
            stores.Organos[idx] = organo;
        return Task.FromResult(organo);
    }

    public Task<Organo?> GetByIdAsync(Guid id)
        => Task.FromResult(stores.Organos.FirstOrDefault(o => o.Id == id && !o.IsDeleted));

    public Task<List<Organo>> GetByOrganizationAsync(Guid organizationId)
        => Task.FromResult(stores.Organos
            .Where(o => o.OrganizationId == organizationId && !o.IsDeleted)
            .ToList());

    public Task<List<Organo>> GetByTypeAsync(Guid organizationId, Domain.Enums.TipoOrgano tipo)
        => Task.FromResult(stores.Organos
            .Where(o => o.OrganizationId == organizationId && o.Tipo == tipo && !o.IsDeleted)
            .ToList());

    public Task<bool> DeleteAsync(Guid id)
    {
        var organo = stores.Organos.FirstOrDefault(o => o.Id == id && !o.IsDeleted);
        if (organo == null)
            return Task.FromResult(false);
        organo.IsDeleted = true;
        organo.DeletedAt = DateTime.UtcNow;
        return Task.FromResult(true);
    }
}

public class InMemoryMiembroOrganoRepository(InMemoryStores stores) : IMiembroOrganoRepository
{
    public Task<MiembroOrgano> CreateAsync(MiembroOrgano miembro)
    {
        stores.Miembros.Add(miembro);
        return Task.FromResult(miembro);
    }

    public Task<MiembroOrgano> UpdateAsync(MiembroOrgano miembro)
    {
        var idx = stores.Miembros.FindIndex(m => m.Id == miembro.Id);
        if (idx >= 0)
            stores.Miembros[idx] = miembro;
        return Task.FromResult(miembro);
    }

    public Task<MiembroOrgano?> GetByIdAsync(Guid id)
        => Task.FromResult(stores.Miembros.FirstOrDefault(m => m.Id == id && !m.IsDeleted));

    public Task<List<MiembroOrgano>> GetByOrganoAsync(Guid organoId)
        => Task.FromResult(stores.Miembros
            .Where(m => m.OrganoId == organoId && !m.IsDeleted)
            .ToList());

    public Task<int> CountByOrganoAsync(Guid organoId)
        => Task.FromResult(stores.Miembros.Count(m => m.OrganoId == organoId && !m.IsDeleted));

    public Task<bool> DeleteAsync(Guid id)
    {
        var miembro = stores.Miembros.FirstOrDefault(m => m.Id == id && !m.IsDeleted);
        if (miembro == null)
            return Task.FromResult(false);
        miembro.IsDeleted = true;
        miembro.DeletedAt = DateTime.UtcNow;
        return Task.FromResult(true);
    }
}

public class InMemoryActaRepository(InMemoryStores stores) : IActaRepository
{
    public Task<Acta> CreateAsync(Acta acta)
    {
        stores.Actas.Add(acta);
        return Task.FromResult(acta);
    }

    public Task<Acta?> GetByIdAsync(Guid id)
        => Task.FromResult(stores.Actas.FirstOrDefault(a => a.Id == id && !a.IsDeleted));

    public Task<List<Acta>> GetByOrganoAsync(Guid organoId)
        => Task.FromResult(stores.Actas
            .Where(a => a.OrganoId == organoId && !a.IsDeleted)
            .OrderByDescending(a => a.Fecha)
            .ToList());

    public Task<int> CountByOrganoAsync(Guid organoId)
        => Task.FromResult(stores.Actas.Count(a => a.OrganoId == organoId && !a.IsDeleted));
}

public class InMemoryAsambleaRepository(InMemoryStores stores) : IAsambleaRepository
{
    public Task<Asamblea> CreateAsync(Asamblea asamblea)
    {
        stores.Asambleas.Add(asamblea);
        return Task.FromResult(asamblea);
    }

    public Task<Asamblea> UpdateAsync(Asamblea asamblea)
    {
        var idx = stores.Asambleas.FindIndex(a => a.Id == asamblea.Id);
        if (idx >= 0)
            stores.Asambleas[idx] = asamblea;
        return Task.FromResult(asamblea);
    }

    public Task<Asamblea?> GetByIdAsync(Guid id)
        => Task.FromResult(stores.Asambleas.FirstOrDefault(a => a.Id == id && !a.IsDeleted));

    public Task<List<Asamblea>> GetByOrganizationAsync(Guid organizationId)
        => Task.FromResult(stores.Asambleas
            .Where(a => a.OrganizationId == organizationId && !a.IsDeleted)
            .OrderByDescending(a => a.Fecha)
            .ToList());
}

public class InMemoryVotoRepository(InMemoryStores stores) : IVotoRepository
{
    public Task<Voto> CreateAsync(Voto voto)
    {
        stores.Votos.Add(voto);
        return Task.FromResult(voto);
    }

    public Task<List<Voto>> GetByAsambleaAsync(Guid asambleaId)
        => Task.FromResult(stores.Votos
            .Where(v => v.AsambleaId == asambleaId && !v.IsDeleted)
            .ToList());

    public Task<int> CountByAsambleaAsync(Guid asambleaId)
        => Task.FromResult(stores.Votos.Count(v => v.AsambleaId == asambleaId && !v.IsDeleted));

    public Task<bool> ExistsByAsambleaAndAsociadoAsync(Guid asambleaId, Guid asociadoId)
        => Task.FromResult(stores.Votos
            .Any(v => v.AsambleaId == asambleaId && v.AsociadoId == asociadoId && !v.IsDeleted));
}