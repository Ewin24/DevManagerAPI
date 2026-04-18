namespace Application.DTOs.Reports;

/// <summary>
/// Respuesta de distribución de habilidades por nivel
/// </summary>
public class SkillDistributionResponse
{
    /// <summary>
    /// Nombre de la habilidad
    /// </summary>
    public string SkillName { get; set; } = null!;

    /// <summary>
    /// Nivel promedio de la habilidad (0-5)
    /// </summary>
    public decimal AverageLevel { get; set; }

    /// <summary>
    /// Total de empleados que tienen esta habilidad
    /// </summary>
    public int TotalEmployees { get; set; }

    /// <summary>
    /// Distribución de empleados por nivel (1-5)
    /// Clave: nivel (1-5), Valor: cantidad de empleados
    /// </summary>
    public Dictionary<string, int> LevelDistribution { get; set; } = new();
}

/// <summary>
/// Habilidad más demandada en proyectos
/// </summary>
public class MostDemandedSkill
{
    /// <summary>
    /// Nombre de la habilidad
    /// </summary>
    public string SkillName { get; set; } = null!;

    /// <summary>
    /// Cantidad de proyectos que requieren esta habilidad
    /// </summary>
    public int RequiredInProjects { get; set; }
}

/// <summary>
/// Métricas de proyectos de la organización
/// </summary>
public class ProjectMetricsResponse
{
    /// <summary>
    /// Total de proyectos activos
    /// </summary>
    public int TotalActiveProjects { get; set; }

    /// <summary>
    /// Cantidad de proyectos en riesgo (sin suficientes empleados con habilidades requeridas)
    /// </summary>
    public int ProjectsAtRisk { get; set; }

    /// <summary>
    /// Top de habilidades más demandadas en proyectos
    /// </summary>
    public List<MostDemandedSkill> MostDemandedSkills { get; set; } = new();
}

/// <summary>
/// Resumen ejecutivo generado por IA
/// </summary>
public class AiSummaryResponse
{
    /// <summary>
    /// Contenido del resumen en formato Markdown
    /// </summary>
    public string Markdown { get; set; } = null!;
}

/// <summary>
/// Información de utilización de un empleado
/// </summary>
public class EmployeeUtilizationDetail
{
    /// <summary>
    /// ID del empleado
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Nombre completo del empleado
    /// </summary>
    public string FullName { get; set; } = null!;

    /// <summary>
    /// Email del empleado
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Número de proyectos activos asignados
    /// </summary>
    public int ActiveProjectCount { get; set; }

    /// <summary>
    /// Nombres de los proyectos activos
    /// </summary>
    public List<string> AssignedProjects { get; set; } = new();

    /// <summary>
    /// Porcentaje de utilización estimado (0-100)
    /// Basado en cantidad de proyectos activos
    /// </summary>
    public int UtilizationPercentage { get; set; }

    /// <summary>
    /// Estado: "Asignado" si tiene proyectos activos, "Sin asignar" si no
    /// </summary>
    public string AllocationStatus { get; set; } = null!;

    /// <summary>
    /// Número de habilidades que posee
    /// </summary>
    public int SkillsCount { get; set; }

    /// <summary>
    /// Años de experiencia (si está disponible)
    /// </summary>
    public int? YearsExperience { get; set; }
}

/// <summary>
/// Resumen de utilización de empleados
/// </summary>
public class EmployeeUtilizationResponse
{
    /// <summary>
    /// Total de empleados en la organización
    /// </summary>
    public int TotalEmployees { get; set; }

    /// <summary>
    /// Cantidad de empleados asignados a proyectos activos
    /// </summary>
    public int AllocatedEmployees { get; set; }

    /// <summary>
    /// Cantidad de empleados sin proyectos asignados
    /// </summary>
    public int UnallocatedEmployees { get; set; }

    /// <summary>
    /// Promedio de proyectos por empleado asignado
    /// </summary>
    public decimal AverageProjectsPerEmployee { get; set; }

    /// <summary>
    /// Porcentaje de utilización general (empleados asignados / total)
    /// </summary>
    public decimal UtilizationRate { get; set; }

    /// <summary>
    /// Detalle de cada empleado y su utilización
    /// </summary>
    public List<EmployeeUtilizationDetail> Employees { get; set; } = new();
}

/// <summary>
/// Métricas de departamento/equipo
/// </summary>
public class DepartmentMetricsResponse
{
    /// <summary>
    /// Total de empleados en la organización
    /// </summary>
    public int TotalEmployees { get; set; }

    /// <summary>
    /// Número de empleados activos (con proyectos o habilidades)
    /// </summary>
    public int ActiveEmployees { get; set; }

    /// <summary>
    /// Promedio de habilidades por empleado
    /// </summary>
    public decimal AverageSkillsPerEmployee { get; set; }

    /// <summary>
    /// Total de habilidades únicas en la organización
    /// </summary>
    public int TotalUniqueSkills { get; set; }

    /// <summary>
    /// Número de empleados con experiencia certificada
    /// </summary>
    public int EmployeesWithCertifications { get; set; }

    /// <summary>
    /// Promedio de años de experiencia en la organización
    /// </summary>
    public decimal AverageYearsExperience { get; set; }

    /// <summary>
    /// Cantidad de roles diferentes en la organización
    /// </summary>
    public int TotalRoles { get; set; }

    /// <summary>
    /// Distribución de empleados por rango de experiencia
    /// </summary>
    public ExperienceDistribution ExperienceDistribution { get; set; } = new();
}

/// <summary>
/// Distribución de empleados por rango de experiencia
/// </summary>
public class ExperienceDistribution
{
    /// <summary>
    /// Empleados con 0-2 años de experiencia (Junior)
    /// </summary>
    public int Junior { get; set; }

    /// <summary>
    /// Empleados con 3-5 años de experiencia (Mid-level)
    /// </summary>
    public int MidLevel { get; set; }

    /// <summary>
    /// Empleados con 6-10 años de experiencia (Senior)
    /// </summary>
    public int Senior { get; set; }

    /// <summary>
    /// Empleados con más de 10 años de experiencia (Lead/Principal)
    /// </summary>
    public int Lead { get; set; }
}
