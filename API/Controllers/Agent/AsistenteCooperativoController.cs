using Application.Common.Models;
using Application.DTOs.Agent;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Agent;

/// <summary>
/// Controlador del Asistente Cooperativo IA
/// Consultas normativas Supersolidaria, Balance Social, cumplimiento y atención al asociado
/// Basado en Ley 79/1988, Ley 454/1998, Circular Básica Jurídica 2020
/// </summary>
[ApiController]
[Route("api/asistente")]
[Authorize]
public class AsistenteCooperativoController : ControllerBase
{
    private readonly ICooperativaAIService _cooperativaService;
    private readonly ILogger<AsistenteCooperativoController> _logger;

    public AsistenteCooperativoController(
        ICooperativaAIService cooperativaService,
        ILogger<AsistenteCooperativoController> logger)
    {
        _cooperativaService = cooperativaService;
        _logger = logger;
    }

    /// <summary>
    /// Consulta normativa al Asistente Cooperativo
    /// </summary>
    /// <remarks>
    /// Realiza una consulta en lenguaje natural sobre normatividad cooperativa colombiana.
    /// El asistente responde con citaciones a normas específicas (Ley 79, Ley 454, CBJ, etc.)
    /// 
    /// **Ejemplos de consultas:**
    /// - "¿Cómo se distribuyen los excedentes?"
    /// - "¿Qué dice la Ley 79 sobre educación cooperativa?"
    /// - "Requisitos para ser miembro del Consejo de Administración"
    /// - "Normatividad aplicable al Balance Social"
    /// - "¿Qué son los aportes sociales?"
    /// 
    /// **Respuesta:**
    /// Incluye citaciones a artículos específicos con descripción y enlace a Supersolidaria.
    /// </remarks>
    /// <param name="request">Consulta en lenguaje natural</param>
    /// <response code="200">Respuesta con citaciones normativas</response>
    /// <response code="401">No autenticado</response>
    [HttpPost("consultar")]
    [ProducesResponseType(typeof(ApiResponse<CooperativaQueryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Consultar([FromBody] CooperativaQueryRequest request)
    {
        var organizationId = GetOrganizationId();

        _logger.LogInformation(
            "Consulta normativa cooperativa para org {OrgId}: {Consulta}",
            organizationId, request.Consulta);

        var response = await _cooperativaService.ConsultarNormatividadAsync(
            organizationId, request);

        return Ok(new ApiResponse<CooperativaQueryResponse>
        {
            Success = true,
            Message = "Consulta normativa procesada",
            Data = response
        });
    }

    /// <summary>
    /// Genera reporte de Balance Social
    /// </summary>
    /// <remarks>
    /// Compila indicadores de las 8 dimensiones del Balance Social según el marco
    /// de Cooperativas de las Américas y lineamientos de la Circular Básica Jurídica Título III Cap. X.
    /// 
    /// **Dimensiones evaluadas:**
    /// 1. Gobernanza Democrática - participación, composición, rotación
    /// 2. Satisfacción de Necesidades - calidad, quejas, cobertura
    /// 3. Compromiso con la Comunidad - inversión, ambiente, convenios
    /// 4. Educación e Información - cobertura, horas, fondo de educación
    /// 5. Ética y Transparencia - código, reportes, control interno
    /// 6. Integración Cooperativa - federaciones, alianzas, eventos
    /// 7. Desarrollo Económico - crecimiento, excedentes, reservas
    /// 8. Desarrollo Social y Humano - bienestar, programas, satisfacción
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     POST /api/asistente/generar-reporte
    ///     {
    ///         "organizationId": "guid-de-la-cooperativa",
    ///         "anio": 2026,
    ///         "incluirRecomendaciones": true
    ///     }
    /// </remarks>
    /// <param name="request">Parámetros del reporte (año, incluir recomendaciones)</param>
    /// <response code="200">Reporte de Balance Social generado</response>
    [HttpPost("generar-reporte")]
    [ProducesResponseType(typeof(ApiResponse<BalanceSocialReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerarReporte([FromBody] GenerarBalanceSocialRequest request)
    {
        var organizationId = GetOrganizationId();

        _logger.LogInformation(
            "Generando Balance Social {Year} para org {OrgId}",
            request.Anio, organizationId);

        var response = await _cooperativaService.GenerarBalanceSocialAsync(
            organizationId, request);

        return Ok(new ApiResponse<BalanceSocialReportDto>
        {
            Success = true,
            Message = "Balance Social generado exitosamente",
            Data = response
        });
    }

    /// <summary>
    /// Verifica cumplimiento normativo cooperativo
    /// </summary>
    /// <remarks>
    /// Evalúa el cumplimiento de la cooperativa en múltiples áreas normativas:
    /// - Educación cooperativa (Ley 79 art. 88-91)
    /// - Seguridad y Salud en el Trabajo (Decreto 1072/2015)
    /// - Protección de Datos / Habeas Data (Ley 1581/2012)
    /// - Aportes Sociales (Ley 79 art. 46-52)
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     POST /api/asistente/verificar-cumplimiento
    ///     {
    ///         "organizationId": "guid-de-la-cooperativa",
    ///         "verificarEducacion": true,
    ///         "verificarSST": true,
    ///         "verificarHabeasData": true,
    ///         "verificarAportes": true
    ///     }
    /// 
    /// **Respuesta:**
    /// Incluye cobertura por área, alertas de incumplimiento y hallazgos específicos.
    /// </remarks>
    /// <param name="request">Áreas a verificar</param>
    /// <response code="200">Resultado de verificación de cumplimiento</response>
    [HttpPost("verificar-cumplimiento")]
    [ProducesResponseType(typeof(ApiResponse<CumplimientoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerificarCumplimiento([FromBody] VerificarCumplimientoRequest request)
    {
        var organizationId = GetOrganizationId();

        _logger.LogInformation(
            "Verificando cumplimiento normativo para org {OrgId}",
            organizationId);

        var response = await _cooperativaService.VerificarCumplimientoAsync(
            organizationId, request);

        return Ok(new ApiResponse<CumplimientoDto>
        {
            Success = true,
            Message = $"Cumplimiento general: {response.CoberturaGeneral}%",
            Data = response
        });
    }

    /// <summary>
    /// Responde dudas de asociados sobre derechos, deberes y trámites
    /// </summary>
    /// <remarks>
    /// El asistente responde preguntas frecuentes de asociados sobre:
    /// - Afiliación y requisitos de ingreso
    /// - Derechos y deberes del asociado
    /// - Retiro voluntario y exclusión
    /// - Distribución de excedentes
    /// - Educación cooperativa obligatoria
    /// - Aportes sociales
    /// - Habeas Data y protección de datos
    /// - Voto y participación en Asamblea
    /// - Sanciones y debido proceso
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     POST /api/asistente/responder
    ///     {
    ///         "organizationId": "guid-de-la-cooperativa",
    ///         "pregunta": "¿Cómo me afilio a la cooperativa?",
    ///         "tipoAsociado": "nuevo"
    ///     }
    /// 
    /// **Respuesta:**
    /// Incluye respuesta clara, citación normativa y acciones sugeridas.
    /// </remarks>
    /// <param name="request">Pregunta del asociado</param>
    /// <response code="200">Respuesta a la duda del asociado</response>
    [HttpPost("responder")]
    [ProducesResponseType(typeof(ApiResponse<CooperativaQueryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Responder([FromBody] ResponderDudaRequest request)
    {
        var organizationId = GetOrganizationId();

        _logger.LogInformation(
            "Respondiendo duda de asociado para org {OrgId}: {Pregunta}",
            organizationId, request.Pregunta);

        var response = await _cooperativaService.ResponderDudaAsociadoAsync(
            organizationId, request);

        return Ok(new ApiResponse<CooperativaQueryResponse>
        {
            Success = true,
            Message = "Respuesta generada",
            Data = response
        });
    }

    private Guid GetOrganizationId()
    {
        var claim = User.FindFirst("OrganizationId")?.Value;
        return Guid.Parse(claim!);
    }
}
