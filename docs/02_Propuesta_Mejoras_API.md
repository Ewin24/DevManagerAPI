# Propuesta de Mejoras y Nuevos Endpoints para la API (DevManager)

Este documento detalla las brechas funcionales detectadas en la API actual desde la perspectiva del Frontend, y propone los contratos de datos (Endpoints y DTOs) necesarios para optimizar el rendimiento y la escalabilidad de la aplicación, especialmente en los módulos de Roles y Reportes.

## 1. Módulo de Roles: Contador de Usuarios
**Problema actual:** El Frontend debe descargar la lista completa de usuarios (`GET /api/users`) y agruparlos en memoria para saber cuántos usuarios tienen asignado cada rol. Esto no es escalable.
**Solución propuesta:** Incluir un campo `userCount` directamente en la respuesta de obtención de roles.

### `GET /api/roles`
**Respuesta Esperada (Modificada):**
```json
{
  "success": true,
  "data": [
    {
      "id": "guid",
      "name": "Admin",
      "description": "Administrador del sistema",
      "userCount": 15  // NUEVO CAMPO REQUERIDO
    }
  ]
}
```

## 2. Módulo de Organizaciones y Usuarios: Null-Safety
**Problema actual:** El endpoint de usuarios devuelve `roleName: null` o `undefined` si el usuario no tiene rol. Esto rompe la UI si el Frontend no implementa código defensivo en cada vista.
**Solución propuesta:** Estandarizar el DTO de salida para que devuelva un valor por defecto o asegurar que el Frontend reciba un string vacío.

### `GET /api/users` y `GET /api/users/{id}`
**Comportamiento Esperado:**
Si el usuario no tiene rol, `roleId` debe ser `null`, pero `roleName` debería retornar `"Sin asignar"` o `""` (string vacío), nunca `null` o omitir el campo.

## 3. Módulo de Reportes: Endpoints Nativos Estadísticos
**Problema actual:** El Frontend calcula estadísticas de "Brechas de Habilidades" y "Distribución de Seniority" descargando todos los empleados, todas sus habilidades y todos los proyectos. En una organización grande, esto colapsará el navegador.
**Solución propuesta:** Crear un controlador específico `ReportsController` que realice estos cálculos a nivel de base de datos (SQL `GROUP BY`, `COUNT`, `AVG`).

### Nuevo Endpoint: `GET /api/reports/skills-distribution`
Retorna la distribución de niveles de habilidad en la organización para graficar rápidamente.
**Respuesta Esperada:**
```json
{
  "success": true,
  "data": [
    {
      "skillName": "React",
      "averageLevel": 3.5,
      "totalEmployees": 42,
      "levelDistribution": {
        "1": 5, "2": 10, "3": 15, "4": 10, "5": 2
      }
    }
  ]
}
```

### Nuevo Endpoint: `GET /api/reports/project-metrics`
Retorna métricas clave de los proyectos activos frente a los requerimientos de habilidades.
**Respuesta Esperada:**
```json
{
  "success": true,
  "data": {
    "totalActiveProjects": 12,
    "projectsAtRisk": 3, // Proyectos donde faltan skills mandatorios
    "mostDemandedSkills": [
      { "skillName": "C#", "requiredInProjects": 8 },
      { "skillName": "Azure", "requiredInProjects": 6 }
    ]
  }
}
```

## 4. Módulo de Reportes: Integración con IA
**Problema actual:** El Agente IA existe (`/agent/query`), pero no hay un endpoint que genere un "Resumen Ejecutivo" automático para la vista de reportes de la organización.
**Solución propuesta:**
Crear un endpoint pre-empaquetado que el Frontend pueda llamar al cargar la vista de reportes.

### Nuevo Endpoint: `GET /api/reports/ai-summary`
**Respuesta Esperada:**
```json
{
  "success": true,
  "data": {
    "markdown": "### Resumen Ejecutivo\nLa organización tiene una fuerte competencia en **Backend (.NET)**, pero existe una brecha crítica en **DevOps (Kubernetes)** para los 3 proyectos activos que lo requieren. Se recomienda iniciar capacitaciones de nivelación."
  }
}
```