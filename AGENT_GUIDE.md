# 🤖 DevManager AI Agent - Guía de Integración

**Fecha:** 19 de Enero de 2026  
**Versión:** 1.0 - Agente Cognitivo Implementado

---

## 📋 Resumen Ejecutivo

Se ha implementado exitosamente un **Agente de Orquestación de Talento** con capacidades cognitivas basado en **Google Gemini AI** siguiendo el patrón **Model Context Protocol (MCP)** y arquitectura **Tool Use**.

### Capacidades Implementadas

✅ **Procesamiento en Tiempo Real:**
- Validación semántica de skills con evidencia
- Matching inteligente de candidatos para proyectos
- Consultas en lenguaje natural sobre talento y proyectos

✅ **Procesamiento en Segundo Plano:**
- Generación de snapshots predictivos (cada 24 horas)
- Optimización de reglas de recomendación (cada 6 horas)

✅ **Seguridad y Gobernanza:**
- Multi-tenancy estricto (aislamiento por OrganizationId)
- Human-in-the-loop (HITL) para acciones críticas
- Auditoría completa de todas las acciones del agente
- Cumplimiento de Ley 1581 de 2012 (Habeas Data)

---

## 🏗️ Arquitectura Implementada

```
┌─────────────────────────────────────────────────────────────┐
│                    API Layer (REST)                          │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  AgentController                                     │   │
│  │  - POST /agent/query                                 │   │
│  │  - POST /agent/validate-skill                        │   │
│  │  - POST /agent/match-candidates                      │   │
│  │  - POST /agent/approve/{actionId}                    │   │
│  │  - POST /agent/reject/{actionId}                     │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│              Application Layer (Business Logic)              │
│  ┌──────────────────────┐       ┌────────────────────────┐ │
│  │  AgentService        │◄──────│  MCPTools              │ │
│  │  (Orquestador)       │       │  (Tool Definitions)    │ │
│  └──────────────────────┘       └────────────────────────┘ │
│             ↓                                                │
│  ┌──────────────────────┐                                   │
│  │  GeminiService       │  Chain of Thought (CoT)          │
│  │  (AI Integration)    │  + ReAct Pattern                 │
│  └──────────────────────┘                                   │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│            Infrastructure Layer (Data + AI)                  │
│  ┌──────────────────────┐       ┌────────────────────────┐ │
│  │  AgentRepository     │       │  Google Gemini API     │ │
│  │  (Auditoría)         │       │  (gemini-1.5-flash)    │ │
│  └──────────────────────┘       └────────────────────────┘ │
│                                                              │
│  ┌──────────────────────┐       ┌────────────────────────┐ │
│  │  Background Services │       │  SQL Server            │ │
│  │  - Snapshots         │       │  (Multi-tenant DB)     │ │
│  │  - Optimizer         │       └────────────────────────┘ │
│  └──────────────────────┘                                   │
└─────────────────────────────────────────────────────────────┘
```

---

## 🚀 Setup y Configuración

### Paso 1: Obtener API Key de Google AI

