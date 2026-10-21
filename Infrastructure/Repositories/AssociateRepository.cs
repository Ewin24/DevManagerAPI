namespace Infrastructure.Repositories;

using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DomainSolidarity = Domain.Entities.Solidarity;
using EfEntities = Infrastructure.Data.Entities;

/// <summary>
/// Repositorio de asociados usando EF Core (siempre filtrado por OrganizationId)
/// </summary>
public class AssociateRepository : IAssociateRepository
{
    private readonly DevManagerDbContext _context;

    public AssociateRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DomainSolidarity.Associate>> GetAllAsync(Guid organizationId, AssociateStatus? status = null)
    {
        var query = _context.Associates
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.MembershipPeriods)
            .Where(a => a.OrganizationId == organizationId && !a.IsDeleted && !a.User.IsDeleted);

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == (byte)status.Value);
        }

        var efAssociates = await query.OrderBy(a => a.User.LastName).ThenBy(a => a.User.FirstName).ToListAsync();
        return efAssociates.Select(MapToDomain);
    }

    public async Task<DomainSolidarity.Associate?> GetByUserIdAsync(Guid userId, Guid organizationId)
    {
        var efAssociate = await _context.Associates
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.MembershipPeriods)
            .FirstOrDefaultAsync(a => a.UserId == userId && a.OrganizationId == organizationId && !a.IsDeleted);

        return efAssociate == null ? null : MapToDomain(efAssociate);
    }

    public async Task CreateAsync(DomainSolidarity.Associate associate)
    {
        var efAssociate = new EfEntities.Associate
        {
            UserId = associate.UserId,
            OrganizationId = associate.OrganizationId,
            Status = (byte)associate.Status,
            CreatedAt = associate.CreatedAt,
            CreatedByUserId = associate.CreatedByUserId
        };
        foreach (var period in associate.Periods)
        {
            efAssociate.MembershipPeriods.Add(MapPeriodToEf(period));
        }

        _context.Associates.Add(efAssociate);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DomainSolidarity.Associate associate)
    {
        var efAssociate = await _context.Associates
            .Include(a => a.MembershipPeriods)
            .FirstOrDefaultAsync(a => a.UserId == associate.UserId && a.OrganizationId == associate.OrganizationId && !a.IsDeleted);

        if (efAssociate == null)
            return;

        efAssociate.Status = (byte)associate.Status;
        efAssociate.UpdatedAt = DateTime.UtcNow;
        efAssociate.UpdatedByUserId = associate.UpdatedByUserId;

        foreach (var period in associate.Periods)
        {
            var efPeriod = efAssociate.MembershipPeriods.FirstOrDefault(p => p.Id == period.Id);
            if (efPeriod == null)
            {
                efAssociate.MembershipPeriods.Add(MapPeriodToEf(period));
                continue;
            }

            efPeriod.AdmissionDate = period.AdmissionDate;
            efPeriod.WithdrawalDate = period.WithdrawalDate;
            efPeriod.WithdrawalReason = period.WithdrawalReason;
            efPeriod.UpdatedAt = DateTime.UtcNow;
            efPeriod.UpdatedByUserId = associate.UpdatedByUserId;
        }

        await _context.SaveChangesAsync();
    }

    #region Mapping Helpers

    private static EfEntities.MembershipPeriod MapPeriodToEf(DomainSolidarity.MembershipPeriod period) => new()
    {
        Id = period.Id,
        OrganizationId = period.OrganizationId,
        UserId = period.UserId,
        ApplicationDate = period.ApplicationDate,
        AdmissionDate = period.AdmissionDate,
        WithdrawalDate = period.WithdrawalDate,
        WithdrawalReason = period.WithdrawalReason,
        CreatedAt = period.CreatedAt,
        CreatedByUserId = period.CreatedByUserId
    };

    private static DomainSolidarity.Associate MapToDomain(EfEntities.Associate ef) => new()
    {
        UserId = ef.UserId,
        OrganizationId = ef.OrganizationId,
        Status = (AssociateStatus)ef.Status,
        CreatedAt = ef.CreatedAt,
        CreatedByUserId = ef.CreatedByUserId,
        UpdatedAt = ef.UpdatedAt,
        UpdatedByUserId = ef.UpdatedByUserId,
        User = new Domain.Entities.IAM.User
        {
            Id = ef.User.Id,
            OrganizationId = ef.User.OrganizationId,
            FirstName = ef.User.FirstName,
            LastName = ef.User.LastName,
            Email = ef.User.Email,
            Phone = ef.User.Phone,
            IsActive = ef.User.IsActive,
            DocumentType = (DocumentType?)ef.User.DocumentType,
            DocumentNumber = ef.User.DocumentNumber,
            IsEmployee = ef.User.IsEmployee
        },
        Periods = ef.MembershipPeriods
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.ApplicationDate)
            .Select(p => new DomainSolidarity.MembershipPeriod
            {
                Id = p.Id,
                OrganizationId = p.OrganizationId,
                UserId = p.UserId,
                ApplicationDate = p.ApplicationDate,
                AdmissionDate = p.AdmissionDate,
                WithdrawalDate = p.WithdrawalDate,
                WithdrawalReason = p.WithdrawalReason,
                CreatedAt = p.CreatedAt,
                CreatedByUserId = p.CreatedByUserId
            }).ToList()
    };

    #endregion
}
