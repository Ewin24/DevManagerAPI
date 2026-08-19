namespace Application.Interfaces;

using Application.DTOs.HabeasData;

/// <summary>
/// Servicio de Habeas Data según Ley 1581/2012
/// Gestiona autorizaciones, tratamiento de datos y solicitudes ARCO
/// </summary>
public interface IHabeasDataService
{
    // ===== Autorizaciones =====

    /// <summary>Registra una autorización de tratamiento de datos</summary>
    Task<AutorizacionDto> RegistrarAutorizacionAsync(CreateAutorizacionDto dto);

    /// <summary>Revoca una autorización de tratamiento de datos</summary>
    Task<AutorizacionDto> RevocarAutorizacionAsync(Guid autorizacionId);

    /// <summary>Obtiene la autorización vigente de un asociado</summary>
    Task<AutorizacionDto?> GetAutorizacionVigenteAsync(Guid asociadoId);

    /// <summary>Verifica si un asociado tiene autorización vigente</summary>
    Task<bool> TieneAutorizacionVigenteAsync(Guid asociadoId);

    // ===== Solicitudes ARCO =====

    /// <summary>Registra una solicitud ARCO (Acceso, Rectificación, Cancelación, Oposición)</summary>
    Task<SolicitudARCODto> CrearSolicitudARCOAsync(CreateSolicitudARCODto dto);

    /// <summary>Atiende una solicitud ARCO</summary>
    Task<SolicitudARCODto> AtenderSolicitudARCOAsync(Guid solicitudId, string respuesta);

    /// <summary>Rechaza una solicitud ARCO con justificación</summary>
    Task<SolicitudARCODto> RechazarSolicitudARCOAsync(Guid solicitudId, string motivoRechazo);

    /// <summary>Obtiene las solicitudes ARCO de un asociado</summary>
    Task<List<SolicitudARCODto>> GetSolicitudesARCOByAsociadoAsync(Guid asociadoId);

    /// <summary>Obtiene las solicitudes ARCO pendientes de una organización</summary>
    Task<List<SolicitudARCODto>> GetSolicitudesARCOPendientesAsync(Guid organizationId);
}
