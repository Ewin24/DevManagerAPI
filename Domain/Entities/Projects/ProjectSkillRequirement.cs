namespace Domain.Entities.Projects;

using Domain.Entities.IAM;
using Domain.Entities.Talent;

/// <summary>
/// Requisitos de habilidades para un proyecto (perfil ideal)
/// </summary>
public class ProjectSkillRequirement
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid SkillId { get; set; }
    public byte RequiredLevel { get; set; } // 1-5
    public bool IsMandatory { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Propiedades calculadas (usadas por servicios)
    public string SkillName { get; set; } = string.Empty;

    // Navegación
    public Organization? Organization { get; set; }
    public Project? Project { get; set; }
    public Skill? Skill { get; set; }
}
