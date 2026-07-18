# Capítulo 1: Stack Tecnológico del Backend

---

## 1.1 Introducción y Criterios de Selección

El presente capítulo documenta el conjunto de tecnologías seleccionadas para la construcción del sistema **DevManagerAPI**, una plataforma de gestión de talento tecnológico con capacidades de inteligencia artificial. La selección de cada componente del stack respondió a criterios técnicos objetivos, entre los cuales se destacan: madurez del ecosistema, soporte de largo plazo (Long-Term Support, LTS), integración nativa entre componentes, rendimiento demostrado en entornos de producción empresarial y disponibilidad de documentación oficial.

Se priorizaron tecnologías del ecosistema **Microsoft .NET**, dada su cohesión interna, el soporte corporativo de largo plazo y la homogeneidad que ofrece para el desarrollo de APIs RESTful con acceso a bases de datos relacionales. La decisión de mantener un stack homogéneo reduce la fricción de integración entre capas, simplifica la gestión de dependencias y facilita la implementación de patrones arquitectónicos como Clean Architecture.

Los criterios de selección aplicados a cada tecnología fueron los siguientes:

| Criterio | Descripción |
|---|---|
| **Soporte LTS** | Preferencia por versiones con soporte activo de larga duración |
| **Integración nativa** | Compatibilidad directa con los demás componentes del stack |
| **Madurez y estabilidad** | Historial de adopción en entornos empresariales reales |
| **Rendimiento** | Capacidad de gestionar carga concurrente con baja latencia |
| **Seguridad** | Mecanismos incorporados para autenticación, autorización y auditoría |
| **Extensibilidad** | Facilidad para incorporar nuevas funcionalidades sin refactorización estructural |

La arquitectura resultante soporta autenticación basada en tokens, acceso a datos mediante ORM, registro de eventos estructurado, integración con modelos de lenguaje de gran escala (LLM) y documentación automática de la API.

---

## 1.2 Plataforma de Ejecución: .NET 8.0 y ASP.NET Core

### Versión utilizada

**.NET 8.0** (versión LTS, publicada en noviembre de 2023). El framework de destino declarado en los archivos de proyecto es `net8.0`.

### Justificación de selección

Se seleccionó **.NET 8.0** como plataforma de ejecución por las siguientes razones técnicas:

1. **Soporte de larga duración**: .NET 8 es una versión LTS con soporte oficial hasta noviembre de 2026, lo que garantiza actualizaciones de seguridad y correcciones durante el ciclo de vida del proyecto.
2. **Rendimiento**: .NET 8 incorpora mejoras sustanciales en el compilador JIT (Just-In-Time) y en el modelo de concurrencia asíncrona (`async`/`await`), posicionándolo consistentemente entre los frameworks más rápidos en benchmarks independientes (TechEmpower).
3. **ASP.NET Core Web API**: El framework incluye de forma nativa el modelo de controladores basado en `ControllerBase`, con soporte integrado para serialización JSON, binding de modelos, validación de datos, manejo de excepciones y generación de respuestas tipadas.
4. **Ecosystem cohesion**: La plataforma integra de manera nativa Entity Framework Core, el sistema de inyección de dependencias, Kestrel como servidor HTTP de alto rendimiento y el pipeline de middleware configurable.

### Rol en el sistema

ASP.NET Core actúa como el motor de ejecución que expone los 13 controladores REST del sistema. El pipeline HTTP procesa las solicitudes entrantes, ejecuta el middleware de autenticación JWT, despacha las peticiones a los controladores correspondientes y serializa las respuestas mediante `System.Text.Json`. La configuración del servidor y el registro de servicios se centralizan en `Program.cs` mediante el patrón de extensiones (`IServiceCollection`).

El proyecto principal de la API declara las siguientes dependencias de plataforma integradas:

- `Microsoft.AspNetCore` — incluido en el SDK `Microsoft.NET.Sdk.Web`
- `Microsoft.Extensions.Hosting` — gestión del ciclo de vida de la aplicación (integrado en .NET 8)

---

## 1.3 Acceso a Datos: Entity Framework Core 8.0.11

### Versión utilizada

**Entity Framework Core 8.0.11** (`Microsoft.EntityFrameworkCore.SqlServer` versión `8.0.11`).

### Justificación de selección

