# Exploration: gestion-humana-solidaria

## Executive Summary

Transform DevManagerAPI — a Clean Architecture .NET 8 talent-and-projects platform — into a **comprehensive people-management platform for the Colombian solidarity sector** (cooperatives, mutuals, employee funds, pre-cooperatives, associations). The seed already provides IAM, talent profiles, project workflows, and an AI agent — all with multi-tenancy, audit trails, and RBAC. The transformation requires evolving 4 bounded contexts and adding 7 new ones to cover asociados lifecycle, aportes sociales, excedentes, governance bodies, balance social, cooperative education, and Habeas Data compliance. The opportunity is significant: existing software (Siarsoft, Ascoop, bespoke solutions) covers financial operations but leaves HR, balance social, and regulatory reporting fragmented or manual.

## Current State (seed)

### Reusable Building Blocks

| Block | Path | Carries Forward As |
|-------|------|-------------------|
| `Organization` (NIT, legal name, multi-tenant root) | `Domain/Entities/IAM/Organization.cs` | Raw material for `Cooperativa`/`Fondo`/`Mutual` entity |
| `User` (name, email, password hash, IsActive, OrganizationId) | `Domain/Entities/IAM/User.cs` | Base identity for `Asociado` (extend with documento, fechaIngreso, estado) |
| `Role` + `Permission` + `UserRole` + `RolePermission` + `UserPermission` | `Domain/Entities/IAM/` | RBAC carries directly; add solidarity-specific roles (Consejo, Junta, Comité) |
| `AuditableEntity` (soft delete, audit timestamps) | `Domain/Common/AuditableEntity.cs` | Reusable across all new entities |
| `EmployeeProfile` (Bio, YearsExperience, LinkedIn) | `Domain/Entities/Talent/EmployeeProfile.cs` | Evolve into `PerfilAsociado` (add competencias, formación, balance social indicators) |
| `Skill` + `EmployeeSkill` + `SkillEvaluation` (proficiency 1-5, validatedBy, LastValidatedAt) | `Domain/Entities/Talent/` | Reusable for cooperative education tracking, skills mapping |
| `Certification` (Name, Issuer, IssueDate, ExpirationDate, EvidenceUrl) | `Domain/Entities/Talent/Certification.cs` | Directly reusable for cooperative education programs |
| `Project` + `ProjectRole` + `ProjectSkillRequirement` + `ProjectApplication` + `ProjectAssignment` | `Domain/Entities/Projects/` | Reusable for solidarity services, comités, asambleas |
| `AgentAction` + `AgentConfiguration` + `AgentTool` | `Domain/Entities/Agent/` | Agent orchestrator evolves into Asistente Cooperativo IA |
| `ApiResponse<T>` envelope | `API/` | Standard across all endpoints |
| JWT with OrganizationId claim | `API/Extensions/ApplicationServiceExtensions.cs` | Multi-tenancy carries forward |
| `DevManagerDbContext` with `ApplyConfigurationsFromAssembly` | `Infrastructure/Data/DevManagerDbContext.cs` | Clean EF pattern, extensible |
| Dual entity pattern (Domain POCO ↔ EF entity with mappers) | `Infrastructure/Data/Entities/` | Continues; consider AutoMapper |
| 12 config/catalog tables (ProjectStatus, SkillLevel, etc.) | `Infrastructure/Data/Entities/Config/` | Add solidarity catalogs (TipoAporte, TipoOrgano, DimensiónBalanceSocial) |
| Background services (ReportSnapshotGenerator, RecommendationOptimizer) | `Infrastructure/BackgroundServices/` | Reusable for balance social snapshots, compliance reports |
| `ReportsService` (skills distribution, project metrics, AI summary) | `Application/Services/ReportsService.cs` | Pattern for Balance Social reports |
| `AgentService` (Gemini orchestration, tool use, HITL) | `Application/Services/AgentService.cs` | Core AI engine — evolves with cooperative domain context |

### Components That Need Evolution

| Component | What Needs to Change |
|-----------|---------------------|
| `User` entity | Needs asociado-specific fields: TipoDocumento, NumeroDocumento, FechaNacimiento, FechaIngreso, FechaRetiro, EstadoAsociado, AportesMinimos. The `User`+`EmployeeProfile` split maps well to the asociado/empleado dual model. |
| `Organization` | Evolve into a richer entity with legal type (cooperativa, mutual, fondo), Supersolidaria registration data, RUC, legal representative. |
| `Role`/`Permission` | Add solidarity governance roles: `consejo-administracion`, `junta-vigilancia`, `comite-educacion`, `revisor-fiscal`. |
| `Project` entities | Generalize: a "project" becomes a "servicio solidario", "comité", "asamblea", "programa de formación". Add quorum, voting results, act tracking. |
| `AgentService` | Retrain/re-prompt Gemini with cooperative regulations, Supersolidaria norms, internal statutes. Add domain-specific tools for consulting normatividad, generating balance social reports. |
| `ReportsService` | Add balance social dimensions: governance, member satisfaction, community impact, education coverage, ethical/environmental indicators. |

