using Application.DTOs.Agent;

namespace Application.Interfaces;

/// <summary>
/// Servicio de Asistente Cooperativo IA
/// Gestión de consultas normativas, balance social, cumplimiento y atención al asociado
/// Basado en Ley 79/1988, Ley 454/1998, Circular Básica Jurídica 2020
/// </summary>
public interface ICooperativaAIService
{
    /// <summary>
    /// Consulta general al asistente cooperativo en lenguaje natural
    /// Responde con citaciones a la normatividad Supersolidaria aplicable
    /// </summary>
    Task<CooperativaQueryResponse> ConsultarNormatividadAsync(
        Guid organizationId,
        CooperativaQueryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Genera un reporte de Balance Social compilando indicadores de todas las dimensiones
    /// </summary>
    Task<BalanceSocialReportDto> GenerarBalanceSocialAsync(
        Guid organizationId,
        GenerarBalanceSocialRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica el cumplimiento normativo de la cooperativa en múltiples áreas
    /// (educación, SST, habeas data, aportes)
    /// </summary>
    Task<CumplimientoDto> VerificarCumplimientoAsync(
        Guid organizationId,
        VerificarCumplimientoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Responde dudas de asociados sobre sus derechos, deberes y trámites
    /// </summary>
    Task<CooperativaQueryResponse> ResponderDudaAsociadoAsync(
        Guid organizationId,
        ResponderDudaRequest request,
        CancellationToken cancellationToken = default);
}
