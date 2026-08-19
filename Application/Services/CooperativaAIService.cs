using Application.DTOs.Agent;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// Implementación del Asistente Cooperativo IA
/// Consultas normativas, balance social, cumplimiento y atención al asociado
/// Implementación en memoria con respuestas template-based (extensible a Gemini)
/// </summary>
public class CooperativaAIService : ICooperativaAIService
{
    private readonly ILogger<CooperativaAIService> _logger;

    // Base de conocimiento normativo cooperativo
    private static readonly List<NormaEntry> Normatividad = new()
    {
        new("Ley 79", "Art. 5", "Capital mínimo irreducible no puede ser reducido por debajo del límite estatutario",
            "Ley 79 de 1988 - Marco general del cooperativismo"),
        new("Ley 79", "Art. 21-25", "Derechos y deberes de los asociados: uso de servicios, voto, educación",
            "Ley 79 de 1988 - De los asociados"),
        new("Ley 79", "Art. 26-45", "Órganos de administración: Asamblea General, Consejo, Junta de Vigilancia",
            "Ley 79 de 1988 - Organización y control"),
        new("Ley 79", "Art. 46-52", "Régimen económico: aportes sociales ordinarios y extraordinarios",
            "Ley 79 de 1988 - Aportes"),
        new("Ley 79", "Art. 54", "Distribución de excedentes: 20% reserva, 20% educación, 10% solidaridad",
            "Ley 79 de 1988 - Excedentes"),
        new("Ley 79", "Art. 88-91", "Educación cooperativa obligatoria. Mínimo 20 horas para fundadores",
            "Ley 79 de 1988 - Educación"),
        new("Ley 454", "Art. 1-3", "Objeto y ámbito de la Economía Solidaria. Creación de Supersolidaria",
            "Ley 454 de 1998 - Marco solidario"),
        new("Ley 454", "Art. 33-40", "Funciones de la Superintendencia de la Economía Solidaria",
            "Ley 454 de 1998 - Supersolidaria"),
        new("Circular Básica Jurídica", "Título I", "Normas de constitución, funcionamiento y control de cooperativas",
            "CBJ 2020 - Supersolidaria"),
        new("Circular Básica Jurídica", "Título II", "Régimen de autorización, vigilancia e inspección",
            "CBJ 2020 - Supersolidaria"),
        new("Circular Básica Jurídica", "Título III Cap. X", "Lineamientos para el Balance Social",
            "CBJ 2020 - Balance Social"),
        new("Decreto 2150", "2017", "Inspección, vigilancia y control. PILA tipo 51 para CTA",
            "Decreto 2150 de 2017"),
        new("Ley 1581", "2012", "Habeas Data: protección de datos personales. ARCO",
            "Ley 1581 de 2012"),
        new("Decreto 1377", "2013", "Reglamentación de Habeas Data: autorización, aviso de privacidad",
            "Decreto 1377 de 2013"),
        new("Decreto 1072", "2015", "SG-SST. Sistema de Gestión de Seguridad y Salud en el Trabajo",
            "Decreto 1072 de 2015")
    };

