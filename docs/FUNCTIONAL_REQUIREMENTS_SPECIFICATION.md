Especificación Formal de Requerimientos Funcionales: Sistema DevManager

Este documento detalla la especificación formal de requerimientos para el sistema DevManager, una plataforma integral diseñada para la gestión estratégica del talento y la ejecución de proyectos. El análisis se fundamenta en una arquitectura de software robusta, alineada con los estándares académicos de las Unidades Tecnológicas de Santander (UTS) y orientada a resolver la fragmentación de la información profesional en el sector productivo de Bucaramanga.


--------------------------------------------------------------------------------


1. Resumen del Análisis de Sistemas y Modelado de Actores

La propuesta de DevManager surge como una respuesta técnica a la ineficiencia operativa generada por la gestión descentralizada del talento. La dependencia histórica de herramientas aisladas ha impedido que las organizaciones identifiquen con precisión las competencias de su capital humano, resultando en una asignación de personal subóptima. DevManager propone una arquitectura unificada que transforma el historial de participación en proyectos y las evaluaciones técnicas en un activo dinámico. Mediante un modelo de datos normalizado, el sistema garantiza que la información profesional no sea estática, sino una métrica evolutiva del crecimiento organizacional.

La deconstrucción del sistema identifica los siguientes actores y responsabilidades fundamentales:

* Administrador de Organización: Gestiona la configuración global de la entidad, garantiza la unicidad legal (NIT) y supervisa las políticas de acceso multi-tenant.
* Gerente de Proyecto: Estructura las necesidades técnicas, define niveles de complejidad (1-3) y evalúa la idoneidad de los colaboradores.
* Empleado / Colaborador: Autogestiona su perfil, registra niveles de competencia (1-5) y lidera su crecimiento mediante postulaciones proactivas.
* Agente Inteligente: Motor lógico que procesa reglas de recomendación, identifica brechas de habilidades (skill gaps) y audita la evolución del talento.

La integridad del sistema se apoya en entidades centrales como Organizations, Users, Projects, Skills y Applications. La interrelación de estos objetos bajo un esquema de aislamiento lógico mediante el identificador de organización (OrganizationId) asegura que, aunque múltiples empresas compartan la infraestructura, los datos permanezcan estrictamente confidenciales y segregados. Esta segmentación es la base sobre la cual se edifican los controles de acceso detallados a continuación.


--------------------------------------------------------------------------------


2. Módulo de Gestión de Identidad y Acceso (IAM)

En un entorno corporativo que procesa datos personales y trayectorias profesionales, la seguridad y el control de acceso basado en roles (RBAC) son críticos. Este módulo implementa la seguridad desde el diseño (security by design), asegurando el cumplimiento de la Ley 1581 de 2012 (Habeas Data) mediante mecanismos de cifrado y aislamiento estricto que impiden la fuga de información entre inquilinos.

* RF-001: Registro y administración de Organizaciones (Multi-tenancy): El sistema debe permitir al Administrador crear organizaciones validando la unicidad del NIT (UQ_Organizations_Nit) para evitar duplicidad legal en el sistema.
* RF-002: Gestión de Usuarios y Roles (RBAC): El sistema debe permitir al Administrador de Organización crear usuarios y asignar roles dentro de su entorno lógico.
* RF-003: Autenticación Segura: El sistema debe autenticar usuarios utilizando campos varbinary(512) para PasswordHash y varbinary(256) para PasswordSalt, prohibiendo explícitamente el almacenamiento de contraseñas en texto plano.
* RF-004: Activación y Desactivación de Cuentas (Soft delete): El sistema debe permitir el borrado lógico de registros configurando el campo IsDeleted = 1. El software debe filtrar automáticamente estos registros en todas las consultas de visualización.

Criterios de Aceptación: Integridad y Aislamiento IAM

ID	Criterio de Aceptación	Validación Técnica
CA-IAM-01	Aislamiento de Datos	El sistema debe rechazar cualquier solicitud de acceso a registros cuyo OrganizationId no coincida con el del usuario autenticado.
CA-IAM-02	Unicidad de Roles	No se permitirá la creación de dos roles con el mismo nombre dentro de la misma organización, respetando el índice UX_Roles_Org_Name.

Establecida la identidad y seguridad del usuario, el sistema habilita la construcción del inventario de capacidades técnicas en el módulo de talento.


--------------------------------------------------------------------------------


3. Módulo de Gestión de Talento y Perfiles Profesionales

El perfil dinámico del trabajador en DevManager trasciende el currículum tradicional, convirtiendo la información estática en un activo estratégico. Al centralizar habilidades y certificaciones, la organización puede realizar un mapeo de competencias en tiempo real, facilitando la toma de decisiones basada en la evidencia técnica y no en suposiciones sobre el potencial del personal.

* RF-005: Gestión de Bio y Perfil Profesional: El sistema debe permitir al Empleado mantener su EmployeeProfile con biografía, años de experiencia y portafolios digitales.
* RF-006: Administración de Catálogo de Habilidades: El sistema debe permitir definir habilidades categorizadas estrictamente como Hard, Soft o Language para fines de análisis comparativo.
* RF-007: Registro de Certificaciones con Evidencia: El sistema debe permitir al Empleado registrar logros académicos adjuntando una EvidenceUrl para validación documental.
* RF-008: Autogestión de Niveles de Habilidad: El sistema debe permitir al Empleado declarar un nivel de competencia en una escala de 1 (Básico) a 5 (Experto).
* RF-009: Validación de Competencias por Terceros: El sistema debe permitir a un Validador (Humano o Agente Inteligente) confirmar niveles de habilidad, registrando el ValidatedByUserId para garantizar la trazabilidad del dato.

La validación rigurosa de estas habilidades permite que la estructuración de proyectos cuente con un mercado interno de talento confiable.