### Missing Pieces (new bounded contexts needed)

| Missing Domain | Why It's Needed |
|---------------|-----------------|
| `Asociados` | Full lifecycle: admission, approval, status changes, suspensions, reinstatements, retirements. Not just a `User` record. |
| `Aportes` | Social contributions regime: ordinary/extraordinary contributions, amortization, reimbursement, minimum non-reducible contributions (Ley 79 art. 5, 46-52). |
| `Excedentes` | Surplus generation and distribution: 20% minimum to reserve, 20% to education fund, 10% to solidarity fund, remainder for revalorization/return (Ley 79 art. 54). |
| `Organos` | Governance bodies: Asamblea General, Consejo de Administración, Junta de Vigilancia, Revisoría Fiscal, Comités. Actas, quorum, voting, mandates (Ley 79 Tit. I Cap. IV). |
| `BalanceSocial` | Multi-dimensional reporting: democratic governance, needs satisfaction, community commitment, ethics, environmental responsibility. Key for Supersolidaria. |
| `Educacion` | Cooperative education: mandatory programs (Ley 79 art. 88-91), 20-hour minimum for founders, ongoing formation tracking, evaluation, coverage indicators. |
| `HabeasData` | Data protection compliance: Ley 1581/2012 authorizations, treatment registers, ARCO rights (Access, Rectification, Cancellation, Opposition). |

## Colombian Solidarity Sector — Regulation

