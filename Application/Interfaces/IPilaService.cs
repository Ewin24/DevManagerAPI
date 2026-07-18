namespace Application.Interfaces;

using Application.DTOs.Nomina;

/// <summary>
/// Servicio de PILA (Planilla Integrada de Liquidación de Aportes)
/// para asociados tipo CTA — Decreto 2150/2017
/// </summary>
public interface IPilaService
{
    /// <summary>Calcula los aportes PILA para un asociado según sus ingresos y nivel de riesgo ARL</summary>
    Task<PilaAporteDto> CalcularAportesAsync(Guid asociadoId, decimal ingresos, int nivelRiesgoARL);

    /// <summary>Genera la planilla completa de aportes para una organización en un período</summary>
    Task<List<PilaAporteDto>> GenerarPlanillaAsync(int mes, int anio, Guid organizationId);
}
