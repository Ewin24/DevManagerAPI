namespace API.Authorization;

/// <summary>
/// Nombres de roles usados en [Authorize(Roles = ...)]
/// </summary>
public static class AppRoles
{
    /// <summary>
    /// Roles administradores (seeder: "Admin"; frontend RoleGuard: admin/administrator).
    /// La comparación de roles en ASP.NET Core es sensible a mayúsculas.
    /// </summary>
    public const string Admin = "Admin,Administrator";
}