1. Visita [Google AI Studio](https://aistudio.google.com/app/apikey)
2. Crea un nuevo proyecto o selecciona uno existente
3. Genera una API Key
4. Copia la key (formato: `AIza...`)

### Paso 2: Configurar appsettings.json

```json
{
  "GoogleAI": {
    "ApiKey": "TU_API_KEY_AQUI",
    "Model": "gemini-1.5-flash",
    "Temperature": 0.7,
    "MaxTokens": 8192
  },
  "Agent": {
    "EnableBackgroundServices": true,
    "SnapshotIntervalHours": 24,
    "OptimizerIntervalHours": 6,
    "RequireHumanApproval": true
  }
}
```

⚠️ **IMPORTANTE:** No commitear la API Key al repositorio. Usar variables de entorno en producción:

```bash
# Linux/Mac
export GoogleAI__ApiKey="tu_api_key"

# Windows PowerShell
$env:GoogleAI__ApiKey="tu_api_key"

# Docker
-e GoogleAI__ApiKey="tu_api_key"
```

### Paso 3: Crear Tablas del Agente

```bash
sqlcmd -S localhost -d DevManager -E -i Infrastructure/Database/DDL/DDL_Agent_Tables.sql
```

O ejecutar manualmente en SSMS:
- `Infrastructure/Database/DDL/DDL_Agent_Tables.sql`

### Paso 4: Verificar Instalación

```bash
dotnet build
dotnet run --project API/API.csproj
```

Verificar en logs:
```
[Information] ReportSnapshotGenerator iniciado
[Information] RecommendationOptimizer iniciado
```

---

## 📡 Uso de la API

### 1. Consulta General al Agente

**Endpoint:** `POST /agent/query`  
**Autenticación:** Bearer Token requerido

#### Ejemplo: Análisis de Brechas de Capacitación

```bash
curl -X POST http://localhost:5073/agent/query \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "query": "Analiza las brechas de capacitación en el equipo de backend. ¿Qué skills faltan?",
    "context": "Tenemos 3 proyectos activos de .NET Core",
    "requireApproval": false
  }'
```

#### Respuesta:

```json
{
  "success": true,
  "message": "Consulta procesada exitosamente",
  "data": {
    "response": "Basado en el análisis del equipo de backend, se identificaron las siguientes brechas...",
    "reasoningSteps": "1. Analicé los 3 proyectos activos de .NET Core\n2. Identifiqué las skills requeridas...",
    "toolsExecuted": [],
    "requiresHumanApproval": false,
    "actionId": "guid-de-la-accion"
  }
}
```

---

### 2. Validación Semántica de Skills

**Endpoint:** `POST /agent/validate-skill`  
**Autenticación:** Bearer Token requerido

#### Ejemplo: Validar Skill de "Arquitectura .NET"

```bash
curl -X POST http://localhost:5073/agent/validate-skill \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "guid-del-empleado",
    "skillId": "guid-de-skill-dotnet-architecture",
    "level": 4,
    "evidenceUrl": "https://github.com/user/netcore-project",
    "experienceDescription": "He liderado la migración de una aplicación monolítica a microservicios usando .NET Core 6, implementando CQRS y Event Sourcing. Coordiné un equipo de 8 desarrolladores y definí los estándares de arquitectura de la empresa..."
  }'
```

#### Respuesta:

```json
{
  "success": true,
  "message": "Skill validado exitosamente",
  "data": {
    "isValid": true,
    "confidenceScore": 85.5,
    "validationReasoning": "El nivel 4/5 es coherente con:\n- 5 años de experiencia en .NET\n- Liderazgo de equipo de 8 desarrolladores\n- Implementación de patrones avanzados (CQRS, DDD)\n- Certificación Microsoft Azure Solutions Architect\n- Portfolio con proyectos complejos en GitHub",
    "recommendations": [
      "Considerar certificación en Azure DevOps para complementar el perfil",
      "Documentar más proyectos de microservicios"
    ],
    "warnings": [],
    "requiresCertification": false
  }
}
```

**Nota:** El campo `experienceDescription` permite al empleado describir su experiencia de forma libre. El agente IA analiza tanto la URL de evidencia como esta descripción para validar el nivel declarado.

---

### 3. Matching Inteligente de Candidatos

**Endpoint:** `POST /agent/match-candidates`  
**Autenticación:** Bearer Token requerido

#### Ejemplo: Encontrar Candidatos para Proyecto

```bash
curl -X POST http://localhost:5073/agent/match-candidates \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "projectId": "guid-del-proyecto",
    "maxCandidates": 5,
    "includeReasoningDetails": true
  }'
```

#### Respuesta:

```json
{
  "success": true,
  "message": "Se encontraron 5 candidatos",
  "data": {
    "projectId": "guid-del-proyecto",
    "projectName": "Sistema de Facturación Electrónica",
    "candidates": [
      {
        "userId": "guid-candidato-1",
        "fullName": "Juan Pérez",
        "email": "juan.perez@empresa.com",
        "matchScore": 92.5,
        "skillAlignments": [
          {
            "skillName": ".NET Core",
            "requiredLevel": 4,
            "currentLevel": 5,
            "isMandatory": true,
            "meets": true
          },
          {
            "skillName": "SQL Server",
            "requiredLevel": 3,
            "currentLevel": 4,
            "isMandatory": true,
            "meets": true
          }
        ],
        "recommendationReason": "Candidato ideal: cumple todos los requisitos obligatorios, supera el nivel requerido en .NET Core, y tiene experiencia previa en proyectos de facturación."
      }
    ],
    "analysisNarrative": "Se analizaron 15 perfiles disponibles. Los top 5 candidatos cumplen con al menos el 80% de los requisitos..."
  }
}
```

---

### 4. Consultas Personalizadas del Usuario Actual

**Endpoint:** `POST /agent/query`  
**Autenticación:** Bearer Token requerido

El agente ahora puede responder preguntas personalizadas sobre TI sin necesidad de especificar tu userId. El sistema extrae automáticamente tu identidad del token JWT.

#### Cómo Funciona

El sistema detecta automáticamente cuando la consulta se refiere al usuario actual mediante pronombres:
- **Español:** "yo", "mi", "mis", "mí", "para mí", "para mi", "me recomi"
- **Inglés:** "my", "me", "I would like"
- **Otros idiomas:** "me适合" (chino)

#### Ejemplo: Recomendaciones de Habilidades

```bash
curl -X POST http://localhost:5073/agent/query \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "query": "¿Qué habilidades me recomiendan aprender basándome en mi perfil actual?",
    "requireApproval": false
  }'
```

#### Ejemplo: Proyectos que Encajan con Mis Habilidades

```bash
curl -X POST http://localhost:5073/agent/query \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "query": "¿Qué proyectos activos encajan mejor con mis habilidades?",
    "requireApproval": false
  }'
```

#### Ejemplo: Análisis de Perfil Profesional

```bash
curl -X POST http://localhost:5073/agent/query \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "query": "Analiza mi perfil y dame recomendaciones para desarrollar mi carrera",
    "requireApproval": false
  }'
```

#### Respuesta

```json
{
  "success": true,
  "message": "Consulta procesada exitosamente",
  "data": {
    "response": "Basado en tu perfil actual:\n\n**Tus fortalezas:**\n- C# nivel 4 (Avanzado)\n- .NET Core nivel 4\n- SQL Server nivel 4\n\n**Oportunidades de mejora:**\n- Kubernetes nivel 2 → 3 (requerido para proyectos cloud\n- Angular/React nivel 3 (alta demanda)\n\n**Recomendaciones:**\n1. Considerar certificación Azure Developer Associate\n2. Participar en el proyecto SIST-HOSP-001 para ganar experiencia cloud\n3. Tomar el curso de Microservicios en la plataforma de capacitación",
    "reasoningSteps": "1. Obtuve tu perfil del token JWT\n2. Recuperé tus skills: C#(4), .NET Core(4), SQL(4)\n3. Comparé con requisitos de proyectos activos\n4. Identifiqué gaps y oportunidades",
    "toolsExecuted": ["get_current_user_context"],
    "requiresHumanApproval": false,
    "actionId": null,
    "confidence": 85
  }
}
```

#### Datos Obtenidos Automáticamente

Cuando detectas una consulta personalizada, el sistema obtiene:
- **Perfil:** Bio, años de experiencia
- **Habilidades:** Nombre, nivel, si está validada
- **Certificaciones:** Nombre, emissor, fecha de emisión

---

### 5. Aprobar/Rechazar Acciones (HITL)

#### Aprobar una Acción

```bash
curl -X POST http://localhost:5073/agent/approve/guid-de-accion \
  -H "Authorization: Bearer YOUR_TOKEN"
```

#### Rechazar una Acción

```bash
curl -X POST http://localhost:5073/agent/reject/guid-de-accion \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "reason": "El candidato no tiene disponibilidad para iniciar el proyecto"
  }'
```

---

## 🔐 Seguridad y Gobernanza

### Multi-tenancy Estricto

```csharp
// El agente SIEMPRE filtra por OrganizationId del JWT
var organizationId = GetOrganizationIdFromJWT();

// TODAS las queries incluyen este filtro
SELECT * FROM talent.EmployeeProfiles 
WHERE OrganizationId = @OrganizationId 
  AND IsDeleted = 0
```

### Human-in-the-loop (HITL)

Acciones que **REQUIEREN** aprobación humana:
- Asignación de empleados a proyectos
- Cambios de nivel de skills (nivel 4 o superior)
- Recomendaciones con confidence < 70%

```csharp
// Configurar por organización
{
  "RequireHumanApproval": true,
  "MinConfidenceThreshold": 70.0
}
```

### Auditoría Completa

Tabla `reporting.AgentActions`:
- **Qué:** Tipo de acción ejecutada
- **Quién:** UserId del ejecutor (null si automático)
- **Cuándo:** Timestamp UTC
- **Datos:** Input/Output completos en JSON
- **Estado:** SUCCESS, FAILED, PENDING_APPROVAL, APPROVED, REJECTED
- **Aprobador:** UserId del aprobador (HITL)

---

## 🧪 Testing del Agente

### Test 1: Validación de Skills

```bash
# 1. Obtener token
TOKEN=$(curl -X POST http://localhost:5073/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@test.com","password":"Test123!"}' \
  | jq -r '.data.token')

# 2. Validar skill
curl -X POST http://localhost:5073/agent/validate-skill \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "tu-user-id",
    "skillId": "tu-skill-id",
    "level": 4,
    "evidenceUrl": "https://github.com/tu-repo"
  }'
```

### Test 2: Matching de Candidatos

```bash
curl -X POST http://localhost:5073/agent/match-candidates \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "projectId": "tu-project-id",
    "maxCandidates": 10
  }'
```

### Test 3: Consulta Natural

```bash
curl -X POST http://localhost:5073/agent/query \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "query": "¿Quiénes son los desarrolladores senior de Python con experiencia en machine learning?",
    "requireApproval": false
  }'
```

---

## 📊 Monitoreo y Logs

### Ver Logs del Agente

```bash
# Logs en tiempo real
dotnet run --project API | grep "Infrastructure.Services.AI"

# Filtrar solo acciones del agente
dotnet run --project API | grep "AgentService"
```

### Logs Esperados

```
[Information] Consultando Gemini con prompt de 450 caracteres
[Information] Respuesta de Gemini: 2543 tokens
[Information] Procesando query del agente para org guid: "¿Quiénes..."
[Information] Validando skill guid nivel 4 para usuario guid
[Information] Matching candidatos para proyecto guid
[Information] ReportSnapshotGenerator iniciado
[Information] Optimizando reglas de recomendación...
```

---

## 🎯 Casos de Uso Avanzados

### Caso 1: Detección Proactiva de Brechas

```json
{
  "query": "Analiza los próximos 3 meses de proyectos planificados. ¿Necesitamos contratar o capacitar personal?",
  "context": "Tenemos 5 proyectos en pipeline que inician en Q2 2026"
}
```

El agente analizará:
- Skills requeridas en proyectos planificados
- Skills disponibles en el equipo actual
- Gaps identificados
- Recomendación: contratar vs capacitar

### Caso 2: Análisis de Tendencias

```json
{
  "query": "¿Qué skills están más demandadas en los proyectos del último año?",
  "requireApproval": false
}
```

El agente generará:
- Top 10 skills más requeridas
- Tendencias de crecimiento
- Skills emergentes
- Recomendaciones de capacitación

### Caso 3: Optimización de Asignaciones

```json
{
  "query": "Tengo 3 proyectos y 10 desarrolladores. ¿Cuál es la asignación óptima para maximizar la productividad?",
  "context": "Proyectos: A (urgente), B (medio), C (largo plazo)"
}
```

El agente considerará:
- Match de skills por proyecto
- Carga de trabajo actual
- Historial de contribución
- Balance de equipo

---

## ⚡ Optimización de Performance

### Reducción de Uso de Tokens

**Divulgación Progresiva:**
```csharp
// ❌ MAL: Enviar todo el esquema
var allData = GetAllDatabase(); // 100,000 tokens

// ✅ BIEN: Solo lo necesario
var projectRequirements = GetProjectRequirements(projectId); // 500 tokens
var relevantCandidates = GetCandidatesWithSkills(requiredSkills); // 2000 tokens
```

Esto logra una **reducción del 98.7%** en uso de tokens según el paper MCP.

### Caching de Resultados

```csharp
// Background service cachea:
- Catálogo de skills (refresca cada 6 horas)
- Perfiles de empleados activos (refresca cada 1 hora)
- Requisitos de proyectos activos (tiempo real)
```

---

## 🐛 Troubleshooting

### Error: "GoogleAI:ApiKey no configurado"

**Solución:**
```json
// appsettings.json
{
  "GoogleAI": {
    "ApiKey": "TU_API_KEY_REAL"
  }
}
```

### Error: "Error de API Gemini (429): Quota exceeded"

**Solución:**
1. Verificar límites en [Google AI Studio](https://aistudio.google.com)
2. Reducir frecuencia de background services
3. Implementar rate limiting

### Error: "No se pudo parsear respuesta estructurada"

**Causa:** Gemini retornó texto no-JSON  
**Solución:** Ya implementado fallback automático en `GeminiService.cs`

### Background Services no inician

**Verificar:**
```json
{
  "Agent": {
    "EnableBackgroundServices": true  // ← debe ser true
  }
}
```

---

## 📈 Métricas y KPIs

### Métricas del Agente (reporting.AgentActions)

```sql
-- Tasa de éxito del agente
SELECT 
    ActionType,
    COUNT(*) as Total,
    SUM(CASE WHEN Status = 'SUCCESS' THEN 1 ELSE 0 END) as Exitosas,
    CAST(SUM(CASE WHEN Status = 'SUCCESS' THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS DECIMAL(5,2)) as TasaExito
FROM reporting.AgentActions
WHERE OrganizationId = @OrgId
  AND CreatedAt >= DATEADD(day, -30, GETUTCDATE())
GROUP BY ActionType;

-- Tiempo de aprobación (HITL)
SELECT 
    AVG(DATEDIFF(minute, CreatedAt, ApprovedAt)) as MinutosPromedioAprobacion
FROM reporting.AgentActions
WHERE Status IN ('APPROVED', 'REJECTED')
  AND ApprovedAt IS NOT NULL;
```

---

## 🔄 Próximos Pasos

### Fase 2: Mejoras Planificadas

- [ ] Implementar vector embeddings para semantic search de skills
- [ ] Agregar memoria conversacional (historial de queries por usuario)
- [ ] Integrar con Azure OpenAI para redundancia
- [ ] Fine-tuning de Gemini con datos históricos de la organización
- [ ] Dashboard de métricas del agente en tiempo real
- [ ] Notificaciones proactivas (Slack/Teams)

### Fase 3: Capacidades Avanzadas

- [ ] Análisis de sentimientos en feedback de proyectos
- [ ] Predicción de riesgo de rotación de talento
- [ ] Recomendaciones de capacitación personalizadas
- [ ] Simulación de escenarios "what-if"

---

## 📚 Referencias

- [Google Gemini API Documentation](https://ai.google.dev/docs)
- [Model Context Protocol (MCP)](https://modelcontextprotocol.io/)
- [Chain of Thought Prompting](https://arxiv.org/abs/2201.11903)
- [ReAct: Reasoning and Acting](https://arxiv.org/abs/2210.03629)

---

**Estado:** ✅ **Agente Cognitivo 100% Funcional**  
**Próximo milestone:** Fine-tuning y optimización con datos reales

---

*Última actualización: 19 de Enero de 2026*
