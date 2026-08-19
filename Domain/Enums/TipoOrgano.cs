namespace Domain.Enums;

/// <summary>
/// Tipos de órganos de administración y control — Ley 79 art.26-45
/// </summary>
public enum TipoOrgano
{
    /// <summary>Asamblea General de Asociados</summary>
    AsambleaGeneral = 1,

    /// <summary>Consejo de Administración</summary>
    ConsejoAdministracion = 2,

    /// <summary>Junta de Vigilancia</summary>
    JuntaVigilancia = 3,

    /// <summary>Revisor Fiscal</summary>
    RevisorFiscal = 4,

    /// <summary>Comité</summary>
    Comite = 5,

    /// <summary>Otros órganos</summary>
    Otros = 99
}
