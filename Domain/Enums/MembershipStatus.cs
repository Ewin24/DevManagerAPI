namespace Domain.Enums;

/// <summary>
/// Estado de membresía de una persona en una organización solidaria
/// </summary>
public enum MembershipStatus : byte
{
    /// <summary>Membresía activa</summary>
    Active = 1,

    /// <summary>Membresía suspendida</summary>
    Suspended = 2,

    /// <summary>Membresía retirada/cancelada</summary>
    Retired = 3
}