Entity Framework Core (EF Core) fue seleccionado como Object-Relational Mapper (ORM) por los siguientes motivos:

1. **Alineación con el stack**: EF Core es el ORM oficial del ecosistema .NET, con integración de primera clase con ASP.NET Core y el sistema de inyección de dependencias.
2. **Migraciones gestionadas**: Permite gestionar la evolución del esquema de base de datos mediante migraciones versionadas en código C#, eliminando la dependencia de scripts SQL manuales.
3. **LINQ nativo**: Permite expresar consultas complejas mediante LINQ fuertemente tipado, con traducción automática a SQL optimizado en tiempo de ejecución.
4. **Configuración mediante Fluent API**: Soporta configuración declarativa de entidades, relaciones, restricciones e índices sin recurrir a atributos de datos en las clases de dominio, preservando la pureza de las entidades.
5. **Soporte multi-esquema**: Permite mapear entidades a tablas en esquemas SQL Server específicos (p. ej., `iam.Users`, `talent.EmployeeProfiles`), lo cual es esencial para la arquitectura de cinco esquemas del sistema.

### Rol en el sistema

EF Core actúa como la capa de traducción entre los objetos del dominio y las tablas de SQL Server. El `DevManagerDbContext` centraliza la configuración de los `DbSet<T>` correspondientes a todas las entidades del sistema. Las clases de configuración (`IEntityTypeConfiguration<T>`) se ubican en `Infrastructure/Data/Configurations/` y son descubiertas automáticamente mediante `modelBuilder.ApplyConfigurationsFromAssembly()`.

El patrón de acceso a datos del sistema implementa una separación explícita entre entidades EF Core (modelos de persistencia) y entidades de dominio (POCOs puros), con métodos de mapeo `MapToDomain()` y `MapToEntity()` en cada repositorio.

---

## 1.4 Sistema de Gestión de Base de Datos: Microsoft SQL Server 2019+

### Versión utilizada

**Microsoft SQL Server 2019** o superior. La cadena de conexión se configura en `appsettings.json` mediante la clave `ConnectionStrings:DefaultConnection`.

### Justificación de selección

SQL Server fue seleccionado como motor de base de datos relacional por las siguientes razones:

1. **Madurez empresarial**: SQL Server es uno de los motores de base de datos relacionales más utilizados en entornos corporativos, con historial demostrado de estabilidad y rendimiento en aplicaciones de alta concurrencia.
2. **Integración con EF Core**: El proveedor `Microsoft.EntityFrameworkCore.SqlServer` ofrece traducción fiel de consultas LINQ, soporte completo de migraciones y acceso a características específicas de SQL Server como esquemas nombrados, tipos de columna especializados (`varbinary`, `uniqueidentifier`) y funciones de ventana.
3. **Soporte de esquemas nombrados**: SQL Server permite organizar tablas en esquemas lógicos independientes (p. ej., `iam`, `talent`, `projects`, `config`, `reporting`), lo cual es fundamental para la estrategia de separación de dominios del sistema.
4. **Tipos de datos para seguridad**: Soporta `varbinary(N)` para el almacenamiento de hashes y salts criptográficos, y `uniqueidentifier` para claves primarias tipo GUID que eliminan la previsibilidad secuencial.

### Rol en el sistema

SQL Server almacena la totalidad del estado persistente del sistema: identidades de usuarios y organizaciones, perfiles de talento, proyectos, aplicaciones, asignaciones, reglas de recomendación e instantáneas de reportes. La base de datos se organiza en cinco esquemas lógicos:

| Esquema | Propósito |
|---|---|
| `config` | Catálogos de valores del sistema (estados, niveles, tipos) |
| `iam` | Identidad, autenticación y control de acceso |
| `talent` | Perfiles profesionales, habilidades y certificaciones |
| `projects` | Proyectos, roles, postulaciones y asignaciones |
| `reporting` | Instantáneas de datos, reglas y logs de recomendación |

El acceso directo a SQL Server también se instrumenta mediante `Microsoft.Data.SqlClient` (versión `6.1.3`) para operaciones que requieren control granular sobre la conexión subyacente.

---

## 1.5 Seguridad y Autenticación: JSON Web Tokens (JWT)

### Versión utilizada

**Microsoft.AspNetCore.Authentication.JwtBearer 8.0.11**. El algoritmo de firma es **HMACSHA512**.

