namespace Application.Interfaces;

using Application.DTOs.Excedentes;

/// <summary>
/// Servicio de distribución de excedentes según Ley 79 art. 54
/// 20% Reserva Protección Aportes, 20% Fondo Educación, 10% Fondo Solidaridad
/// </summary>
public interface IExcedenteService
{
    /// <summary>Calcula y registra la distribución de excedentes para un período</summary>
    Task<ExcedenteDto> CalcularDistribucionAsync(CreateExcedenteDto dto);

    /// <summary>Obtiene la distribución de excedentes por período</summary>
    Task<ExcedenteDto?> GetByPeriodoAsync(Guid organizationId, DateTime periodo);

    /// <summary>Obtiene todas las distribuciones de una organización</summary>
    Task<List<ExcedenteDto>> GetByOrganizacionAsync(Guid organizationId);

    /// <summary>Aprueba la distribución en Asamblea General</summary>
    Task<ExcedenteDto> AprobarDistribucionAsync(Guid excedenteId, decimal? revalorizacion, decimal? retornoCooperativo);
}
