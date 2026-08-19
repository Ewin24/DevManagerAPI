namespace Application.Interfaces;

using Application.DTOs.Bienestar;

/// <summary>
/// Servicio de bienestar y compensación social — auxilios, becas,
/// créditos blandos, y fondo de solidaridad (Ley 79 art.54)
/// </summary>
public interface IBienestarService
{
    // ===== Programas de Bienestar =====
    /// <summary>Obtiene todos los programas de bienestar activos</summary>
    Task<List<ProgramaBienestarDto>> GetProgramasAsync(Guid organizationId);

    /// <summary>Crea un nuevo programa de bienestar</summary>
    Task<ProgramaBienestarDto> CreateProgramaAsync(CreateProgramaBienestarDto dto);

    // ===== Solicitudes =====
    /// <summary>Crea una solicitud de bienestar</summary>
    Task<SolicitudBienestarDto> CreateSolicitudAsync(CreateSolicitudBienestarDto dto);

    /// <summary>Obtiene las solicitudes de un asociado</summary>
    Task<List<SolicitudBienestarDto>> GetSolicitudesByAsociadoAsync(Guid asociadoId);

    /// <summary>Aprueba una solicitud de bienestar (establece monto aprobado)</summary>
    Task<SolicitudBienestarDto> AprobarSolicitudAsync(Guid solicitudId, decimal montoAprobado, Guid resueltoPorUserId);

    /// <summary>Rechaza una solicitud de bienestar</summary>
    Task<SolicitudBienestarDto> RechazarSolicitudAsync(Guid solicitudId, string observaciones, Guid resueltoPorUserId);

    // ===== Auxilios =====
    /// <summary>Entrega un auxilio a un asociado</summary>
    Task<AuxilioDto> EntregarAuxilioAsync(Guid asociadoId, Guid organizationId, Guid? solicitudId,
        Domain.Enums.TipoAuxilio tipo, decimal monto, string concepto, bool requiereReintegro);

    /// <summary>Obtiene los auxilios de un asociado</summary>
    Task<List<AuxilioDto>> GetAuxiliosByAsociadoAsync(Guid asociadoId);

    // ===== Fondo de Solidaridad =====
    /// <summary>Calcula y registra el aporte al fondo (10% excedentes)</summary>
    Task<FondoSolidaridadDto> CalcularAporteFondoAsync(Guid organizationId, DateTime periodo, decimal totalExcedentes);

    /// <summary>Obtiene el estado actual del fondo</summary>
    Task<FondoSolidaridadDto?> GetFondoActualAsync(Guid organizationId);
}