### Ley 79 de 1988 (December 23) — Cooperative Law
- **Source**: [Gestor Normativo Función Pública](https://www.funcionpublica.gov.co/eva/gestornormativo/norma.php?i=9211)
- **Título Preliminar**: Objectives — facilitate cooperative doctrine/principles, strengthen solidarity and social economy (art. 1-2).
- **Título I — Del Acuerdo Cooperativo**: Defines the cooperative contract (art. 3-12), constitution and recognition (art. 13-20), associates (art. 21-25), administration and oversight (art. 26-45), economic regime (art. 46-56), labor regime (art. 57-60).
- **Título I, Capítulo III — Asociados**: Who can be associates (art. 21), how quality is acquired (art. 22), fundamental rights (art. 23: use services, participate, vote, inform, oversee, withdraw), duties (art. 24: learn cooperative principles, comply, behave solidarity), loss of quality (art. 25: death, dissolution, voluntary withdrawal, exclusion).
- **Título I, Capítulo IV — Administration**: Asamblea General as supreme body (art. 27-34: quorum 50% first call, 10% second; one member one vote; functions), Consejo de Administración (art. 35-36), Gerente (art. 37), Junta de Vigilancia (art. 38-40: up to 3 members, oversight functions), Revisor Fiscal (art. 41-43: CPA required).
- **Título I, Capítulo V — Régimen Económico**: Patrimony = individual + amortized contributions + reserves + donations (art. 46). Ordinary/extraordinary contributions in money, kind, or labor (art. 47). Contributions inalienable, non-embargable (art. 49). 10% max per individual (art. 50). Excedentes distribution: 20% min to reserve, 20% min to education fund, 10% min to solidarity fund (art. 54).
- **Título I, Capítulo IX — Educación Cooperativa**: Art. 88-91 — cooperatives MUST perform permanent education on cooperative principles, methods, and characteristics. 20-hour minimum education for founders (art. 15.5). At least 20% of excedentes to education fund (art. 54).

### Ley 454 de 1998 (August 4) — Solidarity Economy Framework
- **Source**: [Gestor Normativo Función Pública](https://www.funcionpublica.gov.co/eva/gestornormativo/norma.php?i=3433)
- **Título I — Marco Conceptual**: Defines Solidarity Economy (art. 2), principles (art. 4: human primacy, solidarity, democratic administration, voluntary adhesion, cooperative property, economic participation, education, autonomy, service, integration, ecology), subjects (art. 6: cooperatives, second/third-degree orgs, pre-cooperatives, employee funds, mutuals, community enterprises).
- **Título II — Organismos de Apoyo**: Consejo Nacional de Economía Solidaria - CONES (art. 20-22), Fondo de Fomento FONES (art. 23-28).
- **Título III — Estatales**: Creates DANSOCIAL (art. 29-32), creates Superintendencia de la Economía Solidaria (art. 33-38) — inspection, surveillance, control.
- **Art. 34 (mod. Ley 795/2003)**: Supersolidaria supervises cooperatives + solidarity orgs not under specialized supervision. Cooperativas de ahorro y crédito under Delegatura especializada.
- **Art. 36**: Supersolidaria powers include requiring periodic socioeconomic reports (num. 2), setting accounting rules (num. 3), imposing sanctions (num. 6-7), ordering dissolution (num. 9).

### Additional Regulatory Framework
- **Decreto 1332 de 1989**: Administrative aspects of cooperative law implementation.
- **Decreto 2150 de 2017**: Updates to inspection/surveillance regime.
- **Circular Básica Jurídica 2020 (Supersolidaria)**: ⚠️ **Could not be fetched directly** — the Supersolidaria site was unreachable. Known structure: Título I (naturaleza, principios, valores), Título II (constitución, registro, reforma), Título III (régimen económico, aportes, excedentes), Capítulo X (educación). This is the regulatory "bible" and MUST be consulted during spec/design phases.
- **Ley 1581 de 2012 + Decreto 1377 de 2013**: Habeas Data — data protection for associates. Requires: authorization for processing, clear purpose, data subject rights (ARCO). Enforced by Superintendencia de Industria y Comercio (SIC).

### Tipología de Entidades Solidarias

| Type | Key Characteristics | Software Implications |
|------|---------------------|---------------------|
| **Cooperativa de Trabajo Asociado (CTA)** | Associates are workers + owners. No labor contract (art. 59 Ley 79). Compensation via work aporte + excedentes return. | No "nómina laboral" — compensation = compensación + retorno. Admit/withdrawal lifecycle critical. |
| **Cooperativa de Ahorro y Crédito** | Financial activity with associates. Under Supersolidaria Delegatura. Savings, loans, credit. | Needs financial operations module (OUT OF SCOPE for HR). Our HR module feeds asociado data to their financial core. |
| **Cooperativa Multiactiva/Integral** | Multiple activity lines. Must organize in independent sections (art. 63-64). | Multi-section tracking. Each section may have different asociado data needs. |
| **Precooperativa** | Simplified rules (art. 14 Ley 79 mod. Ley 2069/2020). Min 3 founders. | Lighter governance. No need for full Órganos module initially. |
| **Mutual (Asociación Mutual)** | Ley 454 art. 6. Mutual aid, not cooperative but solidarity. | Similar lifecycle but distinct legal framework. "Asociado" not "cooperado". |
| **Fondo de Empleados** | Ley 454 art. 6. Employee association within a company. Savings and credit. | Associates are employees of a sponsor company. May have deduction-at-source. |
| **Asociación Solidaria** | Broad solidarity category. Flexible governance. | Most varied. May need very customizable entity types. |

## Sector Concepts in HR Terms

| Solidarity Concept | Software Equivalent / Feature |
|-------------------|------------------------------|
| **Asociado** | Person record with: identity docs, admission date, status (active/suspended/retired), aporte balance, rights (vote, service use), duties. Extends from `User`. |
| **Admisión de Asociados** | Application → approval workflow (Consejo de Administración or Asamblea). Document collection, background check, minimum education requirement (20 hrs). |
| **Aportes Sociales** | Contribution account: ordinary (periodic, mandatory), extraordinary (by Asamblea approval), voluntary. Balance, amortization, reimbursement on withdrawal. |
| **Excedentes** (surplus) | Not "profit". Distribution algorithm: 20% reserve → 20% education → 10% solidarity → remainder per use/work. Software must compute and track per associate. |
| **Retorno Cooperativo** | Per-associate return based on service use (consumption, work aporte). Not per capital. |
| **Asamblea General** | Event entity with: type (ordinaria/extraordinaria), convocatoria (notice period), quorum tracking (50% first call, 10% second), voting (1 member = 1 vote), acta. |
| **Consejo de Administración** | Rotating board. Election by Asamblea. Term tracking. Meeting minutes. Resolution enforcement. |
| **Junta de Vigilancia** | Social control body (up to 3 members). Oversight of Consejo. Receives complaints. Verifies list of hábiles/inhábiles. |
| **Balance Social** | Periodic report with indicators: governance democracy, member satisfaction (encuestas), education investment, community projects, ethical compliance, environmental footprint. |
| **Educación Cooperativa** | Training programs (mandatory), tracking per associate, hours count, evaluations, coverage ratio (trained/total associates). 20% of excedentes funds this. |
| **Habeas Data** | Authorization record for each associate (signed at admission). Treatment register. ARCO request management. SIC compliance. |
| **Asociado vs. Empleado** | Dual model: an asociado CAN be an employee (different legal relationship). The software must keep both dimensions separate: contrato de asociación ≠ contrato laboral. |

## Bounded Context Map

### Existing (evolution)

#### 1. IAM → IAM Solidario

| Aspect | Current | Evolved |
|--------|---------|---------|
| Responsibility | Organization identity, user auth, RBAC | Cooperative identity, asociado auth, solidarity RBAC + governance roles |
| Key Entities | Organization, User, Role, Permission, UserRole, UserPermission, RolePermission | + `Cooperativa` (extends Organization with legal type, NIT, Supersolidaria reg), + `Asociado` (evolves User with identity docs, admission workflow, dual status), + governance Roles |
| Key Services | AuthService, UserService, TokenService | + AsociadoService (admission/retirement/suspension), + GovernanceRoleService |
| Key DTOs | LoginRequest, UserResponse, RoleSummaryResponse | + AsociadoResponse, AsociadoCreateRequest, AsociadoStatusChangeRequest |
| Key Endpoints | POST /api/auth/login, GET/POST /api/users, GET/POST /api/roles | + POST /api/asociados/admitir, GET /api/asociados/{id}/estado, PUT /api/asociados/{id}/retirar |

#### 2. Talent → Gestión Humana Solidaria

| Aspect | Current | Evolved |
|--------|---------|---------|
| Responsibility | Employee skills, profiles, certifications, evaluations | Asociado competencies, education tracking, performance for work-association CTAs |
| Key Entities | EmployeeProfile, Skill, EmployeeSkill, Certification, SkillEvaluation | + `PerfilAsociado` (extends EmployeeProfile with solidarity-specific fields), + `Formacion` (education program enrollment), + `EvaluacionDesempeño` (adapted for cooperative context) |
| Key Services | ProfileService, SkillService, EmployeeSkillService, CertificationService | + FormacionService, + DesempeñoService |
| Key DTOs | ProfileResponse, SkillDto, CertificationDto | + FormacionResponse, + EvaluacionDesempeñoDto |
| Key Endpoints | GET /api/profile/me, GET /api/skills, GET /api/employees/skills | + POST /api/formacion/inscribir, GET /api/formacion/historial/{asociadoId} |

#### 3. Projects → Servicios Solidarios

| Aspect | Current | Evolved |
|--------|---------|---------|
| Responsibility | Project lifecycle, skill matching, assignments | Solidarity services, committees, assemblies, associate assignments to committees |
| Key Entities | Project, ProjectRole, ProjectSkillRequirement, ProjectApplication, ProjectAssignment | + `ServicioSolidario` (extends Project), + `Comite`, + `ActaAsamblea`, + `Votacion` (quorum, results) |
| Key Services | ProjectService, ApplicationService, AssignmentService | + ServicioSolidarioService, + ComiteService, + AsambleaService |
| Key DTOs | ProjectResponse, ProjectApplicationDto, AssignmentDto | + ServicioResponse, + ActaDto, + VotacionResultDto |
| Key Endpoints | GET /api/projects, POST /api/applications, POST /api/assignments | + POST /api/asambleas/convocar, + POST /api/asambleas/{id}/votar, + GET /api/comites |

#### 4. Agent → Asistente Cooperativo IA

| Aspect | Current | Evolved |
|--------|---------|---------|
| Responsibility | Talent queries, skill validation, candidate matching, HITL | Cooperative assistant: normatividad queries, balance social generation, regulatory compliance assistance, act drafting, education recommendations |
| Key Entities | AgentAction, AgentConfiguration, AgentTool | + Cooperative domain tools (consultarNorma, generarBalanceSocial, verificarCumplimiento) |
| Key Services | AgentService, GeminiService | + CooperativeSystemPrompt (includes Ley 79, Ley 454, Circular Básica), + NormaRepository (regulatory text index) |
| Key DTOs | AgentQueryRequest, AgentQueryResponse, SkillValidationResponse, SkillMatchResponse | + BalanceSocialReportRequest, + ComplianceCheckRequest |
| Key Endpoints | POST /agent/query, POST /agent/validate-skill, POST /agent/match-candidates | + POST /agent/consultar-norma, POST /agent/reporte-balance-social, POST /agent/verificar-cumplimiento |

### Proposed NEW Bounded Contexts

#### 5. Asociados

- **Responsibility**: Full lifecycle management of cooperative associates: admission, status, rights/duties, suspension, reinstatement, withdrawal.
- **Key Aggregates/Entities**:
  - `Asociado` (extends User: DocumentType, DocumentNumber, BirthDate, AdmissionDate, Status [active/suspended/retired/excluded], MinAporteRequirement, AdmissionApprovedBy, AdmissionApprovedAt)
  - `AdmissionRequest` (workflow entity: requested at, documents, reviewed by consejo, approved/rejected, approval act reference)
  - `Sancion` (disciplinary actions: type, cause, duration, imposed by, appeal)
  - `Retiro` (withdrawal record: date, type [voluntary/exclusion/death], aporte reimbursement status, act reference)
- **Key Services**: `AsociadoService` (admit, update status, suspend, reinstate, retire, list), `AdmissionWorkflowService`
- **Key DTOs**: `AsociadoResponse`, `AsociadoCreateRequest`, `AsociadoStatusChangeRequest`, `AdmissionRequestDto`
- **Key Endpoints**: `POST /api/asociados/admitir`, `GET /api/asociados`, `GET /api/asociados/{id}`, `PUT /api/asociados/{id}/estado`, `POST /api/asociados/{id}/retirar`

#### 6. Aportes

- **Responsibility**: Social contributions regime — accounting for each associate's contributions, minimum capital requirements, amortization, reimbursement on withdrawal.
- **Key Aggregates/Entities**:
  - `AporteSocial` (associate contribution: type [ordinary/extraordinary/voluntary], amount, payment date, payment method, reference, capitalized flag)
  - `AportePeriodo` (periodic contribution plan: monthly/quarterly amount, due date, grace period, late fee)
  - `Amortizacion` (contribution amortization: authorized by Asamblea, from excedentes reserve fund, proportional to all associates)
  - `DevolucionAportes` (reimbursement on withdrawal: total aportes, deductions, payment schedule)
- **Key Services**: `AporteSocialService`, `AporteCalculationService`, `AmortizacionService`
- **Key DTOs**: `AporteResponse`, `AporteCreateRequest`, `AporteResumenDto` (total aportes per associate), `AmortizacionDto`
- **Key Endpoints**: `POST /api/aportes/registrar`, `GET /api/aportes/saldo/{asociadoId}`, `POST /api/aportes/amortizar`, `GET /api/aportes/reporte/{cooperativaId}`

#### 7. Excedentes

- **Responsibility**: Annual surplus calculation, statutory distribution (20% reserve + 20% education + 10% solidarity + remainder per Asamblea decision), per-associate return calculation.
- **Key Aggregates/Entities**:
  - `ExcedenteEjercicio` (annual surplus: fiscal year, total surplus, distributions: reserveAmount, educationFundAmount, solidarityFundAmount, revalorizationAmount, returnAmount)
  - `RetornoCooperativo` (per-associate return: percentage or amount based on service use / work contribution)
  - `DistribucionExcedente` (distribution plan: approved by Asamblea General, line items per destination)
- **Key Services**: `ExcedenteService` (calculate, distribute, approve distribution, record per-associate return), `RetornoCalculator`
- **Key DTOs**: `ExcedenteResponse`, `DistribucionRequest`, `RetornoDto`, `ExcedenteResumenDto`
- **Key Endpoints**: `POST /api/excedentes/calcular`, `POST /api/excedentes/aprobar-distribucion`, `GET /api/excedentes/historial`, `GET /api/excedentes/retorno/{asociadoId}`

#### 8. Organos

- **Responsibility**: Governance bodies management: assemblies, board, oversight committee, fiscal auditor, committees. Meeting convocation, quorum, voting, minutes.
- **Key Aggregates/Entities**:
  - `Organo` (type: AsambleaGeneral, ConsejoAdministracion, JuntaVigilancia, ComiteEducacion, ComiteSolidaridad, etc.; members, mandates, period)
  - `Sesion` (meeting: date, type [ordinaria/extraordinaria], convocatoria, quorum, agenda, acta)
  - `MiembroOrgano` (member: asociado, organo, start date, end date, position, elected by)
  - `Votacion` (voting: session, motion/matter, results: yes/no/abstain counts, approved flag)
  - `Acta` (meeting minutes: content, approved by, approved date)
- **Key Services**: `OrganoService`, `SesionService`, `VotacionService`, `ActaService`
- **Key DTOs**: `OrganoResponse`, `SesionDto`, `VotacionRequest`, `VotacionResultDto`, `ActaResponse`
- **Key Endpoints**: `POST /api/organos/asamblea/convocar`, `POST /api/organos/sesion/{id}/votar`, `POST /api/organos/sesion/{id}/acta`, `GET /api/organos/{id}/miembros`

#### 9. BalanceSocial

- **Responsibility**: Multi-dimensional social balance reporting. Indicators across governance, member satisfaction, community impact, education, ethics, environment.
- **Key Aggregates/Entities**:
  - `DimensionBalanceSocial` (configurable dimensions: GobernanzaDemocrática, SatisfacciónNecesidades, CompromisoComunitario, Educación, Ética, ResponsabilidadAmbiental)
  - `IndicadorBalanceSocial` (per-dimension metric: name, formula, target value, actual value, period, auto-calculated fields)
  - `ReporteBalanceSocial` (periodic report: period, all dimension scores, narrative, comparisons, generated by agent or manually)
  - `EncuestaSatisfaccion` (member satisfaction survey: questions, responses, aggregate scores)
- **Key Services**: `BalanceSocialService` (calculate indicators, generate report, trend analysis), `EncuestaService`
- **Key DTOs**: `DimensionDto`, `IndicadorDto`, `BalanceSocialReportResponse`, `EncuestaResponse`
- **Key Endpoints**: `POST /api/balance-social/calcular`, `GET /api/balance-social/reporte/{periodo}`, `POST /api/balance-social/encuesta`, `GET /api/balance-social/tendencias`

#### 10. Educacion

- **Responsibility**: Cooperative education programs — mandatory per Ley 79 art. 88-91. Program management, enrollment, attendance, evaluation, coverage indicators.
- **Key Aggregates/Entities**:
  - `ProgramaEducacion` (education program: name, type [basic/advanced/specialized], hours, content, modality, start/end dates)
  - `InscripcionEducacion` (associate enrollment in a program: date, attendance record, completion status, evaluation score)
  - `EvaluacionEducacion` (program evaluation: test scores, practical assessment, satisfaction)
  - `IndicadorEducacion` (coverage: total trained/total associates, hours per associate, education fund utilization)
- **Key Services**: `EducacionService`, `InscripcionService`, `EvaluacionService`
- **Key DTOs**: `ProgramaDto`, `InscripcionDto`, `EvaluacionDto`, `ReporteEducacionDto`
- **Key Endpoints**: `POST /api/educacion/programas`, `POST /api/educacion/inscribir`, `POST /api/educacion/evaluar`, `GET /api/educacion/reporte-cobertura`

#### 11. HabeasData

- **Responsibility**: Data protection compliance per Ley 1581/2012. Authorization management, treatment records, ARCO requests, SIC response.
- **Key Aggregates/Entities**:
  - `AutorizacionDatos` (data processing authorization: associate ID, signed date, scope, revocation date, consent evidence URL)
  - `RegistroTratamiento` (data processing record: purpose, data categories, processing activities, retention period)
  - `SolicitudARCO` (Access, Rectification, Cancellation, Opposition request: type, date, status, resolution)
  - `PoliticaTratamiento` (privacy policy: version, effective date, text URL, associate acknowledgment records)
- **Key Services**: `HabeasDataService` (authorization, ARCO management, policy versioning, compliance reporting)
- **Key DTOs**: `AutorizacionDto`, `SolicitudArcoDto`, `PoliticaDto`, `ReporteCumplimientoDto`
- **Key Endpoints**: `POST /api/habeas-data/autorizar`, `POST /api/habeas-data/arco`, `GET /api/habeas-data/autorizaciones/{asociadoId}`, `GET /api/habeas-data/reporte-cumplimiento`

## Competitive Landscape

⚠️ **Note**: Siarsoft (siarsoft.com) and Ascoop (ascoop.coop) websites were unreachable during research. Findings below are based on prior industry knowledge and public information. All claims should be verified.

### Key Players

| Player | Coverage | Gaps |
|--------|----------|------|
| **Siarsoft** | Financial software for cooperative credit/loan management, accounting, member records. Strong on financial core. | Weak on HR/people management. No balance social module. No cooperative education tracking. No IA/assistant aspect. Traditional desktop/on-premise. |
| **Ascoop** | Gremial organization. Provides some software tools to member cooperatives. | Focused on advocacy and representation, not software products. Any software is secondary. |
| **Coopensar / Confincoop / Asovic** | Regional cooperative software providers in Santander and other regions. Similar profile to Siarsoft. | Same gaps: financial-centric, no modern HR/balance social/IA. |
| **In-house (Coomeva, Colanta, Equidad, Coosalud)** | Each builds custom internal solutions for their massive membership. Not available as products. | No commercial software exists for lower-tier cooperatives. They use Excel, paper, or outdated systems. |
| **SIIGO / Auros (HR general)** | Good payroll, HR management for traditional companies. | DO NOT understand cooperative specifics: asociado vs empleado distinction, excedentes, balance social, Supersolidaria reports. |

### Gaps This Product Fills

1. **Balance social automation**: No competitor offers structured balance social reporting with configurable dimensions and Supersolidaria-ready output.
2. **Cooperative education tracking**: Mandatory per law (20% of excedentes), yet almost no software tracks it.
3. **Asociado lifecycle management**: Admission → Status → Retirement with workflow, not just a user record.
4. **Multi-cooperative / SaaS**: Existing players are on-premise per-cooperative. Our multi-tenant architecture is a differentiator.
5. **AI Assistant**: No competitor has anything like our Gemini-powered query/regulation consultation.
6. **Modern API-first**: Siarsoft and similar use old desktop/WebForms tech. Their APIs (if any) are proprietary.
7. **Supersolidaria compliance built-in**: Reports structured for the regulatory body, reducing manual work.

### What to Study From HR General Software

- **SIIGO**: Payroll calculation, social security deductions, certificados laborales — understand what to integrate, not replace.
- **Auros**: Recruitment, selection, performance evaluation flows — patterns adaptable to cooperative context.

## Recommended Roadmap

### Phase 0 — Fundación
- **Scope**: Close critical engineering gaps before adding domain complexity. Foundation for sustainable development.
- **Key Features**: (1) xUnit test project with first tests for AuthService + ReportsService, (2) CI via GitHub Actions (build + test + security scan), (3) `.editorconfig` + Roslyn analyzers, (4) secrets extraction (user secrets / env vars / Azure Key Vault), (5) Dockerfile + docker-compose for local dev reproducibility.
- **Bounded Contexts Touched**: None (cross-cutting infrastructure).
- **Effort**: Medium.
- **Dependencies**: None.
- **PR Slicing**: 1 PR of ~400 lines (test project + CI + editorconfig). A second PR if Docker is added.

### Phase 1 — Núcleo Solidario
- **Scope**: Core solidarity entities — Asociados lifecycle + Aportes sociales + Habeas Data authorizations.
- **Key Features**: (1) `Asociado` entity + admission workflow, (2) `AporteSocial` entity + basic contribution recording, (3) `AutorizacionDatos` entity + authorization capture at admission, (4) multi-cooperativa scoping via existing OrganizationId mechanism.
- **Bounded Contexts Touched**: `Asociados` (new), `Aportes` (new), `HabeasData` (new, partial), `IAM Solidario` (evolved).
- **Effort**: Large.
- **Dependencies**: Phase 0.
- **PR Slicing**: 3 PRs — (1) Asociados entity + admission workflow, (2) Aportes recording + balance query, (3) Habeas Data authorization capture.

### Phase 2 — Gestión Humana Solidaria
- **Scope**: Evolve Talent context into full cooperative HR with education tracking and basic balance social.
- **Key Features**: (1) `PerfilAsociado` evolution from `EmployeeProfile`, (2) cooperative education programs (Programa + Inscripción + Evaluación), (3) basic balance social indicators (governance + education coverage), (4) skills mapped to cooperative competencies.
- **Bounded Contexts Touched**: `Gestión Humana Solidaria` (evolved Talent), `Educación` (new), `BalanceSocial` (new, partial).
- **Effort**: Large.
- **Dependencies**: Phase 1.
- **PR Slicing**: 3-4 PRs — education first (most immediate value), then profile evolution, then basic indicators.

### Phase 3 — Cumplimiento
- **Scope**: Full regulatory compliance — complete balance social, Habeas Data end-to-end, Supersolidaria reporting.
- **Key Features**: (1) full balance social with all dimensions (satisfaction surveys, community impact, ethics, environment), (2) ARCO request management flow, (3) Supersolidaria report templates (balance social format, annual report), (4) compliance dashboard.
- **Bounded Contexts Touched**: `BalanceSocial` (complete), `HabeasData` (complete), `Excedentes` (initial).
- **Effort**: Large.
- **Dependencies**: Phase 1-2.
- **PR Slicing**: 3-4 PRs — balance social dimensions first, then ARCO flow, then Supersolidaria reports.

### Phase 4 — Comunidad y Gobernanza
- **Scope**: Governance bodies, assemblies, voting, act management.
- **Key Features**: (1) `Organo` entity with configurable body types, (2) assembly convocation with quorum calculation, (3) voting (1 member 1 vote with delegation tracking), (4) acta generation (template + approval workflow), (5) member portal integration (frontend).
- **Bounded Contexts Touched**: `Órganos` (new), `Excedentes` (distribution approval flow).
- **Effort**: Large.
- **Dependencies**: Phase 1-3.
- **PR Slicing**: 3-5 PRs — convocatoria + quorum first, then voting, then actas, then excedentes distribution approval.

### Phase 5 — IA y Diferenciación
- **Scope**: Cooperative IA assistant, natural language reporting, smart recommendations.
- **Key Features**: (1) retrained Agent with cooperative domain system prompt (Ley 79, Ley 454, Circular Básica, statutes), (2) regulatory query tool (`consultar-norma` tool), (3) auto-generate balance social reports, (4) detect compliance gaps, (5) recommend education programs based on member profiles.
- **Bounded Contexts Touched**: `Agent` (evolved → Asistente Cooperativo IA).
- **Effort**: Medium.
- **Dependencies**: Phase 1-3.
- **PR Slicing**: 2 PRs — system prompt + normatividad tool first, then balance social generation + recommendations.

## First Executable Slice

**Name**: `f0-test-ci-editorconfig`
**Scope**: Create `tests/DevManagerAPI.Tests/` with xUnit project, write first tests for `AuthService.LoginAsync` (success, invalid credentials, inactive user) and `ReportsService.GetSkillsDistributionAsync` (empty org, org with skills), setup GitHub Actions CI workflow (restore → build → test → report), add `.editorconfig` with project-wide C# formatting rules.
**File-level estimate**:
- `tests/DevManagerAPI.Tests/DevManagerAPI.Tests.csproj` (~20 lines)
- `tests/DevManagerAPI.Tests/AuthServiceTests.cs` (~120 lines)
- `tests/DevManagerAPI.Tests/ReportsServiceTests.cs` (~100 lines)
- `.github/workflows/ci.yml` (~50 lines)
- `.editorconfig` (~80 lines)
- Minor: update `DevManager.sln` to include test project (~10 lines)
**Total**: ~380 lines ✅ fits within the 400-line PR budget.

Why this slice: zero tests is the single biggest risk (documented in `openspec/project.md`). Adding tests for the two most critical services provides immediate regression safety before any domain changes begin. CI ensures tests run on every push. `.editorconfig` prevents style drift as the team expands.

## Value Proposition

**"The first cloud-native, AI-powered people management platform built specifically for Colombian solidarity-sector organizations — automating asociado lifecycle, cooperative education, balance social, and Supersolidaria compliance that no existing software handles."** Unlike Siarsoft (financial-only, on-premise) and generic HR suites (that don't understand asociados vs employees), DevManagerAPI Solidario combines cooperative-domain data models, mandatory regulatory compliance (Ley 79/1988, Ley 454/1998, Circular Básica Jurídica 2020, Ley 1581/2012), and a Gemini-powered IA assistant — all in a multi-tenant SaaS architecture ready for any cooperativa, mutual, or fondo de empleados in Colombia.

## Risks

1. **Supersolidaria Circular Básica Jurídica 2020 not yet consulted**: The primary regulatory "bible" was unreachable during exploration. This MUST be acquired and analyzed during the spec/design phase to ensure compliance. The risk is that some detailed procedural requirements may alter entity designs.
2. **Competitive landscape unverified**: Siarsoft, Ascoop, and other competitive intelligence sources were unreachable. The gap analysis relies on prior knowledge; verify during propose/spec phase through direct contact or demos.
3. **Scope creep risk**: 7 new bounded contexts is ambitious. The roadmap's phased approach mitigates this, but each phase must be rigorously scoped with the user before starting.
4. **Gemini domain-prompt quality**: The cooperative assistant's usefulness depends heavily on the quality of system prompts and the regulatory knowledge base. This is non-trivial and must be treated as an engineering task, not a configuration step.
5. **Zero test baseline**: Phase 0 must complete before any domain work. The existing seed has no regression safety. One bad commit could break the entire API without detection.
6. **Frontend effort is out of scope**: The explore phase doesn't touch DevManagerFront (Angular). The frontend will need significant rework for cooperative-specific UIs (admission forms, assembly voting, education dashboards). This must be planned separately.
7. **Single-developer bus factor**: The project needs growth or clear knowledge transfer paths.

## Ready for Proposal

**Yes**. The exploration reveals a clear, phased evolution path from the existing seed to a full solidarity-sector HR platform. The regulatory framework is well-understood (Ley 79/1988, Ley 454/1998), the bounded context map is coherent, and the first executable slice is costed and fits the PR budget. The two unverified sources (Circular Básica, competitor landscape) should be noted as risks but don't block proposal — they can be filled during spec/design.

**What the orchestrator should tell the user**: "The exploration is complete. We have a 6-phase roadmap starting with foundation (tests + CI + editorconfig), then nucleo solidario (asociados + aportes + habeas data), then full HR, compliance, governance, and AI. The first slice is a 380-line PR to set up xUnit tests, CI, and editorconfig — fits within our 400-line budget. 7 new bounded contexts identified, 4 existing ones evolved. Ready for the proposal phase if you want to proceed."
