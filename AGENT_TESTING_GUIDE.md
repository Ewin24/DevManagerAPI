# 🧪 Guía Completa de Pruebas del Agente IA

**Fecha:** 20 de Enero de 2026  
**Versión:** v2.0.1

---

## 📋 Tabla de Contenidos

1. [Configuración Inicial](#configuración-inicial)
2. [Datos de Prueba](#datos-de-prueba)
3. [Pruebas del Agente](#pruebas-del-agente)
4. [Validación de Resultados](#validación-de-resultados)
5. [Troubleshooting](#troubleshooting)

---

## 🔧 Configuración Inicial

### 1. Obtener API Key de Google AI

1. Visita: https://aistudio.google.com/app/apikey
2. Click en **"Create API key"**
3. Selecciona un proyecto de Google Cloud (o crea uno nuevo)
4. Copia la API Key generada (formato: `AIza...`)

⚠️ **IMPORTANTE:** No commitear la API Key al repositorio

### 2. Configurar la API Key

**Opción A: appsettings.Development.json** (Desarrollo local)
```json
{
  "GoogleAI": {
    "ApiKey": "TU_API_KEY_AQUI",
    "Model": "gemini-1.5-flash",
    "Temperature": 0.7,
    "MaxTokens": 8192
  }
}
```

**Opción B: Variables de Entorno** (Producción)
```bash
# Windows PowerShell
$env:GoogleAI__ApiKey="TU_API_KEY_AQUI"

# Linux/Mac
export GoogleAI__ApiKey="TU_API_KEY_AQUI"
```

### 3. Crear Tablas del Agente

```bash
# Ejecutar DDL para crear tablas reporting.AgentActions y reporting.AgentConfiguration
sqlcmd -S localhost -d DevManager -E -i Infrastructure/Database/DDL/DDL_Agent_Tables.sql
```

**Verificar creación:**
```sql
USE DevManager;
GO

SELECT TABLE_SCHEMA, TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'reporting'
AND TABLE_NAME LIKE 'Agent%';
-- Debe retornar: AgentActions, AgentConfiguration
```

### 4. Iniciar el API

```bash
cd API
dotnet run

# O con hot reload
dotnet watch run
```

**Verificar que está corriendo:**
- Swagger: https://localhost:7265/swagger
- HTTP: http://localhost:5073

---

## 📊 Datos de Prueba

### Script SQL para Crear Datos de Prueba

Ejecuta este script para crear una organización de prueba con usuarios, skills y un proyecto:

```sql
USE DevManager;
GO

-- 1. CREAR ORGANIZACIÓN DE PRUEBA
DECLARE @OrgId uniqueidentifier = NEWID();
DECLARE @UserId1 uniqueidentifier = NEWID();
DECLARE @UserId2 uniqueidentifier = NEWID();
DECLARE @SkillJava uniqueidentifier = NEWID();
DECLARE @SkillPython uniqueidentifier = NEWID();
DECLARE @SkillReact uniqueidentifier = NEWID();
DECLARE @ProjectId uniqueidentifier = NEWID();

-- Organización
INSERT INTO iam.Organizations (Id, Name, LegalName, Nit, IsActive, CreatedAt, UpdatedAt, IsDeleted)
VALUES (@OrgId, 'TechCorp Demo', 'TechCorp SAS', '900123456-7', 1, SYSUTCDATETIME(), SYSUTCDATETIME(), 0);

-- Usuarios
INSERT INTO iam.Users (Id, OrganizationId, FullName, Email, PasswordHash, PasswordSalt, IsActive, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
    (@UserId1, @OrgId, 'Juan Pérez', 'juan.perez@techcorp.com', 
     0x5E884898DA28047151D0E56F8DC6292773603D0D6AABBDD62A11EF721D1542D8, -- Hash de "Password123!"
     0x6D73616C74, 
     1, SYSUTCDATETIME(), SYSUTCDATETIME(), 0),
    (@UserId2, @OrgId, 'María García', 'maria.garcia@techcorp.com', 
     0x5E884898DA28047151D0E56F8DC6292773603D0D6AABBDD62A11EF721D1542D8,
     0x6D73616C74,
     1, SYSUTCDATETIME(), SYSUTCDATETIME(), 0);

-- Perfiles de empleados
INSERT INTO talent.EmployeeProfiles (UserId, OrganizationId, Bio, YearsExperience, LinkedInUrl, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
    (@UserId1, @OrgId, 'Desarrollador Full Stack con experiencia en microservicios', 5, 'https://linkedin.com/in/juanperez', SYSUTCDATETIME(), SYSUTCDATETIME(), 0),
    (@UserId2, @OrgId, 'Desarrolladora Backend especializada en Python y IA', 3, 'https://linkedin.com/in/mariagarcia', SYSUTCDATETIME(), SYSUTCDATETIME(), 0);

-- Skills
INSERT INTO talent.Skills (Id, Name, Category, SkillType, OrganizationId, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
    (@SkillJava, 'Java', 'Backend', 1, @OrgId, SYSUTCDATETIME(), SYSUTCDATETIME(), 0),
    (@SkillPython, 'Python', 'Backend', 1, @OrgId, SYSUTCDATETIME(), SYSUTCDATETIME(), 0),
    (@SkillReact, 'React', 'Frontend', 1, @OrgId, SYSUTCDATETIME(), SYSUTCDATETIME(), 0);

-- Habilidades de empleados
INSERT INTO talent.EmployeeSkills (Id, UserId, SkillId, OrganizationId, [Level], EvidenceUrl, LastValidatedAt, ValidatedByUserId, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
    (NEWID(), @UserId1, @SkillJava, @OrgId, 4, 'https://github.com/juanperez/java-projects', SYSUTCDATETIME(), NULL, SYSUTCDATETIME(), SYSUTCDATETIME(), 0),
    (NEWID(), @UserId1, @SkillReact, @OrgId, 3, 'https://github.com/juanperez/react-apps', NULL, NULL, SYSUTCDATETIME(), SYSUTCDATETIME(), 0),
    (NEWID(), @UserId2, @SkillPython, @OrgId, 5, 'https://github.com/mariagarcia/ml-projects', SYSUTCDATETIME(), NULL, SYSUTCDATETIME(), SYSUTCDATETIME(), 0),
    (NEWID(), @UserId2, @SkillReact, @OrgId, 2, NULL, NULL, NULL, SYSUTCDATETIME(), SYSUTCDATETIME(), 0);

-- Proyecto
INSERT INTO projects.Projects (Id, Code, Name, Description, StartDate, EndDate, ComplexityLevel, [Status], OrganizationId, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
    (@ProjectId, 'PROJ-001', 'Sistema de E-Commerce', 
     'Plataforma de comercio electrónico con pagos integrados y gestión de inventario',
     '2026-02-01', '2026-08-31', 2, 1, -- ComplexityLevel: 2=Medium, Status: 1=Open
     @OrgId, SYSUTCDATETIME(), SYSUTCDATETIME(), 0);

-- Requisitos de skills del proyecto
INSERT INTO projects.ProjectSkillRequirements (Id, ProjectId, SkillId, RequiredLevel, IsMandatory, OrganizationId, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
    (NEWID(), @ProjectId, @SkillJava, 4, 1, @OrgId, SYSUTCDATETIME(), SYSUTCDATETIME(), 0),
    (NEWID(), @ProjectId, @SkillReact, 3, 1, @OrgId, SYSUTCDATETIME(), SYSUTCDATETIME(), 0),
    (NEWID(), @ProjectId, @SkillPython, 3, 0, @OrgId, SYSUTCDATETIME(), SYSUTCDATETIME(), 0);

-- Mostrar IDs generados
SELECT 
    'OrganizationId' AS Tipo, CAST(@OrgId AS varchar(50)) AS Valor
UNION ALL SELECT 'UserId1', CAST(@UserId1 AS varchar(50))
UNION ALL SELECT 'UserId2', CAST(@UserId2 AS varchar(50))
UNION ALL SELECT 'SkillJava', CAST(@SkillJava AS varchar(50))
UNION ALL SELECT 'SkillPython', CAST(@SkillPython AS varchar(50))
UNION ALL SELECT 'SkillReact', CAST(@SkillReact AS varchar(50))
UNION ALL SELECT 'ProjectId', CAST(@ProjectId AS varchar(50));

PRINT 'Datos de prueba creados exitosamente!';
```

**Guarda los IDs generados** - los necesitarás para las pruebas.

---

## 🧪 Pruebas del Agente

### Paso 1: Obtener Token JWT

```powershell
# PowerShell
$loginResponse = Invoke-RestMethod -Uri "http://localhost:5073/auth/login" `
    -Method POST `
    -ContentType "application/json" `
    -Body (@{
        email = "juan.perez@techcorp.com"
        password = "Password123!"
    } | ConvertTo-Json)

$token = $loginResponse.data.token
Write-Host "Token obtenido: $token"
```

```bash
# Bash/Linux
TOKEN=$(curl -s -X POST http://localhost:5073/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"juan.perez@techcorp.com","password":"Password123!"}' \
  | jq -r '.data.token')

echo "Token: $TOKEN"
```

---

### Prueba 1: Consulta en Lenguaje Natural

**Objetivo:** Verificar que el agente responde a preguntas en lenguaje natural.

```bash
# Bash
curl -X POST http://localhost:5073/agent/query \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "query": "¿Cuántos desarrolladores tenemos con experiencia en Java?"
  }' | jq '.data'
```

**Resultado esperado:**
```json
{
  "response": "Hay 1 desarrollador con experiencia en Java: Juan Pérez, con nivel 4/5.",
  "reasoning": "1. Busqué en la base de datos de skills...\n2. Encontré 1 empleado...\n3. Juan tiene nivel 4 en Java...",
  "confidence": 95
}
```

---

### Prueba 2: Validación Semántica de Skill

**Objetivo:** Verificar que el agente analiza la evidencia y valida una habilidad.

```powershell
# PowerShell
# Usa el SkillJava ID de los datos de prueba
$validateResponse = Invoke-RestMethod `
    -Uri "http://localhost:5073/agent/validate-skill" `
    -Method POST `
    -Headers @{ "Authorization" = "Bearer $token" } `
    -ContentType "application/json" `
    -Body (@{
        userId = "TU_UserId1_AQUI"  # Reemplazar con el UserId1 real
        skillId = "TU_SkillJava_AQUI"  # Reemplazar con el SkillJava ID real
        evidenceUrl = "https://github.com/juanperez/java-projects"
        claimedLevel = 4
    } | ConvertTo-Json)

Write-Host "Validación:"
Write-Host "¿Aprobado? $($validateResponse.data.isValid)"
Write-Host "Confianza: $($validateResponse.data.confidence)%"
Write-Host "Recomendación: $($validateResponse.data.recommendation)"
```

```bash
# Bash
curl -X POST http://localhost:5073/agent/validate-skill \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "TU_UserId1_AQUI",
    "skillId": "TU_SkillJava_AQUI",
    "evidenceUrl": "https://github.com/juanperez/java-projects",
    "claimedLevel": 4
  }' | jq '.data'
```

**Resultado esperado:**
```json
{
  "isValid": true,
  "confidence": 85,
  "suggestedLevel": 4,
  "recommendation": "La evidencia proporcionada (GitHub con proyectos Java) respalda el nivel 4. Se recomienda aprobar.",
  "reasoning": "El usuario tiene 5 años de experiencia y proporciona evidencia en GitHub..."
}
```

---

### Prueba 3: Matching Inteligente de Candidatos

**Objetivo:** Verificar que el agente encuentra los mejores candidatos para un proyecto.

```powershell
# PowerShell
$matchResponse = Invoke-RestMethod `
    -Uri "http://localhost:5073/agent/match-candidates" `
    -Method POST `
    -Headers @{ "Authorization" = "Bearer $token" } `
    -ContentType "application/json" `
    -Body (@{
        projectId = "TU_ProjectId_AQUI"  # Reemplazar con el ProjectId real
    } | ConvertTo-Json)

Write-Host "Proyecto: $($matchResponse.data.projectName)"
Write-Host "`nCandidatos encontrados: $($matchResponse.data.candidates.Count)"
foreach ($candidate in $matchResponse.data.candidates) {
    Write-Host "`n--- $($candidate.fullName) ---"
    Write-Host "Email: $($candidate.email)"
    Write-Host "Match Score: $($candidate.matchScore)%"
    Write-Host "Razón: $($candidate.recommendationReason)"
}
```

```bash
# Bash
curl -X POST http://localhost:5073/agent/match-candidates \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "projectId": "TU_ProjectId_AQUI"
  }' | jq '.data'
```

**Resultado esperado:**
```json
{
  "projectId": "...",
  "projectName": "Sistema de E-Commerce",
  "candidates": [
    {
      "userId": "...",
      "fullName": "Juan Pérez",
      "email": "juan.perez@techcorp.com",
      "matchScore": 95,
      "skillAlignments": [
        {
          "skillName": "Java",
          "requiredLevel": 4,
          "currentLevel": 4,
          "isMandatory": true,
          "meets": true
        },
        {
          "skillName": "React",
          "requiredLevel": 3,
          "currentLevel": 3,
          "isMandatory": true,
          "meets": true
        }
      ],
      "recommendationReason": "Candidato ideal. Cumple con todos los requisitos obligatorios y tiene 5 años de experiencia."
    }
  ],
  "analysisNarrative": "Se encontró 1 candidato que cumple con todos los requisitos obligatorios..."
}
```

---

### Prueba 4: Flujo HITL (Human-in-the-Loop)

**Objetivo:** Verificar el sistema de aprobación/rechazo manual de acciones del agente.

> **⚠️ NOTA IMPORTANTE:** En la versión actual (v2.0.2), el flujo HITL funciona como **auditoría post-facto**. Cuando apruebas una acción, el sistema actualiza el estado en la BD pero **NO ejecuta acciones automáticas** (como UPDATE de skills o asignación a proyectos). El HITL actual es útil para:
> - Registrar que un humano revisó y aprobó una respuesta
> - Mantener trazabilidad de quién aprobó qué y cuándo
> - Análisis de confianza en las recomendaciones del agente
>
> Para ejecución diferida de acciones (aprobar → ejecutar cambios en BD), ver [HITL_WORKFLOW_GUIDE.md](HITL_WORKFLOW_GUIDE.md).

#### 4.1. Obtener ID de Acción

Después de ejecutar la Prueba 2 (validación de skill), revisa la tabla de auditoría:

```sql
USE DevManager;
GO

SELECT TOP 1 
    Id AS ActionId,
    ActionType,
    Description,
    Status,
    CreatedAt
FROM reporting.AgentActions
ORDER BY CreatedAt DESC;
```

Copia el `ActionId`.

#### 4.2. Aprobar Acción

```powershell
# PowerShell
$approveResponse = Invoke-RestMethod `
    -Uri "http://localhost:5073/agent/approve/TU_ActionId_AQUI" `
    -Method POST `
    -Headers @{ "Authorization" = "Bearer $token" }

Write-Host "Acción aprobada:"
Write-Host $approveResponse.message
```

```bash
# Bash
curl -X POST http://localhost:5073/agent/approve/TU_ActionId_AQUI \
  -H "Authorization: Bearer $TOKEN" | jq '.'
```

#### 4.3. Verificar Estado

```sql
SELECT 
    Id,
    Status,  -- Debe ser 'APPROVED'
    ApprovedByUserId,  -- Debe tener valor
    ApprovedAt  -- Debe tener timestamp
FROM reporting.AgentActions
WHERE Id = 'TU_ActionId_AQUI';
```

---

## ✅ Validación de Resultados

### Verificar Auditoría

```sql
USE DevManager;
GO

-- Ver todas las acciones del agente
SELECT 
    ActionType,
    [Status],
    COUNT(*) AS Total
FROM reporting.AgentActions
WHERE IsDeleted = 0
GROUP BY ActionType, [Status];

-- Ver detalle de acciones recientes
SELECT TOP 10
    ActionType,
    Description,
    [Status],
    CreatedAt,
    ApprovedAt
FROM reporting.AgentActions
WHERE OrganizationId = 'TU_OrgId_AQUI'
ORDER BY CreatedAt DESC;
```

### Verificar Logs de la Aplicación

```powershell
# Ver logs en tiempo real
dotnet run --project API/API.csproj | Select-String -Pattern "Agent|Gemini"
```

Buscar líneas como:
```
info: Application.Services.AgentService[0]
      Validando skill ... para usuario ...
info: Infrastructure.Services.AI.GeminiService[0]
      Gemini API llamado - Tokens usados: 450
```

---

## 🔧 Troubleshooting

### Error: "API Key no configurada"

**Síntoma:**
```
System.ArgumentNullException: GoogleAI:ApiKey no encontrado
```

**Solución:**
1. Verifica que `appsettings.Development.json` tenga la API Key
2. O configura la variable de entorno: `$env:GoogleAI__ApiKey="..."`

---

### Error: "Invalid API Key"

**Síntoma:**
```
HTTP 401 Unauthorized from ai.google.dev
```

**Solución:**
1. Verifica que la API Key sea válida
2. Ve a https://aistudio.google.com/app/apikey y regenera la key
3. Asegúrate de que el proyecto de Google Cloud tenga Gemini API habilitada

---

### Error: "AgentActions table not found"

**Síntoma:**
```
System.Data.SqlClient.SqlException: Invalid object name 'reporting.AgentActions'
```

**Solución:**
```bash
sqlcmd -S localhost -d DevManager -E -i Infrastructure/Database/DDL/DDL_Agent_Tables.sql
```

---

### El Agente No Encuentra Candidatos

**Síntoma:**
```json
{
  "candidates": [],
  "analysisNarrative": "No se encontraron candidatos..."
}
```

**Solución:**
1. Verifica que existan perfiles con skills:
```sql
SELECT 
    u.FullName,
    s.Name AS SkillName,
    es.[Level]
FROM iam.Users u
JOIN talent.EmployeeSkills es ON u.Id = es.UserId
JOIN talent.Skills s ON es.SkillId = s.Id
WHERE u.OrganizationId = 'TU_OrgId_AQUI'
AND u.IsDeleted = 0;
```

2. Si no hay datos, ejecuta el script de [Datos de Prueba](#datos-de-prueba)

---

### Gemini API Lenta o Timeout

**Síntoma:**
```
TaskCanceledException: The request was canceled due to the configured HttpClient.Timeout of 100 seconds elapsing.
```

**Solución:**
1. Verifica tu conexión a Internet
2. Reduce la complejidad de la consulta
3. Aumenta el timeout en `GeminiService.cs`:
```csharp
_httpClient.Timeout = TimeSpan.FromSeconds(180);
```

---

## 📊 Métricas de Éxito

### Checklist de Validación

- [ ] **Configuración**
  - [ ] API Key configurada
  - [ ] Tablas del agente creadas
  - [ ] API iniciada correctamente
  - [ ] Token JWT obtenido

- [ ] **Prueba 1: Consultas NL**
  - [ ] El agente responde en lenguaje natural
  - [ ] El razonamiento (Chain of Thought) es coherente
  - [ ] Confidence score > 70%

- [ ] **Prueba 2: Validación de Skills**
  - [ ] El agente analiza la evidencia
  - [ ] Sugiere un nivel basado en experiencia
  - [ ] Crea registro en AgentActions

- [ ] **Prueba 3: Matching**
  - [ ] Encuentra candidatos con las skills requeridas
  - [ ] Calcula match scores correctamente
  - [ ] Ordena por puntuación de mayor a menor

- [ ] **Prueba 4: HITL**
  - [ ] Las acciones se registran como PENDING_APPROVAL
  - [ ] Se pueden aprobar manualmente
  - [ ] El estado se actualiza correctamente

- [ ] **Auditoría**
  - [ ] Todas las acciones se registran en AgentActions
  - [ ] Los logs muestran las llamadas a Gemini
  - [ ] Los tokens consumidos se registran

---

## 🎯 Próximos Pasos

Una vez que todas las pruebas pasen:

1. **Agregar más datos de prueba** - Usuarios, skills, proyectos
2. **Probar escenarios avanzados** - Proyectos complejos, múltiples candidatos
3. **Configurar background services** - Snapshots y optimización
4. **Implementar vector embeddings** - Para semantic search mejorado
5. **Dashboard de métricas** - Visualizar performance del agente

---

## 📚 Referencias

- [AGENT_GUIDE.md](AGENT_GUIDE.md) - Guía técnica completa del agente
- [QUICKSTART_AGENT.md](QUICKSTART_AGENT.md) - Setup rápido en 5 minutos
- [Examples/AgentClientExample.cs](Examples/AgentClientExample.cs) - Cliente C# con demo

---

**¡El agente está listo para usar! 🎉**
