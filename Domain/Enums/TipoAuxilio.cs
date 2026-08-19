namespace Domain.Enums;

/// <summary>
/// Tipos de auxilio o beneficio del fondo de bienestar social
/// </summary>
public enum TipoAuxilio : byte
{
    /// <summary>Auxilio económico por calamidad o emergencia</summary>
    AuxilioEconomico = 1,

    /// <summary>Beca educativa para asociado o beneficiarios</summary>
    BecaEducativa = 2,

    /// <summary>Crédito blando con tasa preferencial</summary>
    CreditoBlando = 3,

    /// <summary>Auxilio funerario</summary>
    AuxilioFunerario = 4,

    /// <summary>Apoyo para vivienda</summary>
    ApoyoVivienda = 5,

    /// <summary>Otro tipo de beneficio</summary>
    Otro = 99
}
