using System;
using System.Collections.Generic;
using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public partial class DevManagerDbContext : DbContext
{
    public DevManagerDbContext(DbContextOptions<DevManagerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Certification> Certifications { get; set; }

    public virtual DbSet<EmployeeProfile> EmployeeProfiles { get; set; }

    public virtual DbSet<EmployeeSkill> EmployeeSkills { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectApplication> ProjectApplications { get; set; }

    public virtual DbSet<ProjectAssignment> ProjectAssignments { get; set; }

    public virtual DbSet<ProjectParticipation> ProjectParticipations { get; set; }

    public virtual DbSet<ProjectRole> ProjectRoles { get; set; }

    public virtual DbSet<ProjectSkillRequirement> ProjectSkillRequirements { get; set; }

    public virtual DbSet<RecommendationLog> RecommendationLogs { get; set; }

    public virtual DbSet<RecommendationRule> RecommendationRules { get; set; }

    public virtual DbSet<ReportSnapshot> ReportSnapshots { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<SkillEvaluation> SkillEvaluations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    // Agent (Reporting)
    public virtual DbSet<Domain.Entities.Agent.AgentAction> AgentActions { get; set; }

    public virtual DbSet<Domain.Entities.Agent.AgentConfiguration> AgentConfigurations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all entity configurations from the assembly
        // This automatically discovers and applies all IEntityTypeConfiguration<T> implementations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DevManagerDbContext).Assembly);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
