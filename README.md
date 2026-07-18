# Gestión Humana Solidaria

> Plataforma SaaS de gestión humana para el sector solidario colombiano.
> Cooperativas · Mutuales · Fondos de Empleados · Precooperativas · Asociaciones

## Producto

Sistema integral de gestión humana, asociados y cumplimiento regulatorio para el sector solidario. Construido sobre Clean Architecture .NET 8 con multi-tenencia jerárquica y un asistente IA.

### Módulos Core (en evolución)

- **Persona Unificada** — identidad con roles de asociado/empleado/ambos
- **Asociados** — ciclo de vida completo (admisión, retiro, reingreso, sanciones)
- **Aportes Sociales** — ordinarios, extraordinarios, amortización, devolución
- **Órganos de Gobierno** — asamblea, consejo, junta de vigilancia, comités, actas, voto
- **Balance Social** — dimensiones e indicadores (gobernanza, comunidad, ambiente, educación)
- **Educación Cooperativa** — programas obligatorios, cobertura, evaluación
- **Habeas Data** — cumplimiento Ley 1581/2012
- **Asistente Cooperativo IA** — consultas sobre normatividad y gestiones vía Gemini

## Stack

| Capa | Tecnología |
|------|-----------|
| Backend | .NET 8, ASP.NET Core, EF Core 8, C# 12 |
| Base de datos | SQL Server 2019+ |
| Autenticación | JWT con RBAC multi-tenant |
| IA | Google Gemini (gemini-2.5-flash) |
| Frontend | Angular + TypeScript (repo separado) |
| Infra | Docker, GitHub Actions, Serilog |

## Estructura del repositorio

```
├── API/                 # Backend (Clean Architecture 4 capas)
├── openspec/            # Artefactos SDD (especificaciones, diseño, tareas)
├── docs/                # Documentación del producto
├── infra/               # Configuraciones de infraestructura
├── legal/               # Documentos legales SAS
└── .github/workflows/   # CI/CD
```

## Estado actual

**Fase 0 — Fundación** (en progreso). Preparando la plataforma con tests, CI, editorconfig y saneamiento de secretos antes de empezar a modelar el dominio solidario.

→ Ver [`openspec/changes/gestion-humana-solidaria/`](openspec/changes/gestion-humana-solidaria/) para el change activo.

## Licencia

Propietario — SAS. Todos los derechos reservados.