### Justificación de selección

Se adoptó el estándar JSON Web Token (JWT, RFC 7519) para la autenticación y autorización del sistema por las siguientes razones:

1. **Stateless por diseño**: Los tokens JWT son autocontenidos y no requieren almacenamiento de sesión en el servidor, lo que simplifica la arquitectura y facilita la escalabilidad horizontal.
2. **Soporte multi-tenant nativo**: El token incluye el claim `OrganizationId`, que permite al sistema identificar la organización propietaria de cada solicitud sin consultas adicionales a la base de datos.
3. **Estándar de la industria**: JWT es el mecanismo de autenticación más ampliamente adoptado en APIs REST, con soporte en todos los frameworks y clientes HTTP relevantes.
4. **HMACSHA512**: Se seleccionó el algoritmo HMACSHA512 (en lugar de RS256 o HS256) por ofrecer mayor resistencia criptográfica mediante un hash de 512 bits, apropiado para sistemas con datos sensibles como perfiles profesionales y credenciales de acceso.

### Rol en el sistema

El sistema emite tokens JWT mediante el `TokenService` al completar el proceso de autenticación. Cada token incluye los siguientes claims:

| Claim | Tipo | Descripción |
|---|---|---|
| `nameid` | UserId (GUID) | Identificador único del usuario autenticado |
| `email` | string | Dirección de correo electrónico del usuario |
| `name` | string | Nombre completo del usuario |
| `OrganizationId` | GUID (string) | Identificador de la organización del usuario (clave multi-tenant) |
| `jti` | GUID (string) | Identificador único del token (JSON Token Identifier) |

Los tokens tienen una expiración de **8 horas** (configurable en `appsettings.json`). El middleware de validación se configura con `ClockSkew = TimeSpan.Zero` para garantizar la precisión exacta de la expiración. El claim `OrganizationId` es extraído en cada controlador mediante `ClaimsPrincipal` y propagado a las capas de servicio y repositorio para aplicar el aislamiento multi-tenant en todas las consultas.

---

## 1.6 Integración de Inteligencia Artificial: Google Gemini API

### Versión utilizada

**Google Gemini API** con el modelo **`gemini-1.5-flash`**. La integración se implementa mediante un cliente HTTP personalizado (`IGeminiService`) sin dependencia de SDK oficial de Google, utilizando `System.Net.Http.HttpClient` nativo de .NET 8.

### Justificación de selección

Se seleccionó el modelo `gemini-1.5-flash` de Google Gemini por las siguientes razones:

1. **Razonamiento multi-paso (Chain-of-Thought)**: El modelo soporta instrucciones de razonamiento paso a paso, lo que permite al sistema implementar el patrón Chain-of-Thought (CoT) para el análisis de competencias y la generación de recomendaciones estructuradas.
2. **Velocidad y eficiencia**: `gemini-1.5-flash` está optimizado para respuestas de baja latencia, adecuado para consultas en tiempo real desde el `AgentController`.
3. **Contexto extendido**: El modelo soporta ventanas de contexto extensas, lo que permite incluir datos estructurados del sistema (perfiles, proyectos, habilidades) como contexto de la consulta sin superar los límites del token.
4. **Integración via HTTP estándar**: La API de Google Gemini expone un endpoint REST que puede ser consumido mediante `HttpClient` estándar, eliminando la necesidad de dependencias de SDK de terceros en el proyecto.

### Rol en el sistema

El `GeminiService` actúa como cliente de inferencia de lenguaje natural. Recibe prompts construidos por el `AgentService` (que incluyen datos del contexto organizacional, perfiles de empleados y requerimientos de proyectos) y devuelve respuestas estructuradas con razonamiento y recomendaciones. El flujo de integración es el siguiente:

```
AgentService
    ├─ GatherContextData()   → recopila datos del sistema vía servicios de dominio
    ├─ BuildSystemPrompt()   → construye el prompt con contexto organizacional
    └─ BuildDataContext()    → serializa los datos relevantes como JSON
         │
         ▼
GeminiService.QueryWithReasoningAsync()
    ├─ Construye el prompt enriquecido con instrucción CoT
    ├─ HTTP POST → https://generativelanguage.googleapis.com (gemini-1.5-flash)
    └─ Parsea la respuesta: (Response: string, Reasoning: string)
```

