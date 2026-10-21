using Application.Services;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using EfUser = Infrastructure.Data.Entities.User;

namespace Tests;

/// <summary>
/// Shared InMemory harness for the solidarity tests (real repositories, no mocks of persistence)
/// </summary>
public abstract class SolidarityTestBase : IAsyncDisposable
{
    protected readonly DevManagerDbContext Context;
    protected readonly Guid OrgA = Guid.NewGuid();
    protected readonly Guid OrgB = Guid.NewGuid();
    protected readonly Guid Admin = Guid.NewGuid();

    protected SolidarityTestBase()
    {
        var options = new DbContextOptionsBuilder<DevManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        Context = new DevManagerDbContext(options);
    }

    protected IUserRepository Users => new UserRepository(Context, NullLogger<UserRepository>.Instance);
    protected IAssociateRepository Associates => new AssociateRepository(Context);
    protected IConsentRepository Consents => new ConsentRepository(Context);

    protected IEducationRepository Education => new EducationRepository(Context);
    protected AssociateService AssociateService => new(Associates, Users, Consents, Education);
    protected EducationService EducationService => new(Education, Associates);
    protected ConsentService ConsentService => new(Consents, Users);
    protected UserService UserService => new(Users, Mock.Of<IRolePermissionRepository>(), Consents, NullLogger<UserService>.Instance);

    protected async Task<Guid> SeedUserAsync(Guid orgId, bool isEmployee = true)
    {
        var id = Guid.NewGuid();
        Context.Users.Add(new EfUser
        {
            Id = id,
            OrganizationId = orgId,
            FirstName = "First",
            LastName = "Last",
            Email = $"{id}@test.com",
            PasswordHash = new byte[1],
            PasswordSalt = new byte[1],
            IsActive = true,
            IsEmployee = isEmployee,
            CreatedAt = DateTime.UtcNow
        });
        await Context.SaveChangesAsync();
        return id;
    }

    protected async Task<Guid> SeedPolicyAsync(Guid orgId, string version = "v1")
    {
        var policy = new Domain.Entities.Solidarity.DataPolicyVersion
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            Version = version,
            Purpose = "Membership management",
            Text = "Original text",
            EffectiveFrom = new DateOnly(2026, 1, 1),
            CreatedAt = DateTime.UtcNow
        };
        await Consents.CreatePolicyAsync(policy);
        return policy.Id;
    }

    public async ValueTask DisposeAsync() => await Context.DisposeAsync();
}
