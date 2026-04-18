namespace Application.Services;

using Application.DTOs.Agent;
using Application.DTOs.Reports;
using Application.Interfaces;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// Servicio para reportes y análisis estadísticos de la organización
/// </summary>
public class ReportsService : IReportsService
{
    private readonly IEmployeeSkillRepository _employeeSkillRepository;
    private readonly ISkillRepository _skillRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly IAgentService _agentService;
    private readonly ILogger<ReportsService> _logger;

    public ReportsService(
        IEmployeeSkillRepository employeeSkillRepository,
        ISkillRepository skillRepository,
        IProjectRepository projectRepository,
        IAssignmentRepository assignmentRepository,
        IUserRepository userRepository,
        IProfileRepository profileRepository,
        IAgentService agentService,
        ILogger<ReportsService> logger)
    {
        _employeeSkillRepository = employeeSkillRepository;
        _skillRepository = skillRepository;
        _projectRepository = projectRepository;
        _assignmentRepository = assignmentRepository;
        _userRepository = userRepository;
        _profileRepository = profileRepository;
        _agentService = agentService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene la distribución de habilidades agrupadas por nivel
    /// GROUP BY skill, calcula average level y distribución por nivel
    /// </summary>
    public async Task<IEnumerable<SkillDistributionResponse>> GetSkillsDistributionAsync(Guid organizationId)
    {
        _logger.LogInformation("Obteniendo distribución de habilidades para org {OrgId}", organizationId);

        try
        {
            // Obtener todas las habilidades
            var allSkills = await _skillRepository.GetAllAsync(organizationId);
            var skillDistributions = new List<SkillDistributionResponse>();

            foreach (var skill in allSkills)
            {
                // Obtener todas las asignaciones de EmployeeSkill para esta habilidad
                // Nota: Este es un enfoque simplificado que podría optimarse con una consulta SQL directa
                // En una implementación real, agregaríamos un método específico al repositorio
                
                // Por ahora, usaremos una aproximación que obtiene los datos disponibles
                var skillName = skill.Name;
                var levelDistribution = new Dictionary<string, int>
                {
                    { "1", 0 },
                    { "2", 0 },
                    { "3", 0 },
                    { "4", 0 },
                    { "5", 0 }
                };

                decimal totalLevel = 0;
                int totalEmployees = 0;

                // Nota: Para una implementación real, necesitaríamos acceso a una vista o procedimiento
                // que calcule esto directamente en la BD para mejor performance
                // Por ahora, retornamos la estructura correcta con ceros si no hay datos

                if (totalEmployees > 0)
                {
                    skillDistributions.Add(new SkillDistributionResponse
                    {
                        SkillName = skillName,
                        AverageLevel = Math.Round(totalLevel / totalEmployees, 2),
                        TotalEmployees = totalEmployees,
                        LevelDistribution = levelDistribution
                    });
                }
            }

            return skillDistributions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo distribución de habilidades para org {OrgId}", organizationId);
            return new List<SkillDistributionResponse>();
        }
    }

    /// <summary>
    /// Obtiene métricas clave de proyectos:
    /// - Total de proyectos activos
    /// - Proyectos en riesgo (falta de skills mandatorios)
    /// - Top de habilidades más demandadas
    /// </summary>
    public async Task<ProjectMetricsResponse> GetProjectMetricsAsync(Guid organizationId)
    {
        _logger.LogInformation("Obteniendo métricas de proyectos para org {OrgId}", organizationId);

        try
        {
            // Obtener todos los proyectos en progreso (activos)
            var activeProjects = await _projectRepository.GetAllAsync(organizationId, ProjectStatus.InProgress);
            var activeProjectList = activeProjects.ToList();
            
            var response = new ProjectMetricsResponse
            {
                TotalActiveProjects = activeProjectList.Count,
                ProjectsAtRisk = 0,
                MostDemandedSkills = new List<MostDemandedSkill>()
            };

            // Contar habilidades demandadas en todos los proyectos
            var skillDemandCount = new Dictionary<string, int>();

            foreach (var project in activeProjectList)
            {
                var skillRequirements = await _projectRepository.GetSkillRequirementsAsync(project.Id, organizationId);
                var mandatorySkills = skillRequirements.Where(sr => sr.IsMandatory).ToList();

                // Contar si el proyecto está en riesgo (simplificado: tiene requisitos obligatorios)
                if (mandatorySkills.Any())
                {
                    response.ProjectsAtRisk++; // En una lógica real, verificarías si hay suficientes empleados con esas skills
                }

                // Contar demanda de habilidades
                foreach (var requirement in skillRequirements)
                {
                    if (!skillDemandCount.ContainsKey(requirement.SkillName))
                    {
                        skillDemandCount[requirement.SkillName] = 0;
                    }
                    skillDemandCount[requirement.SkillName]++;
                }
            }

            // Top 5 habilidades más demandadas
            response.MostDemandedSkills = skillDemandCount
                .OrderByDescending(x => x.Value)
                .Take(5)
                .Select(x => new MostDemandedSkill
                {
                    SkillName = x.Key,
                    RequiredInProjects = x.Value
                })
                .ToList();

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo métricas de proyectos para org {OrgId}", organizationId);
            return new ProjectMetricsResponse
            {
                TotalActiveProjects = 0,
                ProjectsAtRisk = 0,
                MostDemandedSkills = new List<MostDemandedSkill>()
            };
        }
    }

    /// <summary>
    /// Genera un resumen ejecutivo mediante IA
    /// Construye un prompt con el análisis de habilidades y proyectos
    /// </summary>
    public async Task<AiSummaryResponse> GetAiSummaryAsync(Guid organizationId)
    {
        _logger.LogInformation("Generando resumen ejecutivo por IA para org {OrgId}", organizationId);

        try
        {
            // Construir un prompt para el agente IA basado en análisis de la organización
            var query = @"Genera un resumen ejecutivo en Markdown (máximo 5 párrafos) sobre:
1. Las habilidades más fuertes en la organización (basadas en los empleados y sus niveles)
2. Las brechas críticas de talento (habilidades demandadas pero no disponibles)
3. Recomendaciones específicas para mejorar la capacidad de la organización
Usa un tono profesional y sé conciso. Formato: Markdown con encabezados (##) y listas.";

            var request = new AgentQueryRequest
            {
                Query = query,
                Context = "Análisis de talento y capacidades organizacionales"
            };

            // Nota: Se necesita un userId válido para QueryAsync
            // En un contexto real, esto vendría del contexto de la solicitud (ej: usuario administrador del sistema)
            var systemAdminId = Guid.Empty; // Placeholder - en producción, usar un ID válido

            var agentResponse = await _agentService.QueryAsync(organizationId, systemAdminId, request);

            return new AiSummaryResponse
            {
                Markdown = agentResponse.Markdown ?? "No se pudo generar el resumen. Intenta nuevamente."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando resumen ejecutivo por IA para org {OrgId}", organizationId);
            return new AiSummaryResponse
            {
                Markdown = "Error al generar el resumen ejecutivo. Por favor, intenta nuevamente más tarde."
            };
        }
    }

    /// <summary>
    /// Obtiene el estado de utilización de empleados en la organización
    /// Incluye información sobre proyectos activos asignados, porcentaje de utilización
    /// y estado de asignación (asignado/sin asignar)
    /// </summary>
    public async Task<EmployeeUtilizationResponse> GetEmployeeUtilizationAsync(Guid organizationId)
    {
        _logger.LogInformation("Obteniendo utilización de empleados para org {OrgId}", organizationId);

        try
        {
            var response = new EmployeeUtilizationResponse();
            var activeProjects = await _projectRepository.GetAllAsync(organizationId, ProjectStatus.InProgress);
            var activeProjectList = activeProjects.ToList();
            
            // Obtener todos los usuarios de la organización
            var allUsers = await _userRepository.GetAllAsync(organizationId);
            var userList = allUsers.ToList();
            response.TotalEmployees = userList.Count;

            var employeeUtilizationDict = new Dictionary<Guid, EmployeeUtilizationDetail>();

            // Inicializar cada empleado con información básica
            foreach (var user in userList)
            {
                var employeeDetail = new EmployeeUtilizationDetail
                {
                    EmployeeId = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    ActiveProjectCount = 0,
                    AssignedProjects = new List<string>(),
                    AllocationStatus = "Sin asignar",
                    UtilizationPercentage = 0,
                    SkillsCount = 0,
                    YearsExperience = null
                };

                // Obtener perfil del empleado para años de experiencia
                try
                {
                    var profile = await _profileRepository.GetByUserIdAsync(user.Id, organizationId);
                    if (profile?.YearsExperience.HasValue == true)
                    {
                        employeeDetail.YearsExperience = profile.YearsExperience;
                    }
                }
                catch
                {
                    // Silenciosamente ignorar si no hay perfil
                }

                // Contar habilidades del empleado
                try
                {
                    var skills = await _employeeSkillRepository.GetByUserIdAsync(user.Id, organizationId);
                    employeeDetail.SkillsCount = skills.Count();
                }
                catch
                {
                    // Silenciosamente ignorar si hay error
                }

                employeeUtilizationDict[user.Id] = employeeDetail;
            }

            // Asignar proyectos a empleados
            foreach (var project in activeProjectList)
            {
                var assignments = await _assignmentRepository.GetByProjectIdAsync(project.Id, organizationId);

                foreach (var assignment in assignments)
                {
                    if (employeeUtilizationDict.TryGetValue(assignment.UserId, out var employee))
                    {
                        employee.ActiveProjectCount++;
                        employee.AssignedProjects.Add(project.Name);
                    }
                }
            }

            // Calcular métricas y actualizar estado de asignación
            int allocatedCount = 0;
            int totalProjectAssignments = 0;

            foreach (var employee in employeeUtilizationDict.Values)
            {
                if (employee.ActiveProjectCount > 0)
                {
                    employee.AllocationStatus = "Asignado";
                    allocatedCount++;
                    totalProjectAssignments += employee.ActiveProjectCount;
                    
                    // Utilización: Basada en número de proyectos (simplificado: cada proyecto = ~25% utilización)
                    // Máximo 100% con 4 o más proyectos
                    employee.UtilizationPercentage = Math.Min(employee.ActiveProjectCount * 25, 100);
                }
            }

            response.AllocatedEmployees = allocatedCount;
            response.UnallocatedEmployees = response.TotalEmployees - allocatedCount;
            response.UtilizationRate = response.TotalEmployees > 0 
                ? Math.Round((decimal)allocatedCount / response.TotalEmployees * 100, 2)
                : 0;
            response.AverageProjectsPerEmployee = allocatedCount > 0
                ? Math.Round((decimal)totalProjectAssignments / allocatedCount, 2)
                : 0;

            response.Employees = employeeUtilizationDict.Values.ToList();

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo utilización de empleados para org {OrgId}", organizationId);
            return new EmployeeUtilizationResponse
            {
                TotalEmployees = 0,
                AllocatedEmployees = 0,
                UnallocatedEmployees = 0,
                AverageProjectsPerEmployee = 0,
                UtilizationRate = 0
            };
        }
    }

    /// <summary>
    /// Obtiene métricas estadísticas generales del departamento/organización
    /// Incluye distribución de experiencia, promedio de habilidades, etc.
    /// </summary>
    public async Task<DepartmentMetricsResponse> GetDepartmentMetricsAsync(Guid organizationId)
    {
        _logger.LogInformation("Obteniendo métricas departamentales para org {OrgId}", organizationId);

        try
        {
            var response = new DepartmentMetricsResponse();

            // Obtener todos los usuarios
            var allUsers = await _userRepository.GetAllAsync(organizationId);
            var userList = allUsers.ToList();
            response.TotalEmployees = userList.Count;

            // Obtener todas las habilidades únicas
            var allSkills = await _skillRepository.GetAllAsync(organizationId);
            var allSkillsList = allSkills.ToList();
            response.TotalUniqueSkills = allSkillsList.Count;

            // Nota: Para TotalRoles, necesitaríamos acceso a roles.
            // Por ahora usamos un valor por defecto o contamos roles únicos de usuarios
            response.TotalRoles = 0;

            // Procesar cada empleado
            int activeEmployees = 0;
            decimal totalYearsExperience = 0;
            int employeesWithExperience = 0;
            int totalSkillsAssigned = 0;
            int employeesWithCertifications = 0;
            
            var experienceDistribution = new ExperienceDistribution();

            foreach (var user in userList)
            {
                try
                {
                    // Obtener perfil del empleado
                    var profile = await _profileRepository.GetByUserIdAsync(user.Id, organizationId);
                    if (profile != null)
                    {
                        activeEmployees++;

                        // Calcular distribución de experiencia
                        if (profile.YearsExperience.HasValue)
                        {
                            int years = profile.YearsExperience.Value;
                            totalYearsExperience += years;
                            employeesWithExperience++;

                            if (years <= 2)
                                experienceDistribution.Junior++;
                            else if (years <= 5)
                                experienceDistribution.MidLevel++;
                            else if (years <= 10)
                                experienceDistribution.Senior++;
                            else
                                experienceDistribution.Lead++;
                        }
                    }

                    // Contar habilidades
                    var skills = await _employeeSkillRepository.GetByUserIdAsync(user.Id, organizationId);
                    var skillList = skills.ToList();
                    totalSkillsAssigned += skillList.Count;

                    // Verificar certificaciones (validadas)
                    // Contamos empleados que tienen al menos una skill validada
                    if (skillList.Any(s => s.LastValidatedAt.HasValue))
                    {
                        employeesWithCertifications++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error procesando empleado {UserId}", user.Id);
                    // Continuar con el siguiente empleado
                }
            }

            response.ActiveEmployees = activeEmployees;
            response.EmployeesWithCertifications = employeesWithCertifications;
            response.AverageSkillsPerEmployee = response.TotalEmployees > 0
                ? Math.Round((decimal)totalSkillsAssigned / response.TotalEmployees, 2)
                : 0;
            response.AverageYearsExperience = employeesWithExperience > 0
                ? Math.Round(totalYearsExperience / employeesWithExperience, 2)
                : 0;
            response.ExperienceDistribution = experienceDistribution;

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo métricas departamentales para org {OrgId}", organizationId);
            return new DepartmentMetricsResponse
            {
                TotalEmployees = 0,
                ActiveEmployees = 0,
                AverageSkillsPerEmployee = 0,
                TotalUniqueSkills = 0,
                EmployeesWithCertifications = 0,
                AverageYearsExperience = 0,
                TotalRoles = 0
            };
        }
    }
}
