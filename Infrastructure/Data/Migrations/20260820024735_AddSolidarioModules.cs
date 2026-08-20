using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSolidarioModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sst");

            migrationBuilder.EnsureSchema(
                name: "reporting");

            migrationBuilder.EnsureSchema(
                name: "config");

            migrationBuilder.EnsureSchema(
                name: "gestion_humana");

            migrationBuilder.EnsureSchema(
                name: "habeasdata");

            migrationBuilder.EnsureSchema(
                name: "bienestar");

            migrationBuilder.EnsureSchema(
                name: "talent");

            migrationBuilder.EnsureSchema(
                name: "nomina");

            migrationBuilder.EnsureSchema(
                name: "excedentes");

            migrationBuilder.EnsureSchema(
                name: "balance_social");

            migrationBuilder.EnsureSchema(
                name: "iam");

            migrationBuilder.EnsureSchema(
                name: "projects");

            migrationBuilder.EnsureSchema(
                name: "reportes");

            migrationBuilder.CreateTable(
                name: "Accidentes",
                schema: "sst",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AsociadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gravedad = table.Column<byte>(type: "tinyint", nullable: false),
                    ARL = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    FURAT = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaInvestigacion = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    InvestigacionCompletada = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Conclusiones = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Causas = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MedidasCorrectivas = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accidentes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgentActions",
                schema: "reporting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    InputData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExecutedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgentActionStatuses",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsFinalState = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentActionStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgentActionTypes",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentActionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgentConfiguration",
                schema: "reporting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfigKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConfigValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationStatuses",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequiresReviewNotes = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsFinalState = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentStatuses",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsFinalState = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutorizacionesHabeasData",
                schema: "habeasdata",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AsociadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaAutorizacion = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    Vigencia = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    Revocada = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaRevocacion = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    Finalidad = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MedioAutorizacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Digital"),
                    DireccionIp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutorizacionesHabeasData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Compensaciones",
                schema: "nomina",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AsociadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Periodo = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    Modelo = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ValorBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorCalculado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compensaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompetenciasAsociado",
                schema: "gestion_humana",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AsociadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Competencia = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    Disponible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetenciasAsociado", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContributionScores",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExperienceBonus = table.Column<decimal>(type: "decimal(3,2)", nullable: false, defaultValue: 0.0m),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContributionScores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationSources",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsAutomated = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExamenesMedicos",
                schema: "sst",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AsociadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoExamen = table.Column<byte>(type: "tinyint", nullable: false),
                    FechaProgramado = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    FechaRealizado = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    Resultado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ArchivoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamenesMedicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Excedentes",
                schema: "excedentes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Periodo = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    TotalExcedentes = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReservaProteccionAportes = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FondoEducacion = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FondoSolidaridad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Revalorizacion = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RetornoCooperativo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AprobadoPorAsamblea = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Excedentes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FondoSolidaridad",
                schema: "bienestar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Periodo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalExcedentes = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AporteFondo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaldoDisponible = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDesembolsado = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Vigente = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FondoSolidaridad", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HerramientasCooperativas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InputSchema = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlEndpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activa = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HerramientasCooperativas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndicadoresBalanceSocial",
                schema: "balance_social",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AsociadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    HorasEducacion = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ParticipacionAsambleas = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ParticipacionComites = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    AportesSociales = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    BeneficiosRecibidos = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CumpleEducacion = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IndiceBalanceSocial = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicadoresBalanceSocial", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                schema: "iam",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Nit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TipoOrganizacionSolidaria = table.Column<int>(type: "int", nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HierarchyLevel = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_Parent",
                        column: x => x.ParentId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Organos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaConstitucion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "iam",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Module = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PilaAportes",
                schema: "nomina",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AsociadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Periodo = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    TipoAportante = table.Column<int>(type: "int", nullable: false, defaultValue: 51),
                    IngresoBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AporteEPS = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AportePension = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AporteARL = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PilaAportes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgramasBienestar",
                schema: "bienestar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Presupuesto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MaxBeneficiarios = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramasBienestar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgramasEducacion",
                schema: "gestion_humana",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Tipo = table.Column<byte>(type: "tinyint", nullable: false),
                    Horas = table.Column<int>(type: "int", nullable: false),
                    EsObligatorio = table.Column<bool>(type: "bit", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramasEducacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectComplexityLevels",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExperienceMultiplier = table.Column<decimal>(type: "decimal(3,2)", nullable: false, defaultValue: 1.0m),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectComplexityLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectStatuses",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AllowsApplications = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportesSupersolidaria",
                schema: "reportes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Periodo = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    BalanceSocialJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AsociadosJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CumplimientoJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoReporte = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Trimestral"),
                    Enviado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesSupersolidaria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Riesgos",
                schema: "sst",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NivelRiesgo = table.Column<int>(type: "int", nullable: false),
                    Factor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Controles = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Riesgos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeniorityLevels",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MinYearsExperience = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    MaxYearsExperience = table.Column<byte>(type: "tinyint", nullable: true),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeniorityLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillCategories",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillCategories_Parent",
                        column: x => x.ParentCategoryId,
                        principalSchema: "config",
                        principalTable: "SkillCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SkillLevels",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    MinYearsExperience = table.Column<byte>(type: "tinyint", nullable: true),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillTypes",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesARCO",
                schema: "habeasdata",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AsociadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<byte>(type: "tinyint", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false, defaultValueSql: "1"),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Respuesta = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    Radicado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesARCO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecommendationRules",
                schema: "reporting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ConditionExpr = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    RecommendationText = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecommendationRules_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReportSnapshots",
                schema: "reporting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SnapshotDate = table.Column<DateOnly>(type: "date", nullable: false),
                    JsonPayload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportSnapshots_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "iam",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                schema: "talent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    SkillType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skills_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "iam",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(512)", maxLength: 512, nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    PersonType = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    DocumentType = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Asambleas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Convocatoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuorumMinimo = table.Column<int>(type: "int", nullable: false),
                    Asistentes = table.Column<int>(type: "int", nullable: true),
                    Cerrada = table.Column<bool>(type: "bit", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Resultados = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asambleas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asambleas_Organos_OrganoId",
                        column: x => x.OrganoId,
                        principalTable: "Organos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MiembrosOrgano",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AsociadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiembrosOrgano", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiembrosOrgano_Organos_OrganoId",
                        column: x => x.OrganoId,
                        principalTable: "Organos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesBienestar",
                schema: "bienestar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AsociadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramaBienestarId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TipoAuxilio = table.Column<byte>(type: "tinyint", nullable: false),
                    MontoSolicitado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoAprobado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    Motivo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FechaRequerida = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    ObservacionesResolucion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ResueltoPorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesBienestar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesBienestar_ProgramasBienestar_ProgramaBienestarId",
                        column: x => x.ProgramaBienestarId,
                        principalSchema: "bienestar",
                        principalTable: "ProgramasBienestar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AsociadosEducacion",
                schema: "gestion_humana",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AsociadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramaEducacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HorasCursadas = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Progreso = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m),
                    FechaInscripcion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCompletado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Completado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Resultado = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsociadosEducacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsociadosEducacion_ProgramasEducacion_ProgramaEducacionId",
                        column: x => x.ProgramaEducacionId,
                        principalSchema: "gestion_humana",
                        principalTable: "ProgramasEducacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ComplexityLevel = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Complexity",
                        column: x => x.ComplexityLevel,
                        principalSchema: "config",
                        principalTable: "ProjectComplexityLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Projects_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Projects_Status",
                        column: x => x.Status,
                        principalSchema: "config",
                        principalTable: "ProjectStatuses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                schema: "iam",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "iam",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "iam",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certifications",
                schema: "talent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Issuer = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvidenceUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certifications_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Certifications_Users",
                        column: x => x.UserId,
                        principalSchema: "iam",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProfiles",
                schema: "talent",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
                    YearsExperience = table.Column<int>(type: "int", nullable: true),
                    LinkedInUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PortfolioUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProfiles", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_EmployeeProfiles_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmployeeProfiles_Users",
                        column: x => x.UserId,
                        principalSchema: "iam",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSkills",
                schema: "talent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    EvidenceUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    ExperienceDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LastValidatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    ValidatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSkills_Level",
                        column: x => x.Level,
                        principalSchema: "config",
                        principalTable: "SkillLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmployeeSkills_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmployeeSkills_Skills",
                        column: x => x.SkillId,
                        principalSchema: "talent",
                        principalTable: "Skills",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmployeeSkills_Users",
                        column: x => x.UserId,
                        principalSchema: "iam",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmployeeSkills_Validator",
                        column: x => x.ValidatedByUserId,
                        principalSchema: "iam",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PersonOrganizations",
                schema: "iam",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MembershipType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonOrganizations", x => new { x.PersonId, x.OrganizationId });
                    table.ForeignKey(
                        name: "FK_PersonOrganizations_Organization",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonOrganizations_Person",
                        column: x => x.PersonId,
                        principalSchema: "iam",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecommendationLogs",
                schema: "reporting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    GeneratedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResultJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecommendationLogs_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecommendationLogs_Users",
                        column: x => x.GeneratedByUserId,
                        principalSchema: "iam",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                schema: "iam",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => new { x.UserId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_UserPermissions_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "iam",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "iam",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "iam",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles",
                        column: x => x.RoleId,
                        principalSchema: "iam",
                        principalTable: "Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserRoles_Users",
                        column: x => x.UserId,
                        principalSchema: "iam",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Actas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AsambleaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoSesion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quorum = table.Column<int>(type: "int", nullable: false),
                    Decisiones = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConvocatoriaUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActaUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actas_Asambleas_AsambleaId",
                        column: x => x.AsambleaId,
                        principalTable: "Asambleas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Actas_Organos_OrganoId",
                        column: x => x.OrganoId,
                        principalTable: "Organos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AsambleaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AsociadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VotoEmitido = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votos_Asambleas_AsambleaId",
                        column: x => x.AsambleaId,
                        principalTable: "Asambleas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Auxilios",
                schema: "bienestar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AsociadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolicitudBienestarId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FondoSolidaridadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Tipo = table.Column<byte>(type: "tinyint", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RequiereReintegro = table.Column<bool>(type: "bit", nullable: false),
                    FechaLimiteReintegro = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auxilios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auxilios_FondoSolidaridad_FondoSolidaridadId",
                        column: x => x.FondoSolidaridadId,
                        principalSchema: "bienestar",
                        principalTable: "FondoSolidaridad",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Auxilios_SolicitudesBienestar_SolicitudBienestarId",
                        column: x => x.SolicitudBienestarId,
                        principalSchema: "bienestar",
                        principalTable: "SolicitudesBienestar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProjectApplications",
                schema: "projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Motivation = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectApplications_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectApplications_Projects",
                        column: x => x.ProjectId,
                        principalSchema: "projects",
                        principalTable: "Projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectApplications_Status",
                        column: x => x.Status,
                        principalSchema: "config",
                        principalTable: "ApplicationStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectApplications_Users",
                        column: x => x.UserId,
                        principalSchema: "iam",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectApplications_Users_ReviewedBy",
                        column: x => x.ReviewedByUserId,
                        principalSchema: "iam",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectParticipation",
                schema: "projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    ContributionScore = table.Column<byte>(type: "tinyint", nullable: true),
                    FeedbackComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectParticipation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectParticipation_ContributionScore",
                        column: x => x.ContributionScore,
                        principalSchema: "config",
                        principalTable: "ContributionScores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectParticipation_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectParticipation_Projects",
                        column: x => x.ProjectId,
                        principalSchema: "projects",
                        principalTable: "Projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectParticipation_Users",
                        column: x => x.UserId,
                        principalSchema: "iam",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectRoles",
                schema: "projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    NeededCount = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectRoles_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectRoles_Projects",
                        column: x => x.ProjectId,
                        principalSchema: "projects",
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectSkillRequirements",
                schema: "projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequiredLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSkillRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectSkillRequirements_Level",
                        column: x => x.RequiredLevel,
                        principalSchema: "config",
                        principalTable: "SkillLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectSkillRequirements_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectSkillRequirements_Projects",
                        column: x => x.ProjectId,
                        principalSchema: "projects",
                        principalTable: "Projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectSkillRequirements_Skills",
                        column: x => x.SkillId,
                        principalSchema: "talent",
                        principalTable: "Skills",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SkillEvaluations",
                schema: "talent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Source = table.Column<byte>(type: "tinyint", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeltaLevel = table.Column<short>(type: "smallint", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillEvaluations_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SkillEvaluations_Projects",
                        column: x => x.ProjectId,
                        principalSchema: "projects",
                        principalTable: "Projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SkillEvaluations_Skills",
                        column: x => x.SkillId,
                        principalSchema: "talent",
                        principalTable: "Skills",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SkillEvaluations_Source",
                        column: x => x.Source,
                        principalSchema: "config",
                        principalTable: "EvaluationSources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SkillEvaluations_Users",
                        column: x => x.UserId,
                        principalSchema: "iam",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectAssignments",
                schema: "projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAssignments_Organizations",
                        column: x => x.OrganizationId,
                        principalSchema: "iam",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectAssignments_ProjectRoles",
                        column: x => x.ProjectRoleId,
                        principalSchema: "projects",
                        principalTable: "ProjectRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectAssignments_Projects",
                        column: x => x.ProjectId,
                        principalSchema: "projects",
                        principalTable: "Projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectAssignments_Status",
                        column: x => x.Status,
                        principalSchema: "config",
                        principalTable: "AssignmentStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectAssignments_Users",
                        column: x => x.UserId,
                        principalSchema: "iam",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectAssignments_Users_AssignedBy",
                        column: x => x.AssignedByUserId,
                        principalSchema: "iam",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accidentes_AsociadoId",
                schema: "sst",
                table: "Accidentes",
                column: "AsociadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Accidentes_InvestigacionCompletada",
                schema: "sst",
                table: "Accidentes",
                column: "InvestigacionCompletada");

            migrationBuilder.CreateIndex(
                name: "IX_Accidentes_OrganizationId",
                schema: "sst",
                table: "Accidentes",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Actas_AsambleaId",
                table: "Actas",
                column: "AsambleaId");

            migrationBuilder.CreateIndex(
                name: "IX_Actas_OrganoId",
                table: "Actas",
                column: "OrganoId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentActions_Org_Status_Date",
                schema: "reporting",
                table: "AgentActions",
                columns: new[] { "OrganizationId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AgentActions_Org_Type",
                schema: "reporting",
                table: "AgentActions",
                columns: new[] { "OrganizationId", "ActionType" });

            migrationBuilder.CreateIndex(
                name: "UQ_AgentActionStatuses_Code",
                schema: "config",
                table: "AgentActionStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_AgentActionTypes_Code",
                schema: "config",
                table: "AgentActionTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_AgentConfig_Org_Key",
                schema: "reporting",
                table: "AgentConfiguration",
                columns: new[] { "OrganizationId", "ConfigKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_ApplicationStatuses_Code",
                schema: "config",
                table: "ApplicationStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asambleas_OrganoId",
                table: "Asambleas",
                column: "OrganoId");

            migrationBuilder.CreateIndex(
                name: "IX_AsociadosEducacion_ProgramaEducacionId",
                schema: "gestion_humana",
                table: "AsociadosEducacion",
                column: "ProgramaEducacionId");

            migrationBuilder.CreateIndex(
                name: "UQ_AssignmentStatuses_Code",
                schema: "config",
                table: "AssignmentStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AutorizacionesHabeasData_AsociadoId",
                schema: "habeasdata",
                table: "AutorizacionesHabeasData",
                column: "AsociadoId");

            migrationBuilder.CreateIndex(
                name: "IX_AutorizacionesHabeasData_Revocada",
                schema: "habeasdata",
                table: "AutorizacionesHabeasData",
                column: "Revocada");

            migrationBuilder.CreateIndex(
                name: "IX_Auxilios_FondoSolidaridadId",
                schema: "bienestar",
                table: "Auxilios",
                column: "FondoSolidaridadId");

            migrationBuilder.CreateIndex(
                name: "IX_Auxilios_SolicitudBienestarId",
                schema: "bienestar",
                table: "Auxilios",
                column: "SolicitudBienestarId");

            migrationBuilder.CreateIndex(
                name: "IX_Certifications_Org_User",
                schema: "talent",
                table: "Certifications",
                columns: new[] { "OrganizationId", "UserId" },
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_Certifications_UserId",
                schema: "talent",
                table: "Certifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Compensaciones_Asociado_Periodo",
                schema: "nomina",
                table: "Compensaciones",
                columns: new[] { "AsociadoId", "Periodo" });

            migrationBuilder.CreateIndex(
                name: "UQ_ContributionScores_Code",
                schema: "config",
                table: "ContributionScores",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfiles_Org",
                schema: "talent",
                table: "EmployeeProfiles",
                column: "OrganizationId",
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkills_Level",
                schema: "talent",
                table: "EmployeeSkills",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkills_Org_Skill",
                schema: "talent",
                table: "EmployeeSkills",
                columns: new[] { "OrganizationId", "SkillId" },
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkills_SkillId",
                schema: "talent",
                table: "EmployeeSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkills_UserId",
                schema: "talent",
                table: "EmployeeSkills",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkills_ValidatedByUserId",
                schema: "talent",
                table: "EmployeeSkills",
                column: "ValidatedByUserId");

            migrationBuilder.CreateIndex(
                name: "UX_EmployeeSkills_Org_User_Skill",
                schema: "talent",
                table: "EmployeeSkills",
                columns: new[] { "OrganizationId", "UserId", "SkillId" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "UQ_EvaluationSources_Code",
                schema: "config",
                table: "EvaluationSources",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamenesMedicos_AsociadoId",
                schema: "sst",
                table: "ExamenesMedicos",
                column: "AsociadoId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamenesMedicos_OrganizationId",
                schema: "sst",
                table: "ExamenesMedicos",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamenesMedicos_OrganizationId_FechaProgramado",
                schema: "sst",
                table: "ExamenesMedicos",
                columns: new[] { "OrganizationId", "FechaProgramado" });

            migrationBuilder.CreateIndex(
                name: "IX_Excedentes_OrganizationId",
                schema: "excedentes",
                table: "Excedentes",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Excedentes_OrganizationId_Periodo",
                schema: "excedentes",
                table: "Excedentes",
                columns: new[] { "OrganizationId", "Periodo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_FondoSolidaridad_Org_Periodo",
                schema: "bienestar",
                table: "FondoSolidaridad",
                columns: new[] { "OrganizationId", "Periodo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_IndicadorBalance_Asociado_Anio",
                schema: "balance_social",
                table: "IndicadoresBalanceSocial",
                columns: new[] { "AsociadoId", "Anio" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MiembrosOrgano_OrganoId",
                table: "MiembrosOrgano",
                column: "OrganoId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_ParentId",
                schema: "iam",
                table: "Organizations",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "UQ_Organizations_Nit",
                schema: "iam",
                table: "Organizations",
                column: "Nit",
                unique: true,
                filter: "[Nit] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Permissions_Code",
                schema: "iam",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonOrganizations_OrganizationId",
                schema: "iam",
                table: "PersonOrganizations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PilaAportes_Asociado_Periodo",
                schema: "nomina",
                table: "PilaAportes",
                columns: new[] { "AsociadoId", "Periodo" });

            migrationBuilder.CreateIndex(
                name: "IX_PilaAportes_Org_Periodo",
                schema: "nomina",
                table: "PilaAportes",
                columns: new[] { "OrganizationId", "Periodo" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectApplications_Org_Project_Status",
                schema: "projects",
                table: "ProjectApplications",
                columns: new[] { "OrganizationId", "ProjectId", "Status" },
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectApplications_ProjectId",
                schema: "projects",
                table: "ProjectApplications",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectApplications_ReviewedByUserId",
                schema: "projects",
                table: "ProjectApplications",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectApplications_Status",
                schema: "projects",
                table: "ProjectApplications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectApplications_UserId",
                schema: "projects",
                table: "ProjectApplications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UX_ProjectApplications_Org_Project_User",
                schema: "projects",
                table: "ProjectApplications",
                columns: new[] { "OrganizationId", "ProjectId", "UserId" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAssignments_AssignedByUserId",
                schema: "projects",
                table: "ProjectAssignments",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAssignments_Org_Project_Status",
                schema: "projects",
                table: "ProjectAssignments",
                columns: new[] { "OrganizationId", "ProjectId", "Status" },
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAssignments_ProjectId",
                schema: "projects",
                table: "ProjectAssignments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAssignments_ProjectRoleId",
                schema: "projects",
                table: "ProjectAssignments",
                column: "ProjectRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAssignments_Status",
                schema: "projects",
                table: "ProjectAssignments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAssignments_UserId",
                schema: "projects",
                table: "ProjectAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UX_ProjectAssignments_Org_Project_User_Active",
                schema: "projects",
                table: "ProjectAssignments",
                columns: new[] { "OrganizationId", "ProjectId", "UserId" },
                unique: true,
                filter: "([IsDeleted]=(0) AND [Status]<>(3))");

            migrationBuilder.CreateIndex(
                name: "UQ_ProjectComplexityLevels_Code",
                schema: "config",
                table: "ProjectComplexityLevels",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectParticipation_ContributionScore",
                schema: "projects",
                table: "ProjectParticipation",
                column: "ContributionScore");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectParticipation_ProjectId",
                schema: "projects",
                table: "ProjectParticipation",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectParticipation_UserId",
                schema: "projects",
                table: "ProjectParticipation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UX_ProjectParticipation_Org_Project_User",
                schema: "projects",
                table: "ProjectParticipation",
                columns: new[] { "OrganizationId", "ProjectId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRoles_ProjectId",
                schema: "projects",
                table: "ProjectRoles",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "UX_ProjectRoles_Org_Project_Name",
                schema: "projects",
                table: "ProjectRoles",
                columns: new[] { "OrganizationId", "ProjectId", "Name" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ComplexityLevel",
                schema: "projects",
                table: "Projects",
                column: "ComplexityLevel");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Org_Status",
                schema: "projects",
                table: "Projects",
                columns: new[] { "OrganizationId", "Status" },
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Status",
                schema: "projects",
                table: "Projects",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "UX_Projects_Org_Code",
                schema: "projects",
                table: "Projects",
                columns: new[] { "OrganizationId", "Code" },
                unique: true,
                filter: "([Code] IS NOT NULL AND [IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkillRequirements_Org_Skill",
                schema: "projects",
                table: "ProjectSkillRequirements",
                columns: new[] { "OrganizationId", "SkillId" },
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkillRequirements_ProjectId",
                schema: "projects",
                table: "ProjectSkillRequirements",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkillRequirements_RequiredLevel",
                schema: "projects",
                table: "ProjectSkillRequirements",
                column: "RequiredLevel");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkillRequirements_SkillId",
                schema: "projects",
                table: "ProjectSkillRequirements",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "UX_ProjectSkillRequirements_Org_Project_Skill",
                schema: "projects",
                table: "ProjectSkillRequirements",
                columns: new[] { "OrganizationId", "ProjectId", "SkillId" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "UQ_ProjectStatuses_Code",
                schema: "config",
                table: "ProjectStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationLogs_GeneratedByUserId",
                schema: "reporting",
                table: "RecommendationLogs",
                column: "GeneratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationLogs_Org_Date",
                schema: "reporting",
                table: "RecommendationLogs",
                columns: new[] { "OrganizationId", "GeneratedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_RecommendationRules_Org_IsActive",
                schema: "reporting",
                table: "RecommendationRules",
                columns: new[] { "OrganizationId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportesSupersolidaria_OrganizationId",
                schema: "reportes",
                table: "ReportesSupersolidaria",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportesSupersolidaria_OrganizationId_Periodo",
                schema: "reportes",
                table: "ReportesSupersolidaria",
                columns: new[] { "OrganizationId", "Periodo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_ReportSnapshots_Org_Date",
                schema: "reporting",
                table: "ReportSnapshots",
                columns: new[] { "OrganizationId", "SnapshotDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Riesgos_Activo",
                schema: "sst",
                table: "Riesgos",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Riesgos_OrganizationId",
                schema: "sst",
                table: "Riesgos",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                schema: "iam",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "UX_Roles_Org_Name",
                schema: "iam",
                table: "Roles",
                columns: new[] { "OrganizationId", "Name" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "UQ_SeniorityLevels_Code",
                schema: "config",
                table: "SeniorityLevels",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillCategories_ParentCategoryId",
                schema: "config",
                table: "SkillCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "UQ_SkillCategories_Code",
                schema: "config",
                table: "SkillCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillEvaluations_Org_User_Skill_Date",
                schema: "talent",
                table: "SkillEvaluations",
                columns: new[] { "OrganizationId", "UserId", "SkillId", "CreatedAt" },
                descending: new[] { false, false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_SkillEvaluations_ProjectId",
                schema: "talent",
                table: "SkillEvaluations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillEvaluations_SkillId",
                schema: "talent",
                table: "SkillEvaluations",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillEvaluations_Source",
                schema: "talent",
                table: "SkillEvaluations",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_SkillEvaluations_UserId",
                schema: "talent",
                table: "SkillEvaluations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_SkillLevels_Code",
                schema: "config",
                table: "SkillLevels",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Skills_Org_Name",
                schema: "talent",
                table: "Skills",
                columns: new[] { "OrganizationId", "Name" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "UQ_SkillTypes_Code",
                schema: "config",
                table: "SkillTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesARCO_AsociadoId",
                schema: "habeasdata",
                table: "SolicitudesARCO",
                column: "AsociadoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesARCO_Estado",
                schema: "habeasdata",
                table: "SolicitudesARCO",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesARCO_OrganizationId",
                schema: "habeasdata",
                table: "SolicitudesARCO",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesBienestar_ProgramaBienestarId",
                schema: "bienestar",
                table: "SolicitudesBienestar",
                column: "ProgramaBienestarId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_Org_User",
                schema: "iam",
                table: "UserPermissions",
                columns: new[] { "OrganizationId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionId",
                schema: "iam",
                table: "UserPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_Org_User",
                schema: "iam",
                table: "UserRoles",
                columns: new[] { "OrganizationId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "iam",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Org_IsActive",
                schema: "iam",
                table: "Users",
                columns: new[] { "OrganizationId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "UX_Users_Org_Email",
                schema: "iam",
                table: "Users",
                columns: new[] { "OrganizationId", "Email" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_AsambleaId",
                table: "Votos",
                column: "AsambleaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accidentes",
                schema: "sst");

            migrationBuilder.DropTable(
                name: "Actas");

            migrationBuilder.DropTable(
                name: "AgentActions",
                schema: "reporting");

            migrationBuilder.DropTable(
                name: "AgentActionStatuses",
                schema: "config");

            migrationBuilder.DropTable(
                name: "AgentActionTypes",
                schema: "config");

            migrationBuilder.DropTable(
                name: "AgentConfiguration",
                schema: "reporting");

            migrationBuilder.DropTable(
                name: "AsociadosEducacion",
                schema: "gestion_humana");

            migrationBuilder.DropTable(
                name: "AutorizacionesHabeasData",
                schema: "habeasdata");

            migrationBuilder.DropTable(
                name: "Auxilios",
                schema: "bienestar");

            migrationBuilder.DropTable(
                name: "Certifications",
                schema: "talent");

            migrationBuilder.DropTable(
                name: "Compensaciones",
                schema: "nomina");

            migrationBuilder.DropTable(
                name: "CompetenciasAsociado",
                schema: "gestion_humana");

            migrationBuilder.DropTable(
                name: "EmployeeProfiles",
                schema: "talent");

            migrationBuilder.DropTable(
                name: "EmployeeSkills",
                schema: "talent");

            migrationBuilder.DropTable(
                name: "ExamenesMedicos",
                schema: "sst");

            migrationBuilder.DropTable(
                name: "Excedentes",
                schema: "excedentes");

            migrationBuilder.DropTable(
                name: "HerramientasCooperativas");

            migrationBuilder.DropTable(
                name: "IndicadoresBalanceSocial",
                schema: "balance_social");

            migrationBuilder.DropTable(
                name: "MiembrosOrgano");

            migrationBuilder.DropTable(
                name: "PersonOrganizations",
                schema: "iam");

            migrationBuilder.DropTable(
                name: "PilaAportes",
                schema: "nomina");

            migrationBuilder.DropTable(
                name: "ProjectApplications",
                schema: "projects");

            migrationBuilder.DropTable(
                name: "ProjectAssignments",
                schema: "projects");

            migrationBuilder.DropTable(
                name: "ProjectParticipation",
                schema: "projects");

            migrationBuilder.DropTable(
                name: "ProjectSkillRequirements",
                schema: "projects");

            migrationBuilder.DropTable(
                name: "RecommendationLogs",
                schema: "reporting");

            migrationBuilder.DropTable(
                name: "RecommendationRules",
                schema: "reporting");

            migrationBuilder.DropTable(
                name: "ReportesSupersolidaria",
                schema: "reportes");

            migrationBuilder.DropTable(
                name: "ReportSnapshots",
                schema: "reporting");

            migrationBuilder.DropTable(
                name: "Riesgos",
                schema: "sst");

            migrationBuilder.DropTable(
                name: "RolePermissions",
                schema: "iam");

            migrationBuilder.DropTable(
                name: "SeniorityLevels",
                schema: "config");

            migrationBuilder.DropTable(
                name: "SkillCategories",
                schema: "config");

            migrationBuilder.DropTable(
                name: "SkillEvaluations",
                schema: "talent");

            migrationBuilder.DropTable(
                name: "SkillTypes",
                schema: "config");

            migrationBuilder.DropTable(
                name: "SolicitudesARCO",
                schema: "habeasdata");

            migrationBuilder.DropTable(
                name: "UserPermissions",
                schema: "iam");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "iam");

            migrationBuilder.DropTable(
                name: "Votos");

            migrationBuilder.DropTable(
                name: "ProgramasEducacion",
                schema: "gestion_humana");

            migrationBuilder.DropTable(
                name: "FondoSolidaridad",
                schema: "bienestar");

            migrationBuilder.DropTable(
                name: "SolicitudesBienestar",
                schema: "bienestar");

            migrationBuilder.DropTable(
                name: "ApplicationStatuses",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ProjectRoles",
                schema: "projects");

            migrationBuilder.DropTable(
                name: "AssignmentStatuses",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ContributionScores",
                schema: "config");

            migrationBuilder.DropTable(
                name: "SkillLevels",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Skills",
                schema: "talent");

            migrationBuilder.DropTable(
                name: "EvaluationSources",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "iam");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "iam");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "iam");

            migrationBuilder.DropTable(
                name: "Asambleas");

            migrationBuilder.DropTable(
                name: "ProgramasBienestar",
                schema: "bienestar");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "projects");

            migrationBuilder.DropTable(
                name: "Organos");

            migrationBuilder.DropTable(
                name: "ProjectComplexityLevels",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Organizations",
                schema: "iam");

            migrationBuilder.DropTable(
                name: "ProjectStatuses",
                schema: "config");
        }
    }
}
