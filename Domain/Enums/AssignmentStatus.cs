namespace Domain.Enums;

/// <summary>
/// Estado de una asignación activa a proyecto
/// </summary>
public enum AssignmentStatus : byte
{
    /// <summary>
    /// Asignación activa, usuario trabajando
    /// </summary>
    Active = 1,

    /// <summary>
    /// Usuario removido del proyecto
    /// </summary>
    Removed = 2,

    /// <summary>
    /// Asignación completada exitosamente
    /// </summary>
    Completed = 3
}
