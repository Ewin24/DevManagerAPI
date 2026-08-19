using Infrastructure.Data.Entities;
using Infrastructure.Data.Entities.Config;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public partial class DevManagerDbContext : DbContext
{
    public DevManagerDbContext(DbContextOptions<DevManagerDbContext> options)
        : base(options)
    {
    }

    // ========= IAM =========
    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<PersonOrganization> PersonOrganizations { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }

    // ========= Talent =========
    public virtual DbSet<Certification> Certifications { get; set; }

    public virtual DbSet<EmployeeProfile> EmployeeProfiles { get; set; }

    public virtual DbSet<EmployeeSkill> EmployeeSkills { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<SkillEvaluation> SkillEvaluations { get; set; }

    // ========= Projects =========
    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectApplication> ProjectApplications { get; set; }

    public virtual DbSet<ProjectAssignment> ProjectAssignments { get; set; }

    public virtual DbSet<ProjectParticipation> ProjectParticipations { get; set; }

    public virtual DbSet<ProjectRole> ProjectRoles { get; set; }

    public virtual DbSet<ProjectSkillRequirement> ProjectSkillRequirements { get; set; }

    // ========= Reporting =========
    public virtual DbSet<RecommendationLog> RecommendationLogs { get; set; }

    public virtual DbSet<RecommendationRule> RecommendationRules { get; set; }

    public virtual DbSet<ReportSnapshot> ReportSnapshots { get; set; }

    // ========= Agent =========
    public virtual DbSet<Domain.Entities.Agent.AgentAction> AgentActions { get; set; }

    public virtual DbSet<Domain.Entities.Agent.AgentConfiguration> AgentConfigurations { get; set; }

    // ========= Nomina (Sector Solidario) =========
    public virtual DbSet<Compensacion> Compensaciones { get; set; }

    public virtual DbSet<PilaAporte> PilaAportes { get; set; }

    // ========= Bienestar y Compensacion (Sector Solidario) =========
    public virtual DbSet<Domain.Entities.Bienestar.ProgramaBienestar> ProgramasBienestar { get; set; }

    public virtual DbSet<Domain.Entities.Bienestar.SolicitudBienestar> SolicitudesBienestar { get; set; }

    public virtual DbSet<Domain.Entities.Bienestar.FondoSolidaridad> FondosSolidaridad { get; set; }

    public virtual DbSet<Domain.Entities.Bienestar.Auxilio> Auxilios { get; set; }

    // ========= Gestion Humana Solidaria =========
    public virtual DbSet<Domain.Entities.GestionHumana.CompetenciaAsociado> CompetenciasAsociado { get; set; }

    public virtual DbSet<Domain.Entities.GestionHumana.ProgramaEducacion> ProgramasEducacion { get; set; }

    public virtual DbSet<Domain.Entities.GestionHumana.AsociadoEducacion> AsociadosEducacion { get; set; }

    // ========= Balance Social =========
    public virtual DbSet<Domain.Entities.BalanceSocial.IndicadorBalanceSocial> IndicadoresBalanceSocial { get; set; }

    // ========= Seguridad y Salud en el Trabajo (SST) =========
    public virtual DbSet<Domain.Entities.SST.ExamenMedico> ExamenesMedicos { get; set; }

    public virtual DbSet<Domain.Entities.SST.Accidente> Accidentes { get; set; }

    public virtual DbSet<Domain.Entities.SST.Riesgo> Riesgos { get; set; }

    // ========= Excedentes =========
    public virtual DbSet<Domain.Entities.Excedentes.Excedente> Excedentes { get; set; }

    // ========= Habeas Data =========
    public virtual DbSet<Domain.Entities.HabeasData.Autorizacion> Autorizaciones { get; set; }

    public virtual DbSet<Domain.Entities.HabeasData.SolicitudARCO> SolicitudesARCO { get; set; }

    // ========= Reportes Supersolidaria =========
    public virtual DbSet<Domain.Entities.Reportes.ReporteSupersolidaria> ReportesSupersolidaria { get; set; }

    // ========= Organos de Administracion =========
    public virtual DbSet<Domain.Entities.Organos.Organo> Organos { get; set; }

    public virtual DbSet<Domain.Entities.Organos.MiembroOrgano> MiembrosOrgano { get; set; }

    public virtual DbSet<Domain.Entities.Organos.Acta> Actas { get; set; }

    public virtual DbSet<Domain.Entities.Organos.Asamblea> Asambleas { get; set; }

    public virtual DbSet<Domain.Entities.Organos.Voto> Votos { get; set; }

    // ========= Asistente Cooperativo (IA) =========
    public virtual DbSet<Domain.Entities.Agent.HerramientaCooperativa> HerramientasCooperativas { get; set; }

    // ========= Config (Catálogos) =========
    public virtual DbSet<ProjectStatusConfig> ProjectStatuses { get; set; }

    public virtual DbSet<ProjectComplexityLevelConfig> ProjectComplexityLevels { get; set; }

    public virtual DbSet<ApplicationStatusConfig> ApplicationStatuses { get; set; }

    public virtual DbSet<AssignmentStatusConfig> AssignmentStatuses { get; set; }

    public virtual DbSet<SkillLevelConfig> SkillLevels { get; set; }

    public virtual DbSet<ContributionScoreConfig> ContributionScores { get; set; }

    public virtual DbSet<EvaluationSourceConfig> EvaluationSources { get; set; }

    public virtual DbSet<SkillTypeConfig> SkillTypes { get; set; }

    public virtual DbSet<SkillCategoryConfig> SkillCategories { get; set; }

    public virtual DbSet<AgentActionTypeConfig> AgentActionTypes { get; set; }

    public virtual DbSet<AgentActionStatusConfig> AgentActionStatuses { get; set; }

    public virtual DbSet<SeniorityLevelConfig> SeniorityLevels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all entity configurations from the assembly
        // This automatically discovers and applies all IEntityTypeConfiguration<T> implementations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DevManagerDbContext).Assembly);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}