    // Plantillas de respuestas para dudas frecuentes de asociados
    private static readonly List<DudaPlantilla> DudasFrecuentes = new()
    {
        new("¿cómo me afilio", "derechos",
            "Para afiliarte como asociado debes cumplir los requisitos del art. 21 de la Ley 79/1988: " +
            "ser persona natural, acreditar idoneidad, recibir educación cooperativa básica (mín. 20 horas), " +
            "pagar el aporte social inicial y ser aceptado por el Consejo de Administración."),
        new("¿cuáles son mis derechos", "derechos",
            "Según el art. 23 de la Ley 79/1988, tus derechos como asociado incluyen: " +
            "1) Usar los servicios de la cooperativa, 2) Participar en la Asamblea con un voto, " +
            "3) Ser elegido para cargos sociales, 4) Recibir educación cooperativa, " +
            "5) Recibir excedentes proporcionalmente al uso de servicios, " +
            "6) Retirarte voluntariamente con reembolso de tus aportes."),
        new("¿cuáles son mis deberes", "deberes",
            "Según el art. 24 de la Ley 79/1988, tus deberes incluyen: " +
            "1) Cumplir las obligaciones estatutarias, 2) Asistir a la Asamblea, " +
            "3) Aceptar y cumplir las decisiones de los órganos, " +
            "4) Pagar oportunamente los aportes y obligaciones, " +
            "5) Recibir la educación cooperativa, 6) Desempeñar los cargos para los que seas elegido."),
        new("¿cómo me retiro", "tramites",
            "Para retirarte voluntariamente (art. 25 Ley 79/1988): " +
            "1) Presenta solicitud escrita al Consejo de Administración, " +
            "2) El Consejo debe pronunciarse en un plazo máximo de 30 días, " +
            "3) Si no hay objeciones, se aprueba el retiro, " +
            "4) Recibirás el reembolso de tus aportes después del plazo de amortización " +
            "según lo dispuesto por la Asamblea, deduciendo obligaciones pendientes."),
        new("excedentes", "beneficios",
            "Los excedentes se distribuyen según el art. 54 Ley 79/1988: " +
            "20% a Reserva de Protección de Aportes, 20% a Fondo de Educación, " +
            "10% a Fondo de Solidaridad. El resto se destina según decisión de la Asamblea " +
            "a revalorización de aportes y retorno cooperativo proporcional al USO DE SERVICIOS, " +
            "NO al capital aportado."),
        new("educación", "obligaciones",
            "La educación cooperativa es OBLIGATORIA (Ley 79 art. 88-91). " +
            "Todo asociado debe recibir formación cooperativa. El Fondo de Educación " +
            "(20% de excedentes) financia programas de: doctrina cooperativa, educación financiera, " +
            "liderazgo y gobierno cooperativo. La cooperativa debe reportar cobertura anual " +
            "a la Asamblea."),
        new("aporte social", "financiero",
            "El aporte social ordinario es la contribución periódica que hace el asociado " +
            "según el estatuto (Ley 79 art. 46-52). Es el capital de trabajo de la cooperativa. " +
            "No puede devolverse mientras el asociado esté activo, solo al retiro " +
            "después del período de amortización. Existen aportes extraordinarios " +
            "aprobados por la Asamblea."),
        new("habeas data", "privacidad",
            "Tus datos personales están protegidos por la Ley 1581/2012. " +
            "Tienes derecho a: ACCEDER a tus datos, RECTIFICAR información incorrecta, " +
            "CANCELAR datos cuando no sean necesarios, OPONERTE al tratamiento. " +
            "La cooperativa debe tener tu autorización expresa para procesar datos personales."),
        new("voto", "derechos",
            "El voto en las cooperativas es: un asociado = un voto (Ley 79 art. 23). " +
            "No importa el monto de tus aportes. Puedes votar en la Asamblea General " +
            "para elegir el Consejo de Administración y la Junta de Vigilancia, " +
            "aprobar reformas estatutarias, decidir distribución de excedentes, " +
            "y aprobar el Balance Social."),
        new("sanciones", "disciplinario",
            "Las sanciones a asociados deben estar previstas en el estatuto (Ley 79 art. 25). " +
            "Pueden ser: amonestación, suspensión temporal de derechos, multas, " +
            "o exclusión. El debido proceso debe garantizarse siempre: " +
            "comunicación de cargos, derecho de defensa, decisión motivada " +
            "y posibilidad de apelación ante la Asamblea.")
    };

    public CooperativaAIService(ILogger<CooperativaAIService> logger)
    {
        _logger = logger;
    }

    public Task<CooperativaQueryResponse> ConsultarNormatividadAsync(
        Guid organizationId,
        CooperativaQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Consultando normatividad para org {OrgId}: {Query}",
            organizationId, request.Consulta);

        var consultaLower = request.Consulta.ToLowerInvariant();
        var keywords = consultaLower.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Buscar normas relevantes por coincidencia de palabras clave
        var relevantes = Normatividad
            .Where(n =>
                n.Titulo.ToLowerInvariant().ContainsAny(keywords) ||
                n.Articulo.ToLowerInvariant().ContainsAny(keywords) ||
                n.Descripcion.ToLowerInvariant().ContainsAny(keywords))
            .ToList();

