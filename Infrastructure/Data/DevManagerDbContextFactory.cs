using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data;

/// <summary>
/// Design-time factory for EF Core tools (migrations, etc.).
/// Bypasses the API host so `dotnet ef` works without a real appsettings.
/// The connection string here is only used at design time; runtime uses
/// "ConnectionStrings:DefaultConnection" from the API configuration.
/// </summary>
public class DevManagerDbContextFactory : IDesignTimeDbContextFactory<DevManagerDbContext>
{
    public DevManagerDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<DevManagerDbContext>()
            .UseSqlServer(
                "Server=localhost;Database=GestionHumanaSolidaria;TrustServerCertificate=true;Integrated Security=true;MultipleActiveResultSets=true")
            .Options;

        return new DevManagerDbContext(options);
    }
}
