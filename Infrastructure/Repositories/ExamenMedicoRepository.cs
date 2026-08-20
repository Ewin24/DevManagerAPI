namespace Infrastructure.Repositories;

using Domain.Entities.SST;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IExamenMedicoRepository"/>
/// </summary>
public class ExamenMedicoRepository : IExamenMedicoRepository
{
    private readonly DevManagerDbContext _context;

    public ExamenMedicoRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<ExamenMedico> CreateAsync(ExamenMedico examen)
    {
        _context.ExamenesMedicos.Add(examen);
        await _context.SaveChangesAsync();
        return examen;
    }

    public async Task<ExamenMedico> UpdateAsync(ExamenMedico examen)
    {
        var tracked = await _context.ExamenesMedicos
            .FirstOrDefaultAsync(e => e.Id == examen.Id && !e.IsDeleted);
        if (tracked != null)
        {
            _context.Entry(tracked).CurrentValues.SetValues(examen);
            await _context.SaveChangesAsync();
            return tracked;
        }
        return examen;
    }

    public async Task<ExamenMedico?> GetByIdAsync(Guid id)
    {
        return await _context.ExamenesMedicos
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
    }

    public async Task<List<ExamenMedico>> GetByAsociadoAsync(Guid asociadoId)
    {
        return await _context.ExamenesMedicos
            .AsNoTracking()
            .Where(e => e.AsociadoId == asociadoId && !e.IsDeleted)
            .OrderByDescending(e => e.FechaProgramado)
            .ToListAsync();
    }

    public async Task<List<ExamenMedico>> GetPendientesAsync(Guid organizationId)
    {
        return await _context.ExamenesMedicos
            .AsNoTracking()
            .Where(e => e.OrganizationId == organizationId && !e.IsDeleted && e.FechaRealizado == null)
            .OrderBy(e => e.FechaProgramado)
            .ToListAsync();
    }
}