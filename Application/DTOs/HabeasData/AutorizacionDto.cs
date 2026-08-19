namespace Application.DTOs.HabeasData;

/// <summary>
/// DTO para autorización de tratamiento de datos (Ley 1581/2012)
/// </summary>
public record AutorizacionDto
{
    public Guid Id { get; init; }
    public Guid AsociadoId { get; init; }
    public Guid OrganizationId { get; init; }
    public DateTime FechaAutorizacion { get; init; }
    public DateTime? Vigencia { get; init; }
    public bool Revocada { get; init; }
    public DateTime? FechaRevocacion { get; init; }
    public string Finalidad { get; init; } = null!;
    public string MedioAutorizacion { get; init; } = null!;
    public string? DireccionIp { get; init; }
    public bool Vigente => !Revocada && (!Vigencia.HasValue || Vigencia.Value >= DateTime.UtcNow);
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// DTO para registrar una autorización de datos
/// </summary>
public record CreateAutorizacionDto
{
    public Guid AsociadoId { get; init; }
    public Guid OrganizationId { get; init; }
    public string Finalidad { get; init; } = null!;
    public string MedioAutorizacion { get; init; } = "Digital";
    public string? DireccionIp { get; init; }
}
