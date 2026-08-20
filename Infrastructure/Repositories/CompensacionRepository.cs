namespace Infrastructure.Repositories;

using Domain.Entities.Nomina;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="ICompensacionRepository"/>
/// (Nomina usa patrón dual-entity: Domain POCO ↔ Infrastructure EF entity)
/// </summary>
public class CompensacionRepository : ICompensacionRepository
{
    private readonly DevManagerDbContext _context;

    public CompensacionRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<Compensacion> CreateAsync(Compensacion compensacion)
    {
        var efEntity = new Infrastructure.Data.Entities.Compensacion
        {
            Id = compensacion.Id,
            AsociadoId = compensacion.AsociadoId,
            OrganizationId = compensacion.OrganizationId,
            Periodo = compensacion.Periodo,
            Modelo = compensacion.Modelo,
            ValorBase = compensacion.ValorBase,
            ValorCalculado = compensacion.ValorCalculado,
            Observaciones = compensacion.Observaciones,
            CreatedAt = compensacion.CreatedAt,
            CreatedByUserId = compensacion.CreatedByUserId,
            UpdatedAt = compensacion.UpdatedAt,
            UpdatedByUserId = compensacion.UpdatedByUserId,
            IsDeleted = compensacion.IsDeleted,
            DeletedAt = compensacion.DeletedAt,
            DeletedByUserId = compensacion.DeletedByUserId
        };

        _context.Compensaciones.Add(efEntity);
        await _context.SaveChangesAsync();

        return MapToDomain(efEntity);
    }

    public async Task<List<Compensacion>> GetByAsociadoAsync(Guid asociadoId, int anio)
    {
        var efEntities = await _context.Compensaciones
            .AsNoTracking()
            .Where(c => c.AsociadoId == asociadoId && !c.IsDeleted && c.Periodo.Year == anio)
            .OrderByDescending(c => c.Periodo)
            .ToListAsync();

        return efEntities.Select(MapToDomain).ToList();
    }

    private static Compensacion MapToDomain(Infrastructure.Data.Entities.Compensacion e) => new()
    {
        Id = e.Id,
        AsociadoId = e.AsociadoId,
        OrganizationId = e.OrganizationId,
        Periodo = e.Periodo,
        Modelo = e.Modelo,
        ValorBase = e.ValorBase,
        ValorCalculado = e.ValorCalculado,
        Observaciones = e.Observaciones,
        CreatedAt = e.CreatedAt,
        CreatedByUserId = e.CreatedByUserId,
        UpdatedAt = e.UpdatedAt,
        UpdatedByUserId = e.UpdatedByUserId,
        IsDeleted = e.IsDeleted,
        DeletedAt = e.DeletedAt,
        DeletedByUserId = e.DeletedByUserId
    };
}