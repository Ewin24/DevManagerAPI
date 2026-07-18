using Domain.Entities.Agent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration;

/// <summary>
/// Configuración de entidades del agente para Entity Framework
/// </summary>
public class AgentActionConfiguration : IEntityTypeConfiguration<AgentAction>
{
    public void Configure(EntityTypeBuilder<AgentAction> builder)
    {
        builder.ToTable("AgentActions", "reporting");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasDefaultValueSql("NEWID()");

        builder.Property(e => e.OrganizationId)
            .IsRequired();

        builder.Property(e => e.ActionType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.InputData)
            .IsRequired();

        builder.Property(e => e.OutputData)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.CreatedAt)
            .HasColumnType("datetime2(3)")
            .HasDefaultValueSql("sysutcdatetime()");

        builder.Property(e => e.ApprovedAt)
            .HasColumnType("datetime2(3)");

        // Índices
        builder.HasIndex(e => new { e.OrganizationId, e.Status, e.CreatedAt })
            .HasDatabaseName("IX_AgentActions_Org_Status_Date");

        builder.HasIndex(e => new { e.OrganizationId, e.ActionType })
            .HasDatabaseName("IX_AgentActions_Org_Type")
            .HasFilter("IsDeleted = 0");
    }
}

public class AgentConfigurationEntity : IEntityTypeConfiguration<AgentConfiguration>
{
    public void Configure(EntityTypeBuilder<AgentConfiguration> builder)
    {
        builder.ToTable("AgentConfiguration", "reporting");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasDefaultValueSql("NEWID()");

        builder.Property(e => e.OrganizationId)
            .IsRequired();

        builder.Property(e => e.ConfigKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.ConfigValue)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("datetime2(3)")
            .HasDefaultValueSql("sysutcdatetime()");

        // Índice único por organización y clave
        builder.HasIndex(e => new { e.OrganizationId, e.ConfigKey })
            .IsUnique()
            .HasDatabaseName("UX_AgentConfig_Org_Key");
    }
}