Las acciones generadas por el agente se almacenan con estado `PENDING_APPROVAL` hasta que un usuario autorizado ejecute la aprobación mediante `POST /agent/approve/{id}` o el rechazo mediante `POST /agent/reject/{id}` (flujo Human-in-the-Loop, HITL).

---

## 1.7 Registro de Eventos: Serilog

### Versión utilizada

- **Serilog.AspNetCore 8.0.1**
- **Serilog.Sinks.File 5.0.0**

### Justificación de selección

Se adoptó **Serilog** como framework de registro de eventos en lugar del proveedor estándar `Microsoft.Extensions.Logging` por las siguientes razones:

1. **Logging estructurado**: Serilog emite los eventos de registro como objetos estructurados (no como cadenas de texto plano), lo que facilita su indexación, búsqueda y análisis en herramientas de observabilidad (p. ej., Seq, Elasticsearch, Azure Monitor).
2. **Sistema de sinks extensible**: Permite configurar múltiples destinos de salida (sinks) de forma simultánea: consola, archivo, bases de datos, servicios en la nube, sin cambios en el código de negocio.
3. **Integración con ASP.NET Core**: La librería `Serilog.AspNetCore` reemplaza el sistema de logging nativo de .NET preservando la interfaz `ILogger<T>` estándar, lo que garantiza compatibilidad con todas las librerías del ecosistema.
4. **Enriquecimiento contextual**: Permite adjuntar propiedades contextuales (p. ej., `RequestId`, `UserId`, `OrganizationId`) a todos los eventos de registro dentro de un scope, facilitando la correlación de trazas.

### Rol en el sistema

Serilog se configura en `Program.cs` mediante `UseSerilog()` y se inicializa con dos sinks:

- **Sink de consola**: Emite eventos de nivel `Information` y superior durante la ejecución, útil para depuración en desarrollo.
- **Sink de archivo**: Persiste eventos en archivos rotativos en el directorio `logs/`, con rotación diaria, útil para auditoría en producción.

Los controladores y servicios del sistema inyectan `ILogger<T>` mediante el sistema de DI estándar, sin acoplamiento directo a Serilog. Esto preserva la capacidad de sustituir el proveedor de logging en el futuro sin modificar la lógica de negocio.

---

## 1.8 Documentación de la API: Swagger / OpenAPI

### Versión utilizada

**Swashbuckle.AspNetCore 6.6.2**

### Justificación de selección

Se integró **Swagger/OpenAPI** mediante la librería Swashbuckle por las siguientes razones:

1. **Generación automática de especificación**: Swashbuckle genera automáticamente la especificación OpenAPI 3.0 a partir de los atributos de los controladores, comentarios XML de documentación y tipos de respuesta declarados, eliminando la necesidad de mantener documentación manual.
2. **Exploración interactiva**: La interfaz Swagger UI permite a los desarrolladores y evaluadores explorar y probar los endpoints directamente desde el navegador, con soporte para autenticación Bearer JWT.
3. **Estándar de la industria**: OpenAPI es el estándar de facto para la documentación de APIs REST, compatible con herramientas de generación de clientes, pruebas automáticas y validación de contratos.
4. **Soporte XML documentation**: El proyecto API está configurado con `GenerateDocumentationFile = true`, lo que permite incluir los comentarios de documentación XML de los controladores y DTOs en la especificación generada.

### Rol en el sistema

Swashbuckle se registra en el contenedor de DI mediante la extensión `AddSwaggerDocumentation()` definida en `API/Extensions/ApplicationServiceExtensions.cs`. Se configura con soporte para autenticación Bearer JWT y generación de documento XML. La especificación OpenAPI resultante documenta los 13 controladores del sistema con sus respectivos endpoints, parámetros, cuerpos de solicitud y tipos de respuesta.

En el entorno de desarrollo, la interfaz Swagger UI se expone en `/swagger`. En entornos de producción, la exposición de esta interfaz puede deshabilitarse mediante configuración.

---

## 1.9 Tabla de Dependencias NuGet

La siguiente tabla consolida todas las dependencias de paquetes NuGet utilizadas en el sistema, organizadas por proyecto y capa de la arquitectura Clean Architecture.

### Proyecto API (capa de presentación)

