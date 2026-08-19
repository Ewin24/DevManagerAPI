namespace Application.Interfaces;

using Application.DTOs.BalanceSocial;

/// <summary>
/// Servicio de balance social — indicadores y reportes de gestión social
/// por asociado y organización
/// </summary>
public interface IBalanceSocialService
{
    /// <summary>Obtiene los indicadores de balance social de un asociado por año</summary>
    Task<IndicadorBalanceSocialDto?> GetIndicadorAsync(Guid asociadoId, int anio);

    /// <summary>Calcula y registra el indicador de balance social de un asociado</summary>
    Task<IndicadorBalanceSocialDto> CalcularIndicadorAsync(Guid asociadoId, Guid organizationId, int anio);

    /// <summary>Obtiene todos los indicadores de una organización en un año</summary>
    Task<List<IndicadorBalanceSocialDto>> GetIndicadoresByOrganizacionAsync(Guid organizationId, int anio);

    /// <summary>Obtiene los asociados que no cumplen con educación mínima (20hr)</summary>
    Task<List<IndicadorBalanceSocialDto>> GetNoCumplenEducacionAsync(Guid organizationId, int anio);
}
