namespace Domain.Entities.Talent;

using Domain.Entities.IAM;

/// <summary>
/// Catálogo maestro de habilidades (técnicas y blandas)
/// </summary>
public class Skill
{
    public Guid Id { get; set; }
    public Guid? OrganizationId { get; set; } // NULL = catálogo global
    public string Name { get; set; } = null!;
    public string? Category { get; set; }
    public string? SkillType { get; set; } // 'Hard', 'Soft', 'Language'
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Navegación
    public Organization? Organization { get; set; }
}