        // Si no hay coincidencias exactas, devolver las más generales
        if (!relevantes.Any())
        {
            relevantes = Normatividad
                .Where(n =>
                    consultaLower.Contains("excedente") && n.Articulo == "Art. 54" ||
                    consultaLower.Contains("educación") && n.Titulo == "Ley 79" ||
                    consultaLower.Contains("asociado") && n.Articulo == "Art. 21-25" ||
                    consultaLower.Contains("habeas") && n.Titulo == "Ley 1581" ||
                    consultaLower.Contains("sst") && n.Titulo == "Decreto 1072" ||
                    consultaLower.Contains("balance") && n.Articulo.Contains("Título III") ||
                    consultaLower.Contains("aporte") && n.Articulo == "Art. 46-52" ||
                    consultaLower.Contains("voto") && n.Articulo == "Art. 21-25" ||
                    consultaLower.Contains("órgano") && n.Articulo == "Art. 26-45" ||
                    consultaLower.Contains("asamblea") && n.Articulo == "Art. 26-45" ||
                    consultaLower.Contains("consejo") && n.Articulo == "Art. 26-45" ||
                    consultaLower.Contains("vigilancia") && n.Articulo == "Art. 26-45" ||
                    consultaLower.Contains("pil") && n.Titulo == "Decreto 2150")
                .ToList();
        }

        var citaciones = relevantes
            .Select(n => new CitacionNormativa
            {
                Norma = n.Titulo,
                Articulo = n.Articulo,
                Descripcion = n.Descripcion,
                UrlReferencia = $"https://www.supersolidaria.gov.co/normativa/{n.Titulo.ToLower().Replace(' ', '-')}"
            })
            .ToList();

        var respuesta = citaciones.Any()
            ? $"Según la normatividad cooperativa colombiana, encontré {citaciones.Count} referencias relevantes a tu consulta."
            : "No encontré normas específicas para tu consulta. Te sugiero contactar al Revisor Fiscal o consultar la Circular Básica Jurídica de Supersolidaria.";

