namespace Application.Interfaces;

using Application.DTOs.SST;

/// <summary>
/// Servicio de Salud Ocupacional y SST
/// SG-SST per Decreto 1072/2015 + Res. 0312/2019
/// Gestiona exámenes médicos, ARL, FURAT y matriz de riesgos
/// </summary>
public interface ISstService
{
    // ===== Exámenes Médicos =====

    /// <summary>Programa un examen médico ocupacional</summary>
    Task<ExamenMedicoDto> ProgramarExamenAsync(CreateExamenMedicoDto dto);

    /// <summary>Registra el resultado de un examen realizado</summary>
    Task<ExamenMedicoDto> RegistrarExamenAsync(Guid examenId, string resultado, string? archivoUrl, string? observaciones);

    /// <summary>Obtiene los exámenes de un asociado</summary>
    Task<List<ExamenMedicoDto>> GetExamenesByAsociadoAsync(Guid asociadoId);

    /// <summary>Obtiene los exámenes pendientes de una organización</summary>
    Task<List<ExamenMedicoDto>> GetExamenesPendientesAsync(Guid organizationId);

    // ===== ARL Vigencia =====

    /// <summary>Verifica si la ARL está vigente (alerta 30 días antes de expiry)</summary>
    Task<(bool Vigente, int DiasRestantes, string? Alerta)> VerificarVigenciaArlAsync(Guid organizationId);

    // ===== Accidentes =====

    /// <summary>Reporta un accidente de trabajo (FURAT)</summary>
    Task<AccidenteDto> ReportarAccidenteAsync(CreateAccidenteDto dto);

    /// <summary>Obtiene accidentes pendientes de investigación</summary>
    Task<List<AccidenteDto>> GetAccidentesPendientesInvestigacionAsync(Guid organizationId);

    /// <summary>Registra la investigación de un accidente</summary>
    Task<AccidenteDto> RegistrarInvestigacionAsync(Guid accidenteId, DateTime fechaInvestigacion,
        string conclusiones, string causas, string medidasCorrectivas);

    /// <summary>Obtiene los accidentes de una organización</summary>
    Task<List<AccidenteDto>> GetAccidentesByOrganizacionAsync(Guid organizationId);

    // ===== Riesgos =====

    /// <summary>Obtiene la matriz de riesgos de una organización</summary>
    Task<List<RiesgoDto>> GetRiesgosAsync(Guid organizationId);

    /// <summary>Agrega un riesgo a la matriz</summary>
    Task<RiesgoDto> CrearRiesgoAsync(CreateRiesgoDto dto);
}
