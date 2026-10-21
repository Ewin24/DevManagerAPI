namespace Domain.Enums;

/// <summary>
/// Estados del ciclo de vida de un asociado
/// </summary>
public enum AssociateStatus : byte
{
    Applicant = 1,
    Active = 2,
    Suspended = 3,
    Withdrawn = 4
}

/// <summary>
/// Rol derivado de un usuario: asociado (registro no retirado), empleado (bandera explícita), ambos o ninguno
/// </summary>
public enum AssociateRole
{
    None = 0,
    Associate = 1,
    Employee = 2,
    Both = 3
}
