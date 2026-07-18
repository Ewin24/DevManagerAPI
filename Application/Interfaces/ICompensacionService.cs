namespace Application.Interfaces;

using Application.DTOs.Nomina;

/// <summary>
/// Servicio de compensación para asociados del sector solidario
/// </summary>
public interface ICompensacionService
{
    /// <summary>Crea un nuevo registro de compensación</summary>
    Task<CompensacionDto> CreateAsync(CreateCompensacionDto dto);

    /// <summary>Calcula la compensación para un asociado en un período</summary>
    Task<decimal> CalcularAsync(Guid asociadoId, int mes, int anio);

    /// <summary>Obtiene las compensaciones de un asociado en un año</summary>
    Task<List<CompensacionDto>> GetByAsociadoAsync(Guid asociadoId, int anio);
}
