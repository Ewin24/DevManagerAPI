# 🚀 Script Automatizado de Pruebas del Agente IA
# DevManager API - v2.0.1

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "🤖 DevManager - Test del Agente IA" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Configuración
$baseUrl = "http://localhost:5073"
$email = "juan.perez@techcorp.com"
$password = "Password123!"

# Función para mostrar resultados
function Show-Result {
    param($title, $data, $color = "Green")
    Write-Host "`n--- $title ---" -ForegroundColor $color
    Write-Host ($data | ConvertTo-Json -Depth 5) -ForegroundColor White
}

# Función para hacer solicitudes con manejo de errores
function Invoke-ApiRequest {
    param(
        [string]$Url,
        [string]$Method = "GET",
        [hashtable]$Headers = @{},
        [object]$Body = $null
    )
    
    try {
        $params = @{
            Uri = $Url
            Method = $Method
            Headers = $Headers
            ContentType = "application/json"
        }
        
        if ($Body) {
            $params.Body = ($Body | ConvertTo-Json -Depth 10)
        }
        
        return Invoke-RestMethod @params
    }
    catch {
        Write-Host "❌ Error en la solicitud:" -ForegroundColor Red
        Write-Host $_.Exception.Message -ForegroundColor Red
        return $null
    }
}

Write-Host "📡 Verificando que el API esté corriendo..." -ForegroundColor Yellow
try {
    $healthCheck = Invoke-WebRequest -Uri "$baseUrl/health" -TimeoutSec 5 -ErrorAction Stop
    Write-Host "✅ API está corriendo en $baseUrl" -ForegroundColor Green
}
catch {
    Write-Host "❌ El API no está corriendo. Ejecuta: dotnet run --project API/API.csproj" -ForegroundColor Red
    exit 1
}

# ========================================
# PASO 1: Obtener Token JWT
# ========================================
Write-Host "`n📝 PASO 1: Autenticación..." -ForegroundColor Yellow

$loginBody = @{
    email = $email
    password = $password
}

$loginResponse = Invoke-ApiRequest -Url "$baseUrl/auth/login" -Method POST -Body $loginBody

if (-not $loginResponse) {
    Write-Host "❌ No se pudo obtener el token. Verifica las credenciales." -ForegroundColor Red
    exit 1
}

$token = $loginResponse.data.token
$headers = @{
    "Authorization" = "Bearer $token"
}

Write-Host "✅ Token obtenido exitosamente" -ForegroundColor Green
Write-Host "Usuario: $($loginResponse.data.fullName)" -ForegroundColor Cyan
Write-Host "Organización: $($loginResponse.data.organizationId)" -ForegroundColor Cyan

# ========================================
# PASO 2: Consulta en Lenguaje Natural
# ========================================
Write-Host "`n📝 PASO 2: Consulta en Lenguaje Natural..." -ForegroundColor Yellow

$queryBody = @{
    query = "¿Cuántos desarrolladores tenemos con experiencia en Java y cuál es su nivel?"
}

$queryResponse = Invoke-ApiRequest `
    -Url "$baseUrl/agent/query" `
    -Method POST `
    -Headers $headers `
    -Body $queryBody

if ($queryResponse) {
    Show-Result "Respuesta del Agente" $queryResponse.data
} else {
    Write-Host "⚠️ La consulta falló. Verifica que la API Key de Google AI esté configurada." -ForegroundColor Yellow
}

Start-Sleep -Seconds 2

# ========================================
# PASO 3: Validación Semántica de Skill
# ========================================
Write-Host "`n📝 PASO 3: Validación Semántica de Skill..." -ForegroundColor Yellow

Write-Host "⚠️ Nota: Necesitas IDs reales de la base de datos para esta prueba" -ForegroundColor Yellow
Write-Host "Ejecuta el script SQL de datos de prueba primero (ver AGENT_TESTING_GUIDE.md)" -ForegroundColor Yellow

