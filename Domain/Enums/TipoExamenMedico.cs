namespace Domain.Enums;

/// <summary>
/// Tipos de examen médico ocupacional según Res. 0312/2019
/// </summary>
public enum TipoExamenMedico : byte
{
    /// <summary>Examen de ingreso (preocupacional)</summary>
    Ingreso = 1,

    /// <summary>Examen periódico (vigilancia epidemiológica)</summary>
    Periodico = 2,

    /// <summary>Examen de retiro (egreso)</summary>
    Retiro = 3
}
