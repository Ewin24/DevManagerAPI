namespace Infrastructure.Repositories;

using Domain.Entities.Reportes;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IReporteSupersolidariaRepository"/>
/// </summary>
public class ReporteSupersolidariaRepository : IReporteSupersolidariaRepository
{
    private readonly DevManagerDbContext _context;

    public ReporteSupersolidariaRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<ReporteSupersolidaria> CreateAsync(ReporteSupersolidaria reporte)
    {
        _context.ReportesSupersolidaria.Add(reporte);
        await _context.SaveChangesAsync();
        return reporte;
    }

    public async Task<ReporteSupersolidaria> UpdateAsync(ReporteSupersolidaria reporte)
    {
        var tracked = await _context.ReportesSupersolidaria
            .FirstOrDefaultAsync(r => r.Id == reporte.Id && !r.IsDeleted);
        if (tracked != null)
        {
            _context.Entry(tracked).CurrentValues.SetValues(reporte);
            await _context.SaveChangesAsync();
            return tracked;
        }
        return reporte;
    }

    public async Task<ReporteSupersolidaria?> GetByIdAsync(Guid id)
    {
        return await _context.ReportesSupersolidaria
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }

    public async Task<ReporteSupersolidaria?> GetByOrganizationAndPeriodoAsync(Guid organizationId, DateTime periodo)
    {
        return await _context.ReportesSupersolidaria
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.OrganizationId == organizationId && r.Periodo == periodo && !r.IsDeleted);
    }

    public async Task<List<ReporteSupersolidaria>> GetByOrganizationAsync(Guid organizationId)
    {
        return await _context.ReportesSupersolidaria
            .AsNoTracking()
            .Where(r => r.OrganizationId == organizationId && !r.IsDeleted)
            .OrderByDescending(r => r.Periodo)
            .ToListAsync();
    }
}