using Application.Common.Exceptions;
using Application.Services;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using EfProject = Infrastructure.Data.Entities.Project;

namespace Tests;

public class ProjectDeleteTests
{
    [Fact]
    public async Task DeleteProject_IsTenantScoped_AndSoftDeletes()
    {
        var options = new DbContextOptionsBuilder<DevManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new DevManagerDbContext(options);

        var ownerOrg = Guid.NewGuid();
        var otherOrg = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        context.Projects.Add(new EfProject { Id = projectId, OrganizationId = ownerOrg, Name = "P1", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var service = new ProjectService(new ProjectRepository(context), Mock.Of<ISkillRepository>());

        // Another tenant cannot delete it
        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteProjectAsync(projectId, otherOrg));
        Assert.False((await context.Projects.AsNoTracking().SingleAsync(p => p.Id == projectId)).IsDeleted);

        // Owner tenant soft-deletes it
        await service.DeleteProjectAsync(projectId, ownerOrg);
        Assert.True((await context.Projects.AsNoTracking().SingleAsync(p => p.Id == projectId)).IsDeleted);

        // Deleting again -> 404
        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteProjectAsync(projectId, ownerOrg));
    }
}
