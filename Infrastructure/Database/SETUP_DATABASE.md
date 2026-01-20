# 🗄️ Configuración de Base de Datos - DevManager

Este documento contiene las instrucciones paso a paso para configurar la base de datos SQL Server para DevManager API.

---

## 📋 Prerequisitos

- SQL Server 2019+ instalado (LocalDB, Express, Standard o Enterprise)
- SQL Server Management Studio (SSMS) **O** sqlcmd en línea de comandos
- Permisos de administrador en SQL Server

---

## 🚀 Opción 1: Usando SQL Server Management Studio (SSMS)

### Paso 1: Abrir SSMS
1. Iniciar **SQL Server Management Studio**
2. Conectarse al servidor: `localhost` o `.` (con autenticación Windows)

### Paso 2: Crear la Base de Datos
1. Click derecho en **Databases** → **New Database...**
2. Nombre: `DevManager`
3. Click **OK**

**O ejecutar en Nueva Query:**
```sql
CREATE DATABASE DevManager;
GO
```

### Paso 3: Ejecutar el DDL (Crear Tablas)
1. Abrir archivo: `Infrastructure/Database/DDL/DDL_Dev_Manager.sql`
2. Asegurarse que la base de datos seleccionada sea `DevManager`
3. Click en **Execute** (F5)
4. Verificar que se muestren los mensajes:
   ```
   IAM Schema creado exitosamente
   Talent Schema creado exitosamente
   Projects Schema creado exitosamente
   Reporting Schema creado exitosamente
   ```

### Paso 4: Ejecutar el DDL del Agente (NUEVO v2.0)
1. Abrir archivo: `Infrastructure/Database/DDL/DDL_Agent_Tables.sql`
2. Asegurarse que la base de datos seleccionada sea `DevManager`
3. Click en **Execute** (F5)
4. Verificar mensaje: `Tablas del agente creadas exitosamente`

### Paso 5: Verificar Creación
Ejecutar queries de verificación:

```sql
-- Verificar esquemas
SELECT name FROM sys.schemas 
WHERE name IN ('iam', 'talent', 'projects', 'reporting');

-- Verificar tablas
SELECT TABLE_SCHEMA, TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_SCHEMA, TABLE_NAME;

-- Verificar stored procedures
SELECT 
    SCHEMA_NAME(schema_id) AS [Schema],
    name AS [StoredProcedure]
FROM sys.procedures
WHERE name LIKE 'sp_%'
ORDER BY SCHEMA_NAME(schema_id), name;
```

---

## 💻 Opción 2: Usando sqlcmd (Línea de Comandos)

### Prerequisito: Verificar sqlcmd instalado
```cmd
sqlcmd -?
```
Si no está instalado, descargar de: https://learn.microsoft.com/en-us/sql/tools/sqlcmd/sqlcmd-utility

### Paso 1: Crear Base de Datos
```cmd
cd C:\Users\pc\Desktop\DevManagerAPI

sqlcmd -S localhost -E -Q "CREATE DATABASE DevManager;"
```

**Explicación de parámetros:**
- `-S localhost` : Servidor SQL Server
- `-E` : Autenticación Windows (Trusted Connection)
- `-Q` : Ejecutar query y salir

### Paso 2: Ejecutar DDL
```cmd
sqlcmd -S localhost -d DevManager -E -i Infrastructure\Database\DDL\DDL_Dev_Manager.sql
```

**Parámetros:**
- `-d DevManager` : Base de datos a usar
- `-i archivo.sql` : Ejecutar script desde archivo

### Paso 3: Crear Stored Procedures
```cmd
sqlcmd -S localhost -d DevManager -E -i Infrastructure\Database\StoredProcedures\01_IAM_Procedures.sql
```

### Paso 4: Verificar
```cmd
sqlcmd -S localhost -d DevManager -E -Q "SELECT name FROM sys.schemas WHERE name IN ('iam', 'talent', 'projects', 'reporting');"

sqlcmd -S localhost -d DevManager -E -Q "SELECT COUNT(*) AS TotalTables FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';"

sqlcmd -S localhost -d DevManager -E -Q "SELECT COUNT(*) AS TotalSPs FROM sys.procedures WHERE name LIKE 'sp_%';"
```

---

## 🔧 Opción 3: Script Automatizado (PowerShell)

Crear archivo `setup-database.ps1`:

```powershell
# Variables
$Server = "localhost"
$Database = "DevManager"
$ScriptsPath = "$PSScriptRoot\Infrastructure\Database"

Write-Host "Configurando base de datos DevManager..." -ForegroundColor Cyan

# 1. Crear base de datos
Write-Host "`n1. Creando base de datos..." -ForegroundColor Yellow
sqlcmd -S $Server -E -Q "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '$Database') CREATE DATABASE $Database;"

# 2. Ejecutar DDL
Write-Host "`n2. Ejecutando DDL (creando tablas)..." -ForegroundColor Yellow
sqlcmd -S $Server -d $Database -E -i "$ScriptsPath\DDL\DDL_Dev_Manager.sql"

# 3. Crear Stored Procedures
Write-Host "`n3. Creando Stored Procedures..." -ForegroundColor Yellow
sqlcmd -S $Server -d $Database -E -i "$ScriptsPath\StoredProcedures\01_IAM_Procedures.sql"

# 4. Verificar
Write-Host "`n4. Verificando instalación..." -ForegroundColor Yellow
$schemas = sqlcmd -S $Server -d $Database -E -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.schemas WHERE name IN ('iam', 'talent', 'projects', 'reporting');" -h -1
$tables = sqlcmd -S $Server -d $Database -E -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';" -h -1
$sps = sqlcmd -S $Server -d $Database -E -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.procedures WHERE name LIKE 'sp_%';" -h -1

Write-Host "`n==================================" -ForegroundColor Green
Write-Host "Instalación completada exitosamente" -ForegroundColor Green
Write-Host "==================================" -ForegroundColor Green
Write-Host "Esquemas creados: $schemas" -ForegroundColor White
Write-Host "Tablas creadas: $tables" -ForegroundColor White
Write-Host "Stored Procedures: $sps" -ForegroundColor White
Write-Host "`nConnection String:" -ForegroundColor Cyan
Write-Host "Server=localhost;Database=DevManager;Trusted_Connection=True;TrustServerCertificate=True;" -ForegroundColor White
```

**Ejecutar:**
```powershell
cd C:\Users\pc\Desktop\DevManagerAPI
.\setup-database.ps1
```

---

## 🔍 Verificación Completa

### Queries de Validación

```sql
USE DevManager;
GO

-- 1. Verificar esquemas (debe retornar 4)
SELECT name FROM sys.schemas 
WHERE name IN ('iam', 'talent', 'projects', 'reporting');

-- 2. Verificar todas las tablas (debe retornar ~15 tablas)
SELECT 
    TABLE_SCHEMA,
    TABLE_NAME,
    (SELECT COUNT(*) 
     FROM INFORMATION_SCHEMA.COLUMNS c 
     WHERE c.TABLE_SCHEMA = t.TABLE_SCHEMA 
       AND c.TABLE_NAME = t.TABLE_NAME) AS ColumnCount
FROM INFORMATION_SCHEMA.TABLES t
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_SCHEMA, TABLE_NAME;

-- 3. Verificar stored procedures (debe retornar 7)
SELECT 
    SCHEMA_NAME(schema_id) AS [Schema],
    name AS [Procedure],
    create_date AS Created
FROM sys.procedures
WHERE name LIKE 'sp_%'
ORDER BY create_date;

-- 4. Verificar índices
SELECT 
    SCHEMA_NAME(t.schema_id) AS [Schema],
    t.name AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE SCHEMA_NAME(t.schema_id) IN ('iam', 'talent', 'projects', 'reporting')
  AND i.name IS NOT NULL
ORDER BY [Schema], TableName, IndexName;

-- 5. Verificar constraints
SELECT 
    SCHEMA_NAME(t.schema_id) AS [Schema],
    t.name AS TableName,
    fk.name AS ForeignKey,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable
FROM sys.foreign_keys fk
INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
WHERE SCHEMA_NAME(t.schema_id) IN ('iam', 'talent', 'projects', 'reporting')
ORDER BY [Schema], TableName;
```

### Resultados Esperados

| Elemento | Cantidad Esperada |
|----------|-------------------|
| Esquemas | 4 (iam, talent, projects, reporting) |
| Tablas IAM | 4 (Organizations, Users, Roles, UserRoles) |
| Tablas Talent | 5 (EmployeeProfiles, Skills, EmployeeSkills, Certifications, SkillEvaluations) |
| Tablas Projects | 6 (Projects, ProjectSkillRequirements, ProjectRoles, ProjectApplications, ProjectAssignments, ProjectParticipation) |
| Tablas Reporting | 3 (ReportSnapshots, RecommendationRules, RecommendationLogs) |
| **Total Tablas** | **18** |
| Stored Procedures IAM | 7 |
| **Total SPs** | **7** (por ahora) |

---

## 🧪 Pruebas Básicas

### 1. Insertar una organización de prueba
```sql
USE DevManager;
GO

DECLARE @OrgId UNIQUEIDENTIFIER = NEWID();
DECLARE @UserId UNIQUEIDENTIFIER = NEWID();

-- Ejecutar SP de registro
EXEC sp_Iam_RegisterOrganization
    @OrganizationId = @OrgId OUTPUT,
    @UserId = @UserId OUTPUT,
    @OrganizationName = 'Test Organization',
    @LegalName = 'Test Org S.A.',
    @Nit = '123456789-0',
    @FirstName = 'Admin',
    @LastName = 'User',
    @Email = 'admin@test.com',
    @Phone = '+57 300 1234567',
    @PasswordHash = 0x00, -- Hash dummy para prueba
    @PasswordSalt = 0x00; -- Salt dummy para prueba

SELECT @OrgId AS OrganizationId, @UserId AS UserId;

-- Verificar inserción
SELECT * FROM iam.Organizations WHERE Id = @OrgId;
SELECT Id, FirstName, LastName, Email FROM iam.Users WHERE Id = @UserId;
```

### 2. Probar SP de consulta
```sql
-- Obtener usuario por email
DECLARE @TestOrgId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM iam.Organizations);

EXEC sp_Iam_GetUserByEmail 
    @Email = 'admin@test.com',
    @OrganizationId = @TestOrgId;
```

### 3. Limpiar datos de prueba (opcional)
```sql
-- Eliminar datos de prueba
DELETE FROM iam.Users WHERE Email = 'admin@test.com';
DELETE FROM iam.Organizations WHERE Name = 'Test Organization';
```

---

## ⚠️ Troubleshooting

### Error: "Cannot open database 'DevManager'"
**Solución:** Verificar que la base de datos existe:
```sql
SELECT name FROM sys.databases WHERE name = 'DevManager';
```

### Error: "Login failed for user"
**Solución:** Usar autenticación Windows o configurar SQL Authentication:
```sql
-- Crear login SQL (si es necesario)
CREATE LOGIN devmanager_user WITH PASSWORD = 'YourStrongPassword123!';
USE DevManager;
CREATE USER devmanager_user FOR LOGIN devmanager_user;
ALTER ROLE db_owner ADD MEMBER devmanager_user;
```

**Connection String con SQL Auth:**
```json
"Server=localhost;Database=DevManager;User Id=devmanager_user;Password=YourStrongPassword123!;TrustServerCertificate=True;"
```

### Error: "Invalid object name 'iam.Users'"
**Solución:** Ejecutar primero el DDL completo.

### Error: "Invalid object name 'reporting.AgentActions'"
**Solución:** Ejecutar script DDL del agente: `DDL_Agent_Tables.sql`

---

## 📊 Resumen de Connection Strings

### Autenticación Windows (Recomendado para desarrollo local)
```json
"Server=localhost;Database=DevManager;Trusted_Connection=True;TrustServerCertificate=True;"
```

### SQL Server Authentication
```json
"Server=localhost;Database=DevManager;User Id=devmanager_user;Password=YourPassword;TrustServerCertificate=True;"
```

### LocalDB (si usas LocalDB)
```json
"Server=(localdb)\\mssqllocaldb;Database=DevManager;Trusted_Connection=True;"
```

### Azure SQL Database
```json
"Server=tcp:yourserver.database.windows.net,1433;Database=DevManager;User ID=admin@yourserver;Password=YourPassword;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

---

## ✅ Checklist Final

- [ ] SQL Server instalado y corriendo
- [ ] Base de datos `DevManager` creada
- [ ] 4 esquemas creados (iam, talent, projects, reporting)
- [ ] 18 tablas creadas
- [ ] 7 stored procedures creados (módulo IAM)
- [ ] Connection string configurado en `appsettings.json`
- [ ] Queries de verificación ejecutadas exitosamente
- [ ] Prueba básica de inserción funcionando

---

## 📞 Próximos Pasos

Una vez completada la configuración de la base de datos:

1. ✅ Verificar connection string en `API/appsettings.Development.json`
2. ✅ Ejecutar la API: `dotnet run --project API/API.csproj`
3. ✅ Abrir Swagger: http://localhost:5073/swagger
4. ✅ Probar endpoint: `POST /auth/register`
5. ✅ Verificar datos en la base de datos

---

*Última actualización: 6 de Enero de 2026*
