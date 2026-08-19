namespace Domain.Entities.GestionHumana;

using Domain.Common;
using Domain.Enums;

/// <summary>
/// Programa de educación cooperativa ofrecido a los asociados
/// </summary>
public class ProgramaEducacion : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }

    /// <summary>Nombre del programa educativo</summary>
    public string Nombre { get; set; } = null!;

    /// <summary>Descripción del contenido y objetivos</summary>
    public string? Descripcion { get; set; }

    /// <summary>Tipo de educación: básica, avanzada, especializada</summary>
    public TipoEducacion Tipo { get; set; }

    /// <summary>Horas totales del programa</summary>
    public int Horas { get; set; }

    /// <summary>¿Es obligatorio según la normativa solidaria?</summary>
    public bool EsObligatorio { get; set; }

    /// <summary>Fecha de inicio del programa</summary>
    public DateTime FechaInicio { get; set; }

    /// <summary>Fecha de finalización del programa</summary>
    public DateTime? FechaFin { get; set; }

    /// <summary>¿Está activo el programa?</summary>
    public bool Activo { get; set; } = true;
}
