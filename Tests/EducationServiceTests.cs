using Xunit;
using Application.Common.Exceptions;
using Application.DTOs.Solidarity;
using Domain.Enums;

namespace Tests;

/// <summary>
/// Education service rules against real repositories (InMemory): validation, conflicts, tenant isolation, admit warning
/// </summary>
public class EducationServiceTests : SolidarityTestBase
{
    private static SaveEducationProgramRequest ProgramRequest(decimal hours = 20, EducationProgramType type = EducationProgramType.Basic) =>
        new() { Name = "Basic course", Type = type, RequiredHours = hours, StartDate = new DateOnly(2026, 1, 15) };

    private async Task<Guid> NewAssociateAsync(Guid orgId, AssociateStatus status = AssociateStatus.Applicant)
    {
        var userId = await SeedUserAsync(orgId);
        await AssociateService.CreateAsync(
            new CreateAssociateRequest { UserId = userId, ApplicationDate = new DateOnly(2025, 1, 1) }, orgId, Admin);
        if (status != AssociateStatus.Applicant)
        {
            await AssociateService.TransitionAsync(userId,
                new TransitionAssociateRequest { ToStatus = AssociateStatus.Active, EffectiveDate = new DateOnly(2025, 1, 10) }, orgId, Admin);
        }
        if (status == AssociateStatus.Withdrawn)
        {
            await AssociateService.TransitionAsync(userId,
                new TransitionAssociateRequest { ToStatus = AssociateStatus.Withdrawn, EffectiveDate = new DateOnly(2025, 6, 1), Reason = "voluntary" }, orgId, Admin);
        }
        return userId;
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task CreateProgram_WithNonPositiveHours_IsRejected(decimal hours)
    {
        await Assert.ThrowsAsync<BusinessValidationException>(() =>
            EducationService.CreateProgramAsync(ProgramRequest(hours), OrgA, Admin));
        Assert.Empty(await EducationService.GetProgramsAsync(OrgA));
    }

    [Fact]
    public async Task Enroll_WithdrawnAssociate_Conflicts()
    {
        var program = await EducationService.CreateProgramAsync(ProgramRequest(), OrgA, Admin);
        var userId = await NewAssociateAsync(OrgA, AssociateStatus.Withdrawn);

        await Assert.ThrowsAsync<ConflictException>(() =>
            EducationService.EnrollAsync(program.Id, new CreateEnrollmentRequest { UserId = userId }, OrgA, Admin));
    }

    [Fact]
    public async Task Enroll_TwiceInSameProgram_Conflicts()
    {
        var program = await EducationService.CreateProgramAsync(ProgramRequest(), OrgA, Admin);
        var userId = await NewAssociateAsync(OrgA);
        await EducationService.EnrollAsync(program.Id, new CreateEnrollmentRequest { UserId = userId }, OrgA, Admin);

        await Assert.ThrowsAsync<ConflictException>(() =>
            EducationService.EnrollAsync(program.Id, new CreateEnrollmentRequest { UserId = userId }, OrgA, Admin));
    }

    [Fact]
    public async Task Enroll_CompletedWithoutDate_IsRejected_AndNothingPersists()
    {
        var program = await EducationService.CreateProgramAsync(ProgramRequest(), OrgA, Admin);
        var userId = await NewAssociateAsync(OrgA);

        await Assert.ThrowsAsync<BusinessValidationException>(() => EducationService.EnrollAsync(program.Id,
            new CreateEnrollmentRequest { UserId = userId, Status = EnrollmentStatus.Completed, HoursCompleted = 20 }, OrgA, Admin));
        Assert.Empty(await EducationService.GetEnrollmentsAsync(program.Id, OrgA));
    }

    [Fact]
    public async Task UpdateEnrollment_NegativeHours_Rejected_AndCompletedWithDate_Succeeds()
    {
        var program = await EducationService.CreateProgramAsync(ProgramRequest(), OrgA, Admin);
        var userId = await NewAssociateAsync(OrgA);
        var enrollment = await EducationService.EnrollAsync(program.Id, new CreateEnrollmentRequest { UserId = userId }, OrgA, Admin);

        await Assert.ThrowsAsync<BusinessValidationException>(() => EducationService.UpdateEnrollmentAsync(enrollment.Id,
            new UpdateEnrollmentRequest { Status = EnrollmentStatus.Enrolled, HoursCompleted = -1 }, OrgA, Admin));

        var updated = await EducationService.UpdateEnrollmentAsync(enrollment.Id, new UpdateEnrollmentRequest
        {
            Status = EnrollmentStatus.Completed, HoursCompleted = 20, CompletionDate = new DateOnly(2026, 3, 1)
        }, OrgA, Admin);

        Assert.Equal(EnrollmentStatus.Completed, updated.Status);
        Assert.Equal(20m, updated.HoursCompleted);
        Assert.Equal(new DateOnly(2026, 3, 1), updated.CompletionDate);
    }

    [Fact]
    public async Task TenantIsolation_OtherOrgSeesProgramsAndEnrollmentsAsNotFound()
    {
        var program = await EducationService.CreateProgramAsync(ProgramRequest(), OrgA, Admin);
        var userId = await NewAssociateAsync(OrgA);
        var enrollment = await EducationService.EnrollAsync(program.Id, new CreateEnrollmentRequest { UserId = userId }, OrgA, Admin);

        Assert.Empty(await EducationService.GetProgramsAsync(OrgB));
        await Assert.ThrowsAsync<NotFoundException>(() => EducationService.GetProgramByIdAsync(program.Id, OrgB));
        await Assert.ThrowsAsync<NotFoundException>(() => EducationService.UpdateProgramAsync(program.Id, ProgramRequest(), OrgB, Admin));
        await Assert.ThrowsAsync<NotFoundException>(() => EducationService.DeleteProgramAsync(program.Id, OrgB, Admin));
        await Assert.ThrowsAsync<NotFoundException>(() => EducationService.GetEnrollmentsAsync(program.Id, OrgB));
        await Assert.ThrowsAsync<NotFoundException>(() => EducationService.UpdateEnrollmentAsync(enrollment.Id,
            new UpdateEnrollmentRequest { Status = EnrollmentStatus.Dropped }, OrgB, Admin));

        // An org B program cannot enroll an org A associate either
        var programB = await EducationService.CreateProgramAsync(ProgramRequest(), OrgB, Admin);
        await Assert.ThrowsAsync<NotFoundException>(() =>
            EducationService.EnrollAsync(programB.Id, new CreateEnrollmentRequest { UserId = userId }, OrgB, Admin));
    }

    [Fact]
    public async Task DeleteProgram_HidesProgramAndItsEnrollments()
    {
        var program = await EducationService.CreateProgramAsync(ProgramRequest(), OrgA, Admin);
        var userId = await NewAssociateAsync(OrgA);
        await EducationService.EnrollAsync(program.Id, new CreateEnrollmentRequest { UserId = userId }, OrgA, Admin);

        await EducationService.DeleteProgramAsync(program.Id, OrgA, Admin);

        Assert.Empty(await EducationService.GetProgramsAsync(OrgA));
        Assert.Empty(await Education.GetEnrollmentsAsync(OrgA));
    }

    [Fact]
    public async Task Admit_WithEightBasicHours_SucceedsWithWarning()
    {
        var program = await EducationService.CreateProgramAsync(ProgramRequest(), OrgA, Admin);
        var userId = await NewAssociateAsync(OrgA);
        await EducationService.EnrollAsync(program.Id, new CreateEnrollmentRequest
        {
            UserId = userId, Status = EnrollmentStatus.Completed, HoursCompleted = 8, CompletionDate = new DateOnly(2025, 1, 5)
        }, OrgA, Admin);

        var result = await AssociateService.TransitionAsync(userId,
            new TransitionAssociateRequest { ToStatus = AssociateStatus.Active, EffectiveDate = new DateOnly(2025, 2, 1) }, OrgA, Admin);

        Assert.Equal(AssociateStatus.Active, result.Status);
        Assert.Single(result.Warnings);
    }

    [Fact]
    public async Task Admit_WithTwentyBasicHours_HasNoWarning()
    {
        var program = await EducationService.CreateProgramAsync(ProgramRequest(), OrgA, Admin);
        var userId = await NewAssociateAsync(OrgA);
        await EducationService.EnrollAsync(program.Id, new CreateEnrollmentRequest
        {
            UserId = userId, Status = EnrollmentStatus.Completed, HoursCompleted = 20, CompletionDate = new DateOnly(2025, 1, 5)
        }, OrgA, Admin);

        var result = await AssociateService.TransitionAsync(userId,
            new TransitionAssociateRequest { ToStatus = AssociateStatus.Active, EffectiveDate = new DateOnly(2025, 2, 1) }, OrgA, Admin);

        Assert.Empty(result.Warnings);
    }
}