        return Task.FromResult(new CooperativaQueryResponse
        {
            Respuesta = respuesta,
            Markdown = FormatNormasMarkdown(citaciones, request.Consulta),
            Citations = citaciones,
            AccionesSugeridas = new List<string>
            {
                "Consulte la Circular Básica Jurídica 2020 completa",
                "Verifique el Balance Social de su cooperativa",
                "Contacte a Supersolidaria para una pre-consulta formal"
            }
        });
    }

    public Task<BalanceSocialReportDto> GenerarBalanceSocialAsync(
        Guid organizationId,
        GenerarBalanceSocialRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generando Balance Social {Year} para org {OrgId}",
            request.Anio, organizationId);

        // Simular datos de indicadores para las 8 dimensiones del Balance Social
        var dimensiones = new List<DimensionSocialDto>
        {
            new()
            {
                Nombre = "Gobernanza Democrática",
                Descripcion = "Participación de asociados en órganos, composición del consejo, votación",
                Cobertura = 75,
                Meta = 85,
                Indicadores = new List<IndicadorSocialDto>
                {
                    new() { Nombre = "Participación en Asamblea", ValorActual = 65, ValorMeta = 80, Unidad = "%" },
                    new() { Nombre = "Rotación de consejeros", ValorActual = 4, ValorMeta = 6, Unidad = "años" },
                    new() { Nombre = "Mujeres en cargos directivos", ValorActual = 40, ValorMeta = 50, Unidad = "%" }
                }
            },
            new()
            {
                Nombre = "Satisfacción de Necesidades",
                Descripcion = "Calidad de servicios, quejas resueltas, cobertura",
                Cobertura = 82,
                Meta = 90,
                Indicadores = new List<IndicadorSocialDto>
                {
                    new() { Nombre = "Asociados satisfechos", ValorActual = 85, ValorMeta = 90, Unidad = "%" },
                    new() { Nombre = "Quejas resueltas en 30 días", ValorActual = 92, ValorMeta = 95, Unidad = "%" },
                    new() { Nombre = "Cobertura de servicios", ValorActual = 70, ValorMeta = 80, Unidad = "%" }
                }
            },
            new()
            {
                Nombre = "Compromiso con la Comunidad",
                Descripcion = "Inversión comunitaria, prácticas ambientales, integración",
                Cobertura = 60,
                Meta = 75,
                Indicadores = new List<IndicadorSocialDto>
                {
                    new() { Nombre = "Inversión comunitaria", ValorActual = 3, ValorMeta = 5, Unidad = "% de excedentes" },
                    new() { Nombre = "Programas ambientales", ValorActual = 2, ValorMeta = 4, Unidad = "programas/año" },
                    new() { Nombre = "Convenios intercooperativos", ValorActual = 3, ValorMeta = 5, Unidad = "convenios" }
                }
            },
            new()
            {
                Nombre = "Educación e Información",
                Descripcion = "Horas de formación, cobertura educativa, comunicación",
                Cobertura = 70,
                Meta = 85,
                Indicadores = new List<IndicadorSocialDto>
                {
                    new() { Nombre = "Cobertura educativa", ValorActual = 55, ValorMeta = 80, Unidad = "%" },
                    new() { Nombre = "Horas/promedio por asociado", ValorActual = 12, ValorMeta = 20, Unidad = "horas/año" },
                    new() { Nombre = "Uso del Fondo de Educación", ValorActual = 60, ValorMeta = 100, Unidad = "%" }
                }
            },
            new()
            {
                Nombre = "Ética y Transparencia",
                Descripcion = "Código de ética, reportes, control interno",
                Cobertura = 85,
                Meta = 90,
                Indicadores = new List<IndicadorSocialDto>
                {
                    new() { Nombre = "Código de ética implementado", ValorActual = 100, ValorMeta = 100, Unidad = "%" },
                    new() { Nombre = "Reportes trimestrales publicados", ValorActual = 4, ValorMeta = 4, Unidad = "reportes/año" },
                    new() { Nombre = "Hallazgos de control interno resueltos", ValorActual = 80, ValorMeta = 90, Unidad = "%" }
                }
            },
            new()
            {
                Nombre = "Integración Cooperativa",
                Descripcion = "Afiliación a federaciones, alianzas, cooperación interinstitucional",
                Cobertura = 55,
                Meta = 70,
                Indicadores = new List<IndicadorSocialDto>
                {
                    new() { Nombre = "Afiliación a federaciones", ValorActual = 1, ValorMeta = 2, Unidad = "federaciones" },
                    new() { Nombre = "Alianzas estratégicas", ValorActual = 2, ValorMeta = 4, Unidad = "alianzas" },
                    new() { Nombre = "Eventos cooperativos", ValorActual = 3, ValorMeta = 6, Unidad = "eventos/año" }
                }
            },
            new()
            {
                Nombre = "Desarrollo Económico",
                Descripcion = "Sostenibilidad financiera, distribución de excedentes, crecimiento",
                Cobertura = 78,
                Meta = 85,
                Indicadores = new List<IndicadorSocialDto>
                {
                    new() { Nombre = "Crecimiento de aportes", ValorActual = 8, ValorMeta = 10, Unidad = "% anual" },
                    new() { Nombre = "Excedentes distribuidos", ValorActual = 75, ValorMeta = 100, Unidad = "% de lo disponible" },
                    new() { Nombre = "Reserva de protección", ValorActual = 12, ValorMeta = 15, Unidad = "% de aportes" }
                }
            },
            new()
            {
                Nombre = "Desarrollo Social y Humano",
                Descripcion = "Bienestar de asociados, conciliación, seguridad",
                Cobertura = 72,
                Meta = 80,
                Indicadores = new List<IndicadorSocialDto>
                {
                    new() { Nombre = "Asociados con bienestar", ValorActual = 45, ValorMeta = 60, Unidad = "%" },
                    new() { Nombre = "Programas de bienestar", ValorActual = 3, ValorMeta = 5, Unidad = "programas" },
                    new() { Nombre = "Índice de satisfacción laboral", ValorActual = 70, ValorMeta = 80, Unidad = "%" }
                }
            }
        };

        var fortalezas = dimensiones
            .Where(d => d.Cobertura >= 80)
            .Select(d => d.Nombre)
            .ToList();

        var oportunidades = dimensiones
            .Where(d => d.Cobertura < 70)
            .Select(d => $"{d.Nombre} ({d.Cobertura}%)")
            .ToList();

        var coberturaPromedio = dimensiones.Average(d => d.Cobertura);

        var narrativa = $@"
