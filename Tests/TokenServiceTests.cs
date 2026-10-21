using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Domain.Entities.IAM;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Tests;

public class TokenServiceTests
{
    private const string Secret = "Test_Secret_Key_For_HMAC_SHA512_Must_Be_At_Least_64_Bytes_Long_1234567890";

    [Fact]
    public void GenerateToken_IncludesRoleClaims_RecognizedByIsInRole()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["JwtSettings:SecretKey"] = Secret })
            .Build();
        var user = new User { Id = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), Email = "a@b.com", FirstName = "A", LastName = "B" };

        var token = new TokenService(config).GenerateToken(user, new[] { "Admin", "Manager" });

        var principal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
            ValidIssuer = "DevManagerAPI",
            ValidAudience = "DevManagerClient"
        }, out _);

        Assert.True(principal.IsInRole("Admin"));
        Assert.True(principal.IsInRole("Manager"));
        Assert.False(principal.IsInRole("Developer"));
        Assert.Equal(user.OrganizationId.ToString(), principal.FindFirst("OrganizationId")?.Value);
    }
}