# Ejemplo con IDs de muestra (reemplazar con IDs reales)
$validateBody = @{
    userId = "00000000-0000-0000-0000-000000000001"  # ⚠️ Reemplazar
    skillId = "00000000-0000-0000-0000-000000000002"  # ⚠️ Reemplazar
    evidenceUrl = "https://github.com/example/java-projects"
    claimedLevel = 4
}

Write-Host "Para probar esta función, configura los IDs reales y descomenta el código." -ForegroundColor Gray

<#
$validateResponse = Invoke-ApiRequest `
    -Url "$baseUrl/agent/validate-skill" `
    -Method POST `
    -Headers $headers `
    -Body $validateBody

if ($validateResponse) {
    Show-Result "Validación de Skill" $validateResponse.data
}
#>

Start-Sleep -Seconds 2

# ========================================
# PASO 4: Matching de Candidatos
# ========================================
Write-Host "`n📝 PASO 4: Matching Inteligente de Candidatos..." -ForegroundColor Yellow

Write-Host "⚠️ Nota: Necesitas un ProjectId real de la base de datos" -ForegroundColor Yellow

# Ejemplo con ID de muestra (reemplazar con ID real)
$matchBody = @{
    projectId = "00000000-0000-0000-0000-000000000003"  # ⚠️ Reemplazar
}

Write-Host "Para probar esta función, configura el ProjectId real y descomenta el código." -ForegroundColor Gray

<#
$matchResponse = Invoke-ApiRequest `
    -Url "$baseUrl/agent/match-candidates" `
    -Method POST `
    -Headers $headers `
    -Body $matchBody

if ($matchResponse) {
    Show-Result "Matching de Candidatos" $matchResponse.data
    
    Write-Host "`n📊 Resumen de Candidatos:" -ForegroundColor Cyan
    foreach ($candidate in $matchResponse.data.candidates) {
        Write-Host "  • $($candidate.fullName) - Score: $($candidate.matchScore)%" -ForegroundColor White
    }
}
#>

# ========================================
# PASO 5: Verificar Auditoría
# ========================================
Write-Host "`n📝 PASO 5: Verificando Auditoría..." -ForegroundColor Yellow

$orgId = $loginResponse.data.organizationId

$auditQuery = @"
SELECT TOP 5
    ActionType,
    Description,
    Status,
    CreatedAt
FROM reporting.AgentActions
WHERE OrganizationId = '$orgId'
ORDER BY CreatedAt DESC
"@

Write-Host "`n🔍 Para ver la auditoría, ejecuta en SQL Server:" -ForegroundColor Cyan
Write-Host $auditQuery -ForegroundColor Gray

# ========================================
# Resumen Final
# ========================================
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "📊 Resumen de Pruebas" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Write-Host "`n✅ Pasos Completados:" -ForegroundColor Green
Write-Host "  1. Autenticación exitosa"
Write-Host "  2. Consulta en lenguaje natural ejecutada"

Write-Host "`n⚠️ Pasos Pendientes (requieren datos reales):" -ForegroundColor Yellow
Write-Host "  3. Validación de skill - Configura userId y skillId"
Write-Host "  4. Matching de candidatos - Configura projectId"
Write-Host "  5. HITL (aprobar/rechazar) - Ejecuta después de validaciones"

Write-Host "`n📚 Documentación Completa:" -ForegroundColor Cyan
Write-Host "  • AGENT_TESTING_GUIDE.md - Guía paso a paso con ejemplos"
Write-Host "  • AGENT_GUIDE.md - Documentación técnica completa"
Write-Host "  • QUICKSTART_AGENT.md - Setup rápido en 5 minutos"

Write-Host "`n🎯 Próximos Pasos:" -ForegroundColor Cyan
Write-Host "  1. Ejecuta el script SQL de datos de prueba (AGENT_TESTING_GUIDE.md)"
Write-Host "  2. Actualiza los IDs en este script"
Write-Host "  3. Descomenta las secciones de prueba"
Write-Host "  4. Vuelve a ejecutar el script"

Write-Host "`n🚀 El agente está listo para usar!" -ForegroundColor Green
Write-Host ""