## Balance Social {request.Anio}

### Resumen General
La cooperativa presenta un cumplimiento social del **{coberturaPromedio:F1}%** en las 8 dimensiones del Balance Social, según el marco de Cooperativas de las Américas y lineamientos de la Circular Básica Jurídica Título III Cap. X.

### Dimensiones Destacadas
- **Ética y Transparencia ({dimensiones[4].Cobertura}%)**: La cooperativa mantiene altos estándares de transparencia y control interno.
- **Satisfacción de Necesidades ({dimensiones[1].Cobertura}%)**: Los asociados reportan buena satisfacción con los servicios.

### Áreas de Mejora
{(oportunidades.Any() ? string.Join("\n", oportunidades.Select(o => $"- **{o}**: Requiere atención prioritaria.")) : "- Todas las dimensiones cumplen satisfactoriamente.")}

### Recomendaciones
{(request.IncluirRecomendaciones ? @"1. Fortalecer la formación cooperativa para alcanzar el 80% de cobertura educativa.
2. Incrementar la inversión comunitaria al 5% de excedentes.
3. Establecer más alianzas intercooperativas y federativas.
4. Implementar programas de bienestar para alcanzar al menos el 50% de asociados." : "Ninguna.")}";

        return Task.FromResult(new BalanceSocialReportDto
        {
            OrganizationId = organizationId,
            OrganizationName = $"Organización {organizationId.ToString()[..8]}",
            Anio = request.Anio,
            Dimensiones = dimensiones,
            ResumenEjecutivo = $"Cobertura social general: {coberturaPromedio:F1}%. {fortalezas.Count} fortalezas, {oportunidades.Count} oportunidades de mejora.",
            Fortalezas = fortalezas,
            OportunidadesMejora = oportunidades,
            Narrativa = request.IncluirRecomendaciones ? narrativa : null
        });
    }

    public Task<CumplimientoDto> VerificarCumplimientoAsync(
        Guid organizationId,
        VerificarCumplimientoRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Verificando cumplimiento para org {OrgId}", organizationId);

        var areas = new List<AreaCumplimientoDto>();
        var alertas = new List<string>();

        if (request.VerificarEducacion)
        {
            var educacionCumple = false;
            var educacionCobertura = 55m; // Simulado: 55% cobertura educativa
            if (educacionCobertura < 80)
            {
                alertas.Add("EDUCACIÓN: Cobertura educativa por debajo del 80%. La Ley 79 art. 88-91 exige educación cooperativa obligatoria.");
            }
            else
            {
                educacionCumple = true;
            }

            areas.Add(new AreaCumplimientoDto
            {
                Nombre = "Educación Cooperativa",
                NormaAplicable = "Ley 79 art. 88-91",
                Cumple = educacionCumple,
                Cobertura = educacionCobertura,
                Detalle = $"Cobertura educativa: {educacionCobertura}% de asociados capacitados (meta: 80%)",
                Hallazgos = educacionCobertura < 80
                    ? new List<string> { "Cobertura insuficiente", "Fondo de Educación subutilizado", "Faltan programas de formación continua" }
                    : new List<string> { "Cobertura adecuada", "Fondo de Educación utilizado correctamente" }
            });
        }

        if (request.VerificarSST)
        {
            var sstCumple = true;
            var sstCobertura = 85m;

            areas.Add(new AreaCumplimientoDto
            {
                Nombre = "Seguridad y Salud en el Trabajo (SG-SST)",
                NormaAplicable = "Decreto 1072/2015, Res. 0312/2019",
                Cumple = sstCumple,
                Cobertura = sstCobertura,
                Detalle = $"SG-SST implementado al {sstCobertura}%",
                Hallazgos = sstCobertura < 80
                    ? new List<string> { "SG-SST incompleto", "COPASST no conformado", "Faltan exámenes periódicos" }
                    : new List<string> { "SG-SST implementado", "COPASST activo", "Exámenes al día" }
            });
        }

        if (request.VerificarHabeasData)
        {
            var habeasCumple = true;
            var habeasCobertura = 90m;

            areas.Add(new AreaCumplimientoDto
            {
                Nombre = "Protección de Datos (Habeas Data)",
                NormaAplicable = "Ley 1581/2012, Decreto 1377/2013",
                Cumple = habeasCumple,
                Cobertura = habeasCobertura,
                Detalle = $"Cumplimiento Habeas Data: {habeasCobertura}%",
                Hallazgos = habeasCobertura < 70
                    ? new List<string> { "Falta autorización expresa de asociados", "Aviso de privacidad no actualizado" }
                    : new List<string> { "Autorizaciones vigentes", "ARCO funcional", "RNBD registrado" }
            });
        }

        if (request.VerificarAportes)
        {
            var aportesCumple = true;
            var aportesCobertura = 88m;

            areas.Add(new AreaCumplimientoDto
            {
                Nombre = "Aportes Sociales",
                NormaAplicable = "Ley 79 art. 46-52",
                Cumple = aportesCumple,
                Cobertura = aportesCobertura,
                Detalle = $"Gestión de aportes: {aportesCobertura}%",
                Hallazgos = aportesCobertura < 70
                    ? new List<string> { "Morosidad alta en aportes", "Capital mínimo no actualizado" }
                    : new List<string> { "Aportes al día", "Capital mínimo irreducible cumplido" }
            });
        }

        return Task.FromResult(new CumplimientoDto
        {
            OrganizationId = organizationId,
            OrganizationName = $"Organización {organizationId.ToString()[..8]}",
            Areas = areas,
            Alertas = alertas
        });
    }

    public Task<CooperativaQueryResponse> ResponderDudaAsociadoAsync(
        Guid organizationId,
        ResponderDudaRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Respondiendo duda de asociado para org {OrgId}: {Pregunta}",
            organizationId, request.Pregunta);

        var preguntaLower = request.Pregunta.ToLowerInvariant();

        // Buscar la plantilla más relevante
        var plantilla = DudasFrecuentes
            .FirstOrDefault(d => preguntaLower.Contains(d.Keyword))
            ?? DudasFrecuentes
                .OrderBy(d => LevenshteinSimilarity(preguntaLower, d.Keyword))
                .FirstOrDefault();

        if (plantilla == null)
        {
            return Task.FromResult(new CooperativaQueryResponse
            {
                Respuesta = "No encontré una respuesta específica para tu pregunta. " +
                           "Te recomiendo consultar directamente con el área de atención al asociado " +
                           "o con el Revisor Fiscal de tu cooperativa.",
                AccionesSugeridas = new List<string>
                {
                    "Consulte el estatuto de su cooperativa",
                    "Contacte al área de atención al asociado",
                    "Solicite una consulta formal al Revisor Fiscal"
                }
            });
        }

        var citacion = plantilla.Categoria switch
        {
            "derechos" => new CitacionNormativa
            {
                Norma = "Ley 79",
                Articulo = "Art. 21-25",
                Descripcion = "Derechos y deberes de los asociados",
                UrlReferencia = "https://www.supersolidaria.gov.co/normativa/ley-79"
            },
            "deberes" => new CitacionNormativa
            {
                Norma = "Ley 79",
                Articulo = "Art. 24",
                Descripcion = "Deberes de los asociados",
                UrlReferencia = "https://www.supersolidaria.gov.co/normativa/ley-79"
            },
            "tramites" => new CitacionNormativa
            {
                Norma = "Ley 79",
                Articulo = "Art. 25",
                Descripcion = "Retiro voluntario y exclusión de asociados",
                UrlReferencia = "https://www.supersolidaria.gov.co/normativa/ley-79"
            },
            "beneficios" => new CitacionNormativa
            {
                Norma = "Ley 79",
                Articulo = "Art. 54",
                Descripcion = "Distribución de excedentes",
                UrlReferencia = "https://www.supersolidaria.gov.co/normativa/ley-79"
            },
            "obligaciones" => new CitacionNormativa
            {
                Norma = "Ley 79",
                Articulo = "Art. 88-91",
                Descripcion = "Educación cooperativa obligatoria",
                UrlReferencia = "https://www.supersolidaria.gov.co/normativa/ley-79"
            },
            "financiero" => new CitacionNormativa
            {
                Norma = "Ley 79",
                Articulo = "Art. 46-52",
                Descripcion = "Régimen económico y aportes",
                UrlReferencia = "https://www.supersolidaria.gov.co/normativa/ley-79"
            },
            "privacidad" => new CitacionNormativa
            {
                Norma = "Ley 1581",
                Articulo = "2012",
                Descripcion = "Habeas Data - protección de datos personales",
                UrlReferencia = "https://www.supersolidaria.gov.co/normativa/ley-1581"
            },
            "disciplinario" => new CitacionNormativa
            {
                Norma = "Ley 79",
                Articulo = "Art. 25",
                Descripcion = "Régimen sancionatorio",
                UrlReferencia = "https://www.supersolidaria.gov.co/normativa/ley-79"
            },
            _ => new CitacionNormativa
            {
                Norma = "Ley 79",
                Articulo = "1988",
                Descripcion = "Marco general del cooperativismo colombiano"
            }
        };

        return Task.FromResult(new CooperativaQueryResponse
        {
            Respuesta = plantilla.Respuesta,
            Markdown = $"## Respuesta a tu consulta\n\n{plantilla.Respuesta}\n\n---\n*Basado en {citacion.Norma} {citacion.Articulo}*",
            Citations = new List<CitacionNormativa> { citacion },
            AccionesSugeridas = plantilla.Categoria switch
            {
                "derechos" => new List<string> { "Revise el estatuto de su cooperativa", "Consulte el Balance Social", "Participe en la próxima Asamblea" },
                "tramites" => new List<string> { "Solicite el formato de retiro en su cooperativa", "Verifique el estado de sus aportes", "Consulte los plazos de amortización" },
                "financiero" => new List<string> { "Revise su extracto de aportes", "Consulte sobre aportes extraordinarios", "Verifique el capital mínimo irreducible" },
                _ => new List<string> { "Consulte la Circular Básica Jurídica", "Contacte a su cooperativa", "Participe en programas de educación cooperativa" }
            }
        });
    }

    private static string FormatNormasMarkdown(List<CitacionNormativa> citaciones, string consulta)
    {
        if (!citaciones.Any())
        {
            return $"## Resultado de consulta\n\nNo encontré referencias normativas específicas para: _{consulta}_";
        }

        var md = $"## Resultado de consulta normativa\n\n" +
                 $"Para tu consulta sobre _{consulta}_, encontré las siguientes referencias:\n\n";

        foreach (var c in citaciones)
        {
            md += $"### {c.Norma} - {c.Articulo}\n";
            md += $"{c.Descripcion}\n\n";
        }

        md += "---\n*Fuente: Superintendencia de la Economía Solidaria (Supersolidaria)*";

        return md;
    }

    /// <summary>
    /// Calcula similitud Levenshtein básica como fallback de búsqueda
    /// </summary>
    private static int LevenshteinSimilarity(string a, string b)
    {
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
            return 0;

        var lenA = a.Length;
        var lenB = b.Length;
        var d = new int[lenA + 1, lenB + 1];

        for (var i = 0; i <= lenA; i++) d[i, 0] = i;
        for (var j = 0; j <= lenB; j++) d[0, j] = j;

        for (var i = 1; i <= lenA; i++)
        {
            for (var j = 1; j <= lenB; j++)
            {
                var cost = a[i - 1] == b[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[lenA, lenB];
    }
}

/// <summary>
/// Entry de conocimiento normativo cooperativo
/// </summary>
internal record NormaEntry(string Titulo, string Articulo, string Descripcion, string Fuente);

/// <summary>
/// Plantilla de respuesta para dudas frecuentes de asociados
/// </summary>
internal record DudaPlantilla(string Keyword, string Categoria, string Respuesta);

/// <summary>
/// Extensiones internas para búsqueda por palabras clave
/// </summary>
internal static class StringExtensions
{
    public static bool ContainsAny(this string text, string[] keywords)
    {
        return keywords.Any(k => text.Contains(k));
    }
}