| Paquete | Versión | Propósito |
|---|---|---|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.11 | Middleware de validación de tokens JWT Bearer en el pipeline HTTP |
| `Microsoft.EntityFrameworkCore.Design` | 8.0.11 | Herramientas de diseño EF Core (migraciones) — solo en tiempo de desarrollo |
| `Serilog.AspNetCore` | 8.0.1 | Integración de Serilog con el pipeline de logging de ASP.NET Core |
| `Serilog.Sinks.File` | 5.0.0 | Sink de Serilog para escritura de logs en archivos rotativos |
| `Swashbuckle.AspNetCore` | 6.6.2 | Generación automática de especificación OpenAPI 3.0 e interfaz Swagger UI |

### Proyecto Infrastructure (capa de infraestructura)

| Paquete | Versión | Propósito |
|---|---|---|
| `Microsoft.Data.SqlClient` | 6.1.3 | Cliente ADO.NET de bajo nivel para SQL Server; conexiones directas cuando se requiere control granular |
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.0.11 | Proveedor EF Core para SQL Server; traducción de LINQ a T-SQL y gestión de migraciones |
| `Microsoft.EntityFrameworkCore.Tools` | 8.0.11 | Herramientas CLI de EF Core para generación y aplicación de migraciones — solo en tiempo de desarrollo |
| `Microsoft.Extensions.Configuration.Abstractions` | 10.0.1 | Abstracciones de configuración para acceso a `appsettings.json` en la capa de infraestructura |
| `Microsoft.Extensions.Hosting.Abstractions` | 8.0.1 | Abstracciones de `IHostedService` para los servicios en segundo plano (`ReportSnapshotGeneratorService`, `RecommendationOptimizerService`) |

### Dependencias integradas en el SDK (sin referencia explícita en `.csproj`)

| Componente | Origen | Propósito |
|---|---|---|
| `Microsoft.AspNetCore` | SDK `Microsoft.NET.Sdk.Web` | Framework HTTP, Kestrel, middleware pipeline, routing, model binding |
| `Microsoft.Extensions.Hosting` | SDK `Microsoft.NET.Sdk.Web` | Motor de arranque de la aplicación (`IHost`, `WebApplication`, ciclo de vida) |
| `System.Net.Http.HttpClient` | .NET 8 BCL | Cliente HTTP para la integración con la API de Google Gemini |
| `System.IdentityModel.Tokens.Jwt` | Transitivo de JwtBearer | Creación, firma y validación de tokens JWT con HMACSHA512 |

---

## 1.10 Síntesis del Capítulo

El presente capítulo documentó el conjunto tecnológico del backend de DevManagerAPI, justificando la selección de cada componente en función de criterios técnicos objetivos. A continuación se resume la correspondencia entre cada tecnología y el problema que resuelve en el sistema:

| Tecnología | Versión | Problema que resuelve |
|---|---|---|
| .NET 8.0 / ASP.NET Core | 8.0 LTS | Plataforma de ejecución de alto rendimiento con soporte LTS para APIs REST |
| Entity Framework Core | 8.0.11 | Abstracción del acceso a datos con soporte multi-esquema y migraciones versionadas |
| SQL Server 2019+ | 2019+ | Persistencia relacional con soporte de esquemas, GUIDs y tipos criptográficos |
| JWT / HMACSHA512 | JwtBearer 8.0.11 | Autenticación stateless con claim `OrganizationId` para aislamiento multi-tenant |
| Google Gemini API | gemini-1.5-flash | Razonamiento Chain-of-Thought para análisis de competencias y recomendaciones |
| Serilog | 8.0.1 / 5.0.0 | Logging estructurado con sinks configurables para observabilidad en producción |
| Swagger / Swashbuckle | 6.6.2 | Documentación automática OpenAPI 3.0 con exploración interactiva de endpoints |

La homogeneidad del stack seleccionado (ecosistema Microsoft .NET con integración de Google Gemini vía HTTP estándar) minimiza la complejidad de integración entre capas y garantiza la disponibilidad de soporte oficial y actualizaciones de seguridad durante el ciclo de vida proyectado del sistema.

Los capítulos siguientes profundizan en la arquitectura del sistema (Capítulo 2), el diseño de la base de datos (Capítulo 3) y la especificación de la API y la lógica de negocio (Capítulo 4).
