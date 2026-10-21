namespace Application.DTOs.Solidarity;

using System.ComponentModel.DataAnnotations;

public class CreateDataPolicyRequest
{
    [Required]
    [MaxLength(20)]
    public string Version { get; set; } = null!;

    [Required]
    [MaxLength(300)]
    public string Purpose { get; set; } = null!;

    [Required]
    public string Text { get; set; } = null!;

    /// <summary>Vigente desde; por defecto hoy (UTC)</summary>
    public DateOnly? EffectiveFrom { get; set; }
}

public class DataPolicyDto
{
    public Guid Id { get; set; }
    public string Version { get; set; } = null!;
    public string Purpose { get; set; } = null!;
    public string Text { get; set; } = null!;
    public DateOnly EffectiveFrom { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GrantConsentRequest
{
    [Required]
    public Guid PolicyVersionId { get; set; }
}

public class RevokeConsentRequest
{
    [Required]
    [MaxLength(400)]
    public string Reason { get; set; } = null!;
}

public class ConsentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PolicyVersionId { get; set; }
    public string PolicyVersion { get; set; } = null!;
    public string Purpose { get; set; } = null!;
    public DateTime GrantedAt { get; set; }
    public Guid GrantedByUserId { get; set; }
    public DateTime? RevokedAt { get; set; }
    public Guid? RevokedByUserId { get; set; }
    public string? RevocationReason { get; set; }
    public bool IsActive { get; set; }
}
