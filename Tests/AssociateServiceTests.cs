using Application.Common.Exceptions;
using Application.DTOs.Solidarity;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests;

public class AssociateServiceTests : SolidarityTestBase
{
    private static TransitionAssociateRequest To(AssociateStatus status, string? date = null, string? reason = null) => new()
    {
        ToStatus = status,
        EffectiveDate = date == null ? null : DateOnly.Parse(date),
        Reason = reason
    };

    private async Task<Guid> NewApplicantAsync(string applicationDate = "2025-01-10")
    {
        var userId = await SeedUserAsync(OrgA);
        await AssociateService.CreateAsync(
            new CreateAssociateRequest { UserId = userId, ApplicationDate = DateOnly.Parse(applicationDate) }, OrgA, Admin);
        return userId;
    }

    [Fact]
    public async Task Create_ThenAdmit_SetsActiveWithAdmissionDate_AndNoWarningsWithoutEducationData()
    {
        var userId = await NewApplicantAsync();

        var result = await AssociateService.TransitionAsync(userId, To(AssociateStatus.Active, "2025-01-15"), OrgA, Admin);

        Assert.Equal(AssociateStatus.Active, result.Status);
        var detail = await AssociateService.GetByIdAsync(userId, OrgA);
        Assert.Equal(new DateOnly(2025, 1, 15), detail.CurrentAdmissionDate);
        Assert.Single(detail.Periods);
    }

    [Fact]
    public async Task Create_TwiceForSameUser_Conflicts()
    {
        var userId = await NewApplicantAsync();

        await Assert.ThrowsAsync<ConflictException>(() => AssociateService.CreateAsync(
            new CreateAssociateRequest { UserId = userId }, OrgA, Admin));
    }

    [Fact]
    public async Task CrossTenant_Read_Transition_AndCreate_BehaveAsNotFound()
    {
        var userId = await NewApplicantAsync();

        await Assert.ThrowsAsync<NotFoundException>(() => AssociateService.GetByIdAsync(userId, OrgB));
        await Assert.ThrowsAsync<NotFoundException>(() => AssociateService.TransitionAsync(userId, To(AssociateStatus.Active), OrgB, Admin));
        await Assert.ThrowsAsync<NotFoundException>(() => AssociateService.CreateAsync(new CreateAssociateRequest { UserId = userId }, OrgB, Admin));
        Assert.Empty(await AssociateService.GetAllAsync(OrgB, null));
        Assert.Single(await AssociateService.GetAllAsync(OrgA, null));
    }

    [Fact]
    public async Task InvalidTransition_Conflicts_AndStatusIsUnchanged()
    {
        var userId = await NewApplicantAsync();

        await Assert.ThrowsAsync<ConflictException>(() => AssociateService.TransitionAsync(userId, To(AssociateStatus.Suspended), OrgA, Admin));

        Assert.Equal(AssociateStatus.Applicant, (await AssociateService.GetByIdAsync(userId, OrgA)).Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task Withdraw_WithoutReason_IsBadRequest_AndNothingPersists(string? reason)
    {
        var userId = await NewApplicantAsync();

        await Assert.ThrowsAsync<BusinessValidationException>(() =>
            AssociateService.TransitionAsync(userId, To(AssociateStatus.Withdrawn, "2025-03-01", reason), OrgA, Admin));

        var detail = await AssociateService.GetByIdAsync(userId, OrgA);
        Assert.Equal(AssociateStatus.Applicant, detail.Status);
        Assert.Null(detail.Periods.Single().WithdrawalDate);
    }

    [Fact]
    public async Task Withdraw_BeforeAdmission_IsBadRequest()
    {
        var userId = await NewApplicantAsync();
        await AssociateService.TransitionAsync(userId, To(AssociateStatus.Active, "2025-01-15"), OrgA, Admin);

        await Assert.ThrowsAsync<BusinessValidationException>(() =>
            AssociateService.TransitionAsync(userId, To(AssociateStatus.Withdrawn, "2025-01-01", "voluntary"), OrgA, Admin));
    }

    [Fact]
    public async Task Readmit_KeepsClosedPeriodUnchanged_AndOpensSecondPeriod()
    {
        var userId = await NewApplicantAsync("2025-01-10");
        await AssociateService.TransitionAsync(userId, To(AssociateStatus.Active, "2025-01-15"), OrgA, Admin);
        await AssociateService.TransitionAsync(userId, To(AssociateStatus.Withdrawn, "2025-03-01", "voluntary"), OrgA, Admin);

        await AssociateService.TransitionAsync(userId, To(AssociateStatus.Applicant, "2026-01-05"), OrgA, Admin);
        await AssociateService.TransitionAsync(userId, To(AssociateStatus.Active, "2026-01-20"), OrgA, Admin);

        var detail = await AssociateService.GetByIdAsync(userId, OrgA);
        Assert.Equal(AssociateStatus.Active, detail.Status);
        Assert.Equal(2, detail.Periods.Count);

        var first = detail.Periods[0];
        Assert.Equal(new DateOnly(2025, 1, 10), first.ApplicationDate);
        Assert.Equal(new DateOnly(2025, 1, 15), first.AdmissionDate);
        Assert.Equal(new DateOnly(2025, 3, 1), first.WithdrawalDate);
        Assert.Equal("voluntary", first.WithdrawalReason);

        var second = detail.Periods[1];
        Assert.Equal(new DateOnly(2026, 1, 5), second.ApplicationDate);
        Assert.Equal(new DateOnly(2026, 1, 20), second.AdmissionDate);
        Assert.Null(second.WithdrawalDate);
        Assert.Equal(new DateOnly(2026, 1, 20), detail.CurrentAdmissionDate);
        Assert.Equal(2, await Context.MembershipPeriods.CountAsync(p => p.UserId == userId));
    }

    [Theory]
    [InlineData(true, AssociateStatus.Active, "Both")]
    [InlineData(false, AssociateStatus.Active, "Associate")]
    [InlineData(true, null, "Employee")]
    [InlineData(false, null, "None")]
    public async Task Role_IsDerivedFromEmployeeFlagAndAssociateRecord(bool isEmployee, AssociateStatus? status, string expected)
    {
        var userId = await SeedUserAsync(OrgA, isEmployee);
        if (status.HasValue)
        {
            await AssociateService.CreateAsync(new CreateAssociateRequest { UserId = userId, ApplicationDate = new DateOnly(2025, 1, 1) }, OrgA, Admin);
            await AssociateService.TransitionAsync(userId, To(AssociateStatus.Active, "2025-01-02"), OrgA, Admin);
        }

        var all = (await AssociateService.GetAllAsync(OrgA, null)).ToList();

        if (status.HasValue)
            Assert.Equal(expected, all.Single(a => a.UserId == userId).Role);
        else
            Assert.Equal(expected, AssociateServiceRole(isEmployee, status));
    }

    private static string AssociateServiceRole(bool isEmployee, AssociateStatus? status)
        => Domain.Entities.Solidarity.AssociateLifecycle.ResolveRole(isEmployee, status).ToString();
}
