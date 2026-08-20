namespace Infrastructure.Repositories;

using Domain.Entities.Nomina;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IPilaAporteRepository"/>
/// (Nomina usa patrón dual-entity: Domain POCO ↔ Infrastructure EF entity)
/// </summary>
public class PilaAporteRepository : IPilaAporteRepository
{
    private readonly DevManagerDbContext _context;

    public PilaAporteRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<PilaAporte> CreateAsync(PilaAporte aporte)
    {
        var efEntity = new Infrastructure.Data.Entities.PilaAporte
        {
            Id = aporte.Id,
            AsociadoId = aporte.AsociadoId,
            OrganizationId = aporte.OrganizationId,
            Periodo = aporte.Periodo,
            TipoAportante = aporte.TipoAportante,
            IngresoBase = aporte.IngresoBase,
            AporteEPS = aporte.AporteEPS,
            AportePension = aporte.AportePension,
            AporteARL = aporte.AporteARL,
            Total = aporte.Total,
            CreatedAt = aporte.CreatedAt,
            CreatedByUserId = aporte.CreatedByUserId,
            UpdatedAt = aporte.UpdatedAt,
            UpdatedByUserId = aporte.UpdatedByUserId,
            IsDeleted = aporte.IsDeleted,
            DeletedAt = aporte.DeletedAt,
            DeletedByUserId = aporte.DeletedByUserId
        };

        _context.PilaAportes.Add(efEntity);
        await _context.SaveChangesAsync();

        return MapToDomain(efEntity);
    }

    public async Task<List<PilaAporte>> GetByOrganizationAndPeriodoAsync(Guid organizationId, int mes, int anio)
    {
        var efEntities = await _context.PilaAportes
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId
                        && !p.IsDeleted
                        && p.Periodo.Year == anio
                        && p.Periodo.Month == mes)
            .OrderByDescending(p => p.Periodo)
            .ToListAsync();

        return efEntities.Select(MapToDomain).ToList();
    }

    private static PilaAporte MapToDomain(Infrastructure.Data.Entities.PilaAporte e) => new()
    {
        Id = e.Id,
        AsociadoId = e.AsociadoId,
        OrganizationId = e.OrganizationId,
        Periodo = e.Periodo,
        TipoAportante = e.TipoAportante,
        IngresoBase = e.IngresoBase,
        AporteEPS = e.AporteEPS,
        AportePension = e.AportePension,
        AporteARL = e.AporteARL,
        Total = e.Total,
        CreatedAt = e.CreatedAt,
        CreatedByUserId = e.CreatedByUserId,
        UpdatedAt = e.UpdatedAt,
        UpdatedByUserId = e.UpdatedByUserId,
        IsDeleted = e.IsDeleted,
        DeletedAt = e.DeletedAt,
        DeletedByUserId = e.DeletedByUserId
    };
}