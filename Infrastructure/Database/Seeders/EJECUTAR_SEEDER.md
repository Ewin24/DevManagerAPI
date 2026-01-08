# 🚀 Seeder Completo - DevManager

## 📋 Datos insertados

### IAM (Identity & Access)
- **2 Organizaciones:** TechCorp Solutions, InnovateLab
- **3 Roles globales:** Admin, Manager, Developer
- **5 Usuarios** (Password: `Password123!`):
  - `admin@techcorp.com` (Admin + Manager) - Carlos Rodriguez
  - `maria.garcia@techcorp.com` (Manager) - María García
  - `juan.martinez@techcorp.com` (Developer) - Juan Martínez
  - `ana.lopez@techcorp.com` (Developer) - Ana López
  - `admin@innovatelab.com` (Admin) - Pedro Ramírez
- **6 Asignaciones de roles**

### Talent
- **11 Skills:** C#, JavaScript, Python, SQL Server, Azure, React, Docker, Kubernetes, Liderazgo, Comunicación, Metodología TechCorp
- **3 Employee Profiles:** María, Juan, Ana (con bio, experiencia, LinkedIn, portfolios)
- **7 Employee Skills:** Niveles 3-5 en diferentes tecnologías
- **2 Certifications:** Azure Developer Associate (Juan), AWS Solutions Architect (Ana)
- **2 Skill Evaluations:** Tracking de evolución de skills de Juan

### Projects
- **3 Projects:**
  - PROJ-001: Sistema de Gestión Hospitalaria (InProgress, Complexity 3)
  - PROJ-002: E-commerce Multitienda (Open, Complexity 2)
  - PROJ-003: App Móvil de Delivery (Draft, Complexity 1)
- **4 Project Skill Requirements:** Requisitos técnicos por proyecto
- **3 Project Roles:** Tech Lead, Backend Developer, Full Stack Developer
- **3 Project Applications:** Aplicaciones de empleados a proyectos
- **1 Project Assignment:** Juan asignado como Tech Lead en PROJ-001
- **1 Project Participation:** Historial de Juan en PROJ-001

### Reporting
- **2 Report Snapshots:** TeamSkillsMatrix y ProjectStatus (con JSON)
- **2 Recommendation Rules:** Reglas de capacitación y alertas de sobrecarga

**Total:** 59 registros en 17 tablas

## ⚡ Ejecutar Seeder

```bash
# Ejecutar seeder
sqlcmd -S localhost -d DevManager -E -i "c:\Users\pc\Desktop\DevManagerAPI\Infrastructure\Database\Seeders\Seeder.sql"
```

## 🔄 Limpiar y volver a poblar

```bash
# Limpiar base de datos
sqlcmd -S localhost -d DevManager -E -i "c:\Users\pc\Desktop\DevManagerAPI\Infrastructure\Database\Seeders\CleanDatabase.sql"

# Ejecutar seeder de nuevo
sqlcmd -S localhost -d DevManager -E -i "c:\Users\pc\Desktop\DevManagerAPI\Infrastructure\Database\Seeders\Seeder.sql"
```

## 🔑 Organization IDs

```
TechCorp Solutions: 11111111-1111-1111-1111-111111111111
InnovateLab:        22222222-2222-2222-2222-222222222222
```

## 🧪 Pruebas API

### Login (Admin TechCorp)
```bash
curl -X POST https://localhost:7265/Auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@techcorp.com",
    "password": "Password123!",
    "organizationId": "11111111-1111-1111-1111-111111111111"
  }'
```

### Login (Manager TechCorp)
```bash
curl -X POST https://localhost:7265/Auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "maria.garcia@techcorp.com",
    "password": "Password123!",
    "organizationId": "11111111-1111-1111-1111-111111111111"
  }'
```

### Listar Proyectos (con JWT token)
```bash
curl -X GET https://localhost:7265/Projects \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Resultado esperado:** 3 proyectos (PROJ-001, PROJ-002, PROJ-003)

### Listar Empleados
```bash
curl -X GET https://localhost:7265/Employees \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Resultado esperado:** 3 empleados (María, Juan, Ana)

### Listar Skills
```bash
curl -X GET https://localhost:7265/Skills \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Resultado esperado:** 11 skills (10 globales + 1 específica de TechCorp)

## ✅ Verificar datos insertados

```sql
SELECT 
    'Organizations' AS Tabla, COUNT(*) AS Total FROM [iam].[Organizations]
UNION ALL SELECT 'Users', COUNT(*) FROM [iam].[Users]
UNION ALL SELECT 'Roles', COUNT(*) FROM [iam].[Roles]
UNION ALL SELECT 'UserRoles', COUNT(*) FROM [iam].[UserRoles]
UNION ALL SELECT 'Skills', COUNT(*) FROM [talent].[Skills]
UNION ALL SELECT 'EmployeeProfiles', COUNT(*) FROM [talent].[EmployeeProfiles]
UNION ALL SELECT 'EmployeeSkills', COUNT(*) FROM [talent].[EmployeeSkills]
UNION ALL SELECT 'Certifications', COUNT(*) FROM [talent].[Certifications]
UNION ALL SELECT 'Projects', COUNT(*) FROM [projects].[Projects]
UNION ALL SELECT 'ProjectSkillReqs', COUNT(*) FROM [projects].[ProjectSkillRequirements]
UNION ALL SELECT 'ProjectRoles', COUNT(*) FROM [projects].[ProjectRoles]
UNION ALL SELECT 'ProjectApplications', COUNT(*) FROM [projects].[ProjectApplications]
UNION ALL SELECT 'ProjectAssignments', COUNT(*) FROM [projects].[ProjectAssignments]
UNION ALL SELECT 'ProjectParticipation', COUNT(*) FROM [projects].[ProjectParticipation]
UNION ALL SELECT 'SkillEvaluations', COUNT(*) FROM [talent].[SkillEvaluations]
UNION ALL SELECT 'ReportSnapshots', COUNT(*) FROM [reporting].[ReportSnapshots]
UNION ALL SELECT 'RecommendationRules', COUNT(*) FROM [reporting].[RecommendationRules];
```

**Resultado esperado:**
```
Tabla                Total
-------------------- -----
Organizations        2
Users                5
Roles                3
UserRoles            6
Skills               11
EmployeeProfiles     3
EmployeeSkills       7
Certifications       2
Projects             3
ProjectSkillReqs     4
ProjectRoles         3
ProjectApplications  3
ProjectAssignments   1
ProjectParticipation 1
SkillEvaluations     2
ReportSnapshots      2
RecommendationRules  2
```

## 📊 Ver usuarios con sus roles

```sql
SELECT 
    u.Email, 
    u.FirstName + ' ' + u.LastName AS NombreCompleto,
    STRING_AGG(r.Name, ', ') AS Roles
FROM [iam].[Users] u
INNER JOIN [iam].[UserRoles] ur ON u.Id = ur.UserId
INNER JOIN [iam].[Roles] r ON ur.RoleId = r.Id
WHERE u.OrganizationId = '11111111-1111-1111-1111-111111111111'
GROUP BY u.Email, u.FirstName, u.LastName
ORDER BY u.Email;
```

**Resultado esperado:**
```
Email                        NombreCompleto    Roles
---------------------------- ----------------- ----------------
admin@techcorp.com           Carlos Rodriguez  Admin, Manager
ana.lopez@techcorp.com       Ana López         Developer
juan.martinez@techcorp.com   Juan Martínez     Developer
maria.garcia@techcorp.com    María García      Manager
```
