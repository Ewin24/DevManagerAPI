using Application.Common.Exceptions;
using Application.DTOs.Solidarity;
using Application.DTOs.Users;
using Domain.Enums;
using Xunit;

namespace Tests;

public class ConsentGateTests : SolidarityTestBase
{
    private static SetDocumentRequest Doc(string number = "123", DocumentType type = DocumentType.CC)
        => new() { DocumentType = type, DocumentNumber = number };

    private async Task GrantAsync(Guid orgId, Guid userId, Guid policyId)
        => await ConsentService.GrantAsync(userId, new GrantConsentRequest { PolicyVersionId = policyId }, orgId, Admin);

    [Fact]
    public async Task SetDocument_WithoutConsent_Is422_AndNothingSaved()
    {
        var userId = await SeedUserAsync(OrgA);

        await Assert.ThrowsAsync<UnprocessableEntityException>(() => UserService.SetDocumentAsync(userId, Doc(), OrgA, Admin));

        Assert.Null((await UserService.GetByIdAsync(userId, OrgA)).DocumentNumber);
    }

    [Fact]
    public async Task SetDocument_WithActiveConsent_Saves()
    {
        var userId = await SeedUserAsync(OrgA);
        await GrantAsync(OrgA, userId, await SeedPolicyAsync(OrgA));

        var user = await UserService.SetDocumentAsync(userId, Doc("123"), OrgA, Admin);

        Assert.Equal("123", user.DocumentNumber);
        Assert.Equal(DocumentType.CC, user.DocumentType);
    }

    [Fact]
    public async Task RevokedConsent_KeepsDocument_ButBlocksEdits()
    {
        var userId = await SeedUserAsync(OrgA);
        await GrantAsync(OrgA, userId, await SeedPolicyAsync(OrgA));
        await UserService.SetDocumentAsync(userId, Doc("123"), OrgA, Admin);
        var consent = (await ConsentService.GetUserConsentsAsync(userId, OrgA)).Single();

        var revoked = await ConsentService.RevokeAsync(consent.Id, new RevokeConsentRequest { Reason = "user request" }, OrgA, Admin);

        Assert.False(revoked.IsActive);
        Assert.Equal("user request", revoked.RevocationReason);
        Assert.NotNull(revoked.RevokedAt);
        await Assert.ThrowsAsync<UnprocessableEntityException>(() => UserService.SetDocumentAsync(userId, Doc("999"), OrgA, Admin));
        Assert.Equal("123", (await UserService.GetByIdAsync(userId, OrgA)).DocumentNumber);
        // Revoked record stays in the history
        Assert.Single(await ConsentService.GetUserConsentsAsync(userId, OrgA));
    }

    [Fact]
    public async Task Revoke_WithoutReason_Is400_AndAlreadyRevoked_Conflicts()
    {
        var userId = await SeedUserAsync(OrgA);
        await GrantAsync(OrgA, userId, await SeedPolicyAsync(OrgA));
        var consent = (await ConsentService.GetUserConsentsAsync(userId, OrgA)).Single();

        await Assert.ThrowsAsync<BusinessValidationException>(() =>
            ConsentService.RevokeAsync(consent.Id, new RevokeConsentRequest { Reason = " " }, OrgA, Admin));
        await ConsentService.RevokeAsync(consent.Id, new RevokeConsentRequest { Reason = "r" }, OrgA, Admin);
        await Assert.ThrowsAsync<ConflictException>(() =>
            ConsentService.RevokeAsync(consent.Id, new RevokeConsentRequest { Reason = "again" }, OrgA, Admin));
    }

    [Fact]
    public async Task DuplicateDocument_InSameOrg_Conflicts_ButOtherOrgSucceeds()
    {
        var u1 = await SeedUserAsync(OrgA);
        var u2 = await SeedUserAsync(OrgA);
        var other = await SeedUserAsync(OrgB);
        var policyA = await SeedPolicyAsync(OrgA);
        var policyB = await SeedPolicyAsync(OrgB);
        await GrantAsync(OrgA, u1, policyA);
        await GrantAsync(OrgA, u2, policyA);
        await GrantAsync(OrgB, other, policyB);

        await UserService.SetDocumentAsync(u1, Doc("123"), OrgA, Admin);

        await Assert.ThrowsAsync<ConflictException>(() => UserService.SetDocumentAsync(u2, Doc("123"), OrgA, Admin));
        Assert.Null((await UserService.GetByIdAsync(u2, OrgA)).DocumentNumber);
        var ok = await UserService.SetDocumentAsync(other, Doc("123"), OrgB, Admin);
        Assert.Equal("123", ok.DocumentNumber);
    }

    [Fact]
    public async Task SetDocument_InvalidInput_Is400()
    {
        var userId = await SeedUserAsync(OrgA);
        await GrantAsync(OrgA, userId, await SeedPolicyAsync(OrgA));

        await Assert.ThrowsAsync<BusinessValidationException>(() => UserService.SetDocumentAsync(userId, Doc("  "), OrgA, Admin));
        await Assert.ThrowsAsync<BusinessValidationException>(() => UserService.SetDocumentAsync(userId, Doc("1", (DocumentType)99), OrgA, Admin));
    }

    [Fact]
    public async Task SetDocument_CrossTenant_IsNotFound()
    {
        var userId = await SeedUserAsync(OrgA);

        await Assert.ThrowsAsync<NotFoundException>(() => UserService.SetDocumentAsync(userId, Doc(), OrgB, Admin));
    }

    [Fact]
    public async Task Grant_CrossTenantUserOrPolicy_IsNotFound_AndDuplicateActiveConflicts()
    {
        var userA = await SeedUserAsync(OrgA);
        var policyA = await SeedPolicyAsync(OrgA);
        var policyB = await SeedPolicyAsync(OrgB);

        await Assert.ThrowsAsync<NotFoundException>(() => GrantAsync(OrgB, userA, policyB));
        await Assert.ThrowsAsync<NotFoundException>(() => GrantAsync(OrgA, userA, policyB));
        await GrantAsync(OrgA, userA, policyA);
        await Assert.ThrowsAsync<ConflictException>(() => GrantAsync(OrgA, userA, policyA));
        await Assert.ThrowsAsync<NotFoundException>(() => ConsentService.GetUserConsentsAsync(userA, OrgB));
        await Assert.ThrowsAsync<NotFoundException>(() => ConsentService.GetUserConsentsAsync(Guid.NewGuid(), OrgA));
    }

    [Fact]
    public async Task Policy_Publish_IsUniquePerVersion_AndPublishedTextCannotBeEdited()
    {
        var created = await ConsentService.CreatePolicyAsync(
            new CreateDataPolicyRequest { Version = "v1", Purpose = "p", Text = "Original text" }, OrgA, Admin);

        await Assert.ThrowsAsync<ConflictException>(() => ConsentService.CreatePolicyAsync(
            new CreateDataPolicyRequest { Version = "v1", Purpose = "p", Text = "x" }, OrgA, Admin));
        await Assert.ThrowsAsync<ConflictException>(() => ConsentService.UpdatePolicyAsync(created.Id, OrgA));

        Assert.Equal("Original text", (await ConsentService.GetPoliciesAsync(OrgA)).Single().Text);
        Assert.Empty(await ConsentService.GetPoliciesAsync(OrgB));
        Assert.Null(await ConsentService.GetCurrentPolicyAsync(OrgB));
        Assert.Equal("v1", (await ConsentService.GetCurrentPolicyAsync(OrgA))!.Version);
    }
}