--------------------------------------------------------------------------------


4. Módulo de Estructuración y Requerimientos de Proyectos

La definición técnica precisa de los proyectos es el pilar para una asignación de personal basada en datos. Al integrar niveles de complejidad y requisitos obligatorios, el sistema elimina la subjetividad en la conformación de equipos, asegurando que los desafíos técnicos del proyecto estén alineados con las capacidades reales de los colaboradores asignados.

* RF-010: Gestión del Ciclo de Vida del Proyecto: El sistema debe permitir la gestión de estados según el dominio técnico: 1-Draft, 2-Open, 3-InProgress, 4-Closed, 5-Cancelled (CK_Projects_Status).
* RF-011: Definición de Requerimientos de Habilidades: El sistema debe permitir al Gerente especificar habilidades mandatorias y opcionales con un nivel mínimo requerido (escala 1-5).
* RF-012: Configuración de Roles y Vacantes: El sistema debe permitir definir los ProjectRoles especificando la cantidad necesaria de personas (NeededCount >= 1) para cada función.
* RF-013: Cálculo de Impacto por Complejidad: El sistema debe capturar el ComplexityLevel (escala tinyint 1-3). Esta métrica es obligatoria, ya que actúa como multiplicador en el cálculo del DeltaLevel del empleado al finalizar el proyecto.

Una vez estructurado el proyecto y sus necesidades, se activa el flujo de postulación interna para los colaboradores disponibles.


--------------------------------------------------------------------------------


5. Ciclo de Vida de Postulación y Asignación de Talento

La proactividad del empleado es fundamental para el desarrollo de carrera. El sistema de postulaciones internas no solo democratiza el acceso a las oportunidades, sino que permite a los gerentes filtrar el interés genuino y la motivación, elementos cualitativos que complementan la idoneidad técnica.

* RF-014: Visualización de Oportunidades según Perfil: El sistema debe presentar al Empleado solo proyectos en estado "Open" (2) que tengan requerimientos de habilidades compatibles con su perfil.
* RF-015: Proceso de Postulación y Mensaje de Motivación: El sistema debe permitir al Empleado postularse registrando un texto de motivación en ProjectApplications.
* RF-016: Gestión de Revisiones y Retroalimentación: El sistema debe permitir al Gerente cambiar el estado de postulación (1-Applied, 2-Approved, 3-Rejected, 4-Withdrawn). En caso de rechazo, el campo ReviewNotes es obligatorio.
* RF-017: Asignación Directa y Gestión de Participación: El sistema debe permitir la asignación directa de personal, creando el registro correspondiente en ProjectAssignments vinculado a un ProjectRole.

Criterio de Aceptación de Flujo: No se permitirán nuevas postulaciones ni aprobaciones si el proyecto ha transicionado a estado 3 (InProgress), 4 (Closed) o 5 (Cancelled).

La finalización de estas asignaciones desencadena el proceso de captura de resultados y evaluación de competencias.


--------------------------------------------------------------------------------


6. Módulo de Evaluación de Desempeño y Evolución de Habilidades (Feedback Loop)

La captura de datos post-ejecución es vital para alimentar los procesos de Procesamiento de Lenguaje Natural (NLP). El sistema no solo registra el "qué" se hizo, sino el "cómo", transformando comentarios cualitativos en insumos para el crecimiento técnico y el análisis inteligente de patrones de desempeño.

* RF-018: Registro de Contribución y Feedback Cualitativo: Al cierre de la participación, el Gerente debe registrar un ContributionScore (1-5) y FeedbackComments (nvarchar(max)). La ausencia de límite en los comentarios garantiza datos suficientes para análisis semántico profundo.
* RF-019: Actualización de Niveles de Habilidad (DeltaLevel): El sistema debe registrar evaluaciones en SkillEvaluations permitiendo un diferencial DeltaLevel entre -5 y 5, basándose en el desempeño y la complejidad del proyecto.
* RF-020: Categorización del Origen de la Evaluación: El sistema debe clasificar cada actualización de habilidad según su Source: 1-Project, 2-Manual, 3-SystemRule, para mantener la auditoría sobre la evolución del colaborador.

Este historial verificable permite que el sistema pase de la simple visualización de datos a la generación de inteligencia de negocios.


--------------------------------------------------------------------------------


7. Módulo de Reportes Estratégicos y Agente de Recomendación Inteligente

El valor final de DevManager reside en la transición de reportes descriptivos a recomendaciones prescriptivas. Mediante el uso de un Agente Inteligente, la plataforma identifica brechas de conocimiento y sugiere acciones correctivas, alineando el capital humano con los objetivos de competitividad regional de Bucaramanga y las líneas de investigación de las UTS.

* RF-021: Generación de Instantáneas de Datos (Snapshots): El sistema debe persistir el estado del talento en ReportSnapshots (formato JSON) para permitir análisis histórico de tendencias y comparativas temporales.
* RF-022: Motor de Reglas de Recomendación: El sistema debe proveer un motor que evalúe ConditionExpr (expresiones lógicas) sobre los datos de los perfiles y proyectos para disparar sugerencias automáticas.
* RF-023: Interfaz del Agente para Skill Gaps: El sistema debe presentar al Gerente recomendaciones de capacitación o rotación basadas en el análisis de brechas de habilidades detectadas por el motor de reglas.
* RF-024: Auditoría de Recomendaciones (Logs): Todas las acciones del agente deben registrarse en RecommendationLogs, capturando el ResultJson para la mejora continua del algoritmo.

La integración sistemática de estos requerimientos funcionales garantiza que DevManager cumpla su objetivo de optimizar el capital humano, impulsando la productividad empresarial y el desarrollo profesional sostenible a través de una gestión basada rigurosamente en datos.