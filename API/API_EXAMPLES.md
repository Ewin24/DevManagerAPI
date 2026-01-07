# 📡 API Examples - DevManager

Ejemplos completos de uso de la API DevManager con curl, Postman y C#.

---

## 🔐 Módulo de Autenticación

### 1. Registrar Organización + Usuario Admin

**Endpoint:** `POST /auth/register`  
**Autenticación:** No requiere (público)

#### curl
```bash
curl -X POST http://localhost:5073/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "organizationName": "TechCorp Solutions",
    "legalName": "TechCorp Solutions S.A.S.",
    "nit": "900123456-1",
    "firstName": "Edwin",
    "lastName": "Trigos",
    "email": "edwin.trigos@techcorp.com",
    "phone": "+57 310 1234567",
    "password": "SecurePass123!"
  }'
```

#### Respuesta Exitosa (201 Created)
```json
{
  "success": true,
  "message": "Organización registrada exitosamente",
  "data": {
    "organizationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "token": "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI3YzllNjY3OS03NDI1LTQwZGUtOTQ0Yi1lMDdmYzFmOTBhZTciLCJlbWFpbCI6Im1hcmlhLnJvZHJpZ3VlekB0ZWNoY29ycC5jb20iLCJnaXZlbl9uYW1lIjoiTWFyaWEgUm9kcmlndWV6IiwiT3JnYW5pemF0aW9uSWQiOiIzZmE4NWY2NC01NzE3LTQ1NjItYjNmYy0yYzk2M2Y2NmFmYTYiLCJqdGkiOiJhYmNkZWYxMi0zNDU2LTc4OTAtYWJjZC1lZjEyMzQ1Njc4OTAiLCJuYmYiOjE3MzYxOTcyMDAsImV4cCI6MTczNjI4MzYwMCwiaXNzIjoiRGV2TWFuYWdlckFQSSIsImF1ZCI6IkRldk1hbmFnZXJDbGllbnQifQ.signature"
  },
  "timestamp": "2026-01-06T18:00:00.000Z"
}
```

#### Respuesta Error - Email Duplicado (409 Conflict)
```json
{
  "success": false,
  "message": "El email 'edwin.trigos@techcorp.com' ya está registrado",
  "errorCode": "CONFLICT",
  "errors": {},
  "traceId": "0HMVQE:00000001",
  "timestamp": "2026-01-06T18:00:00.000Z"
}
```

---

### 2. Login

**Endpoint:** `POST /auth/login`  
**Autenticación:** No requiere (público)

#### curl
```bash
curl -X POST http://localhost:5073/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "edwin.trigos@techcorp.com",
    "password": "SecurePass123!"
  }'
```

#### PowerShell
```powershell
$credentials = @{
    email = "edwin.trigos@techcorp.com"
    password = "SecurePass123!"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "http://localhost:5073/auth/login" `
  -Method Post `
  -ContentType "application/json" `
  -Body $credentials

# Guardar token en variable
$token = $response.data.token
Write-Host "Token: $token"
```

#### JavaScript (Fetch)
```javascript
const login = async () => {
  const response = await fetch('http://localhost:5073/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      email: 'edwin.trigos@techcorp.com',
      password: 'SecurePass123!'
    })
  });
  
  const result = await response.json();
  if (result.success) {
    localStorage.setItem('token', result.data.token);
    console.log('Login exitoso:', result.data.user);
  }
};
```

#### Respuesta Exitosa (200 OK)
```json
{
  "success": true,
  "message": "Login exitoso",
  "data": {
    "token": "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
      "firstName": "Maria",
      "lastName": "Rodriguez",
      "email": "edwin.trigos@techcorp.com",
      "phone": "+57 310 1234567",
      "isActive": true,
      "lastLoginAt": "2026-01-06T18:00:00.000Z",
      "createdAt": "2026-01-06T17:30:00.000Z"
    }
  },
  "timestamp": "2026-01-06T18:00:00.000Z"
}
```

#### Respuesta Error - Credenciales Inválidas (401 Unauthorized)
```json
{
  "success": false,
  "message": "Email o contraseña incorrectos",
  "errorCode": "UNAUTHORIZED",
  "errors": {},
  "traceId": "0HMVQE:00000002",
  "timestamp": "2026-01-06T18:00:00.000Z"
}
```

---

## 👥 Módulo de Usuarios

**⚠️ TODOS los endpoints requieren JWT Bearer token**

### 3. Listar Usuarios

**Endpoint:** `GET /users`  
**Autenticación:** Bearer Token requerido

#### curl
```bash
curl -X GET http://localhost:5073/users \
  -H "Authorization: Bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9..."
```

#### PowerShell
```powershell
$token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9..."

$headers = @{
    "Authorization" = "Bearer $token"
}

$users = Invoke-RestMethod -Uri "http://localhost:5073/users" `
  -Method Get `
  -Headers $headers

$users.data | Format-Table
```

#### C#
```csharp
var client = new HttpClient { BaseAddress = new Uri("http://localhost:5073") };
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", "your_token_here");

var response = await client.GetAsync("/users");
var json = await response.Content.ReadAsStringAsync();
var result = JsonSerializer.Deserialize<ApiResponse<List<UserResponse>>>(json);

foreach (var user in result.Data)
{
    Console.WriteLine($"{user.FirstName} {user.LastName} - {user.Email}");
}
```

#### Respuesta Exitosa (200 OK)
```json
{
  "success": true,
  "message": "Usuarios obtenidos exitosamente",
  "data": [
    {
      "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
      "firstName": "Maria",
      "lastName": "Rodriguez",
      "email": "edwin.trigos@techcorp.com",
      "phone": "+57 310 1234567",
      "isActive": true,
      "lastLoginAt": "2026-01-06T18:00:00.000Z",
      "createdAt": "2026-01-06T17:30:00.000Z"
    },
    {
      "id": "8d0f7780-8536-51ef-c4gd-3d074g77bgb8",
      "firstName": "Juan",
      "lastName": "Pérez",
      "email": "juan.perez@techcorp.com",
      "phone": "+57 320 9876543",
      "isActive": true,
      "lastLoginAt": null,
      "createdAt": "2026-01-06T18:15:00.000Z"
    }
  ],
  "timestamp": "2026-01-06T18:30:00.000Z"
}
```

---

### 4. Obtener Usuario por ID

**Endpoint:** `GET /users/{id}`  
**Autenticación:** Bearer Token requerido

#### curl
```bash
curl -X GET http://localhost:5073/users/7c9e6679-7425-40de-944b-e07fc1f90ae7 \
  -H "Authorization: Bearer your_token_here"
```

#### PowerShell
```powershell
$userId = "7c9e6679-7425-40de-944b-e07fc1f90ae7"
$token = "your_token_here"

$headers = @{ "Authorization" = "Bearer $token" }

Invoke-RestMethod -Uri "http://localhost:5073/users/$userId" `
  -Method Get `
  -Headers $headers
```

#### Respuesta Error - Usuario No Encontrado (404 Not Found)
```json
{
  "success": false,
  "message": "Usuario no encontrado",
  "errorCode": "NOT_FOUND",
  "errors": {
    "Id": ["El usuario con ID '7c9e6679-7425-40de-944b-e07fc1f90ae7' no existe"]
  },
  "traceId": "0HMVQE:00000003",
  "timestamp": "2026-01-06T18:30:00.000Z"
}
```

---

### 5. Crear Usuario

**Endpoint:** `POST /users`  
**Autenticación:** Bearer Token requerido

#### curl
```bash
curl -X POST http://localhost:5073/users \
  -H "Authorization: Bearer your_token_here" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Carlos",
    "lastName": "Mendoza",
    "email": "carlos.mendoza@techcorp.com",
    "phone": "+57 315 5555555",
    "password": "CarlosPass123!"
  }'
```

#### PowerShell
```powershell
$token = "your_token_here"

$newUser = @{
    firstName = "Carlos"
    lastName = "Mendoza"
    email = "carlos.mendoza@techcorp.com"
    phone = "+57 315 5555555"
    password = "CarlosPass123!"
} | ConvertTo-Json

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

Invoke-RestMethod -Uri "http://localhost:5073/users" `
  -Method Post `
  -Headers $headers `
  -Body $newUser
```

#### JavaScript (Axios)
```javascript
const createUser = async (token) => {
  try {
    const response = await axios.post('http://localhost:5073/users', {
      firstName: 'Carlos',
      lastName: 'Mendoza',
      email: 'carlos.mendoza@techcorp.com',
      phone: '+57 315 5555555',
      password: 'CarlosPass123!'
    }, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    console.log('Usuario creado:', response.data);
  } catch (error) {
    console.error('Error:', error.response.data);
  }
};
```

#### Respuesta Exitosa (201 Created)
```json
{
  "success": true,
  "message": "Usuario creado exitosamente",
  "data": {
    "id": "9e1f8891-9647-62fg-d5he-4e185h88chc9",
    "firstName": "Carlos",
    "lastName": "Mendoza",
    "email": "carlos.mendoza@techcorp.com",
    "phone": "+57 315 5555555",
    "isActive": true,
    "lastLoginAt": null,
    "createdAt": "2026-01-06T19:00:00.000Z"
  },
  "timestamp": "2026-01-06T19:00:00.000Z"
}
```

---

### 6. Actualizar Usuario

**Endpoint:** `PUT /users/{id}`  
**Autenticación:** Bearer Token requerido

#### curl
```bash
curl -X PUT http://localhost:5073/users/9e1f8891-9647-62fg-d5he-4e185h88chc9 \
  -H "Authorization: Bearer your_token_here" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Carlos Andrés",
    "lastName": "Mendoza García",
    "phone": "+57 315 9999999",
    "isActive": true
  }'
```

#### PowerShell
```powershell
$userId = "9e1f8891-9647-62fg-d5he-4e185h88chc9"
$token = "your_token_here"

$updateData = @{
    firstName = "Carlos Andrés"
    lastName = "Mendoza García"
    phone = "+57 315 9999999"
    isActive = $true
} | ConvertTo-Json

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

Invoke-RestMethod -Uri "http://localhost:5073/users/$userId" `
  -Method Put `
  -Headers $headers `
  -Body $updateData
```

---

### 7. Eliminar Usuario (Soft Delete)

**Endpoint:** `DELETE /users/{id}`  
**Autenticación:** Bearer Token requerido

#### curl
```bash
curl -X DELETE http://localhost:5073/users/9e1f8891-9647-62fg-d5he-4e185h88chc9 \
  -H "Authorization: Bearer your_token_here"
```

#### PowerShell
```powershell
$userId = "9e1f8891-9647-62fg-d5he-4e185h88chc9"
$token = "your_token_here"

$headers = @{ "Authorization" = "Bearer $token" }

Invoke-RestMethod -Uri "http://localhost:5073/users/$userId" `
  -Method Delete `
  -Headers $headers
```

#### Respuesta Exitosa (200 OK)
```json
{
  "success": true,
  "message": "Usuario eliminado exitosamente",
  "data": null,
  "timestamp": "2026-01-06T19:30:00.000Z"
}
```

---

## 🧪 Colección Postman

### Importar Variables de Entorno

```json
{
  "name": "DevManager Local",
  "values": [
    {
      "key": "baseUrl",
      "value": "http://localhost:5073",
      "enabled": true
    },
    {
      "key": "token",
      "value": "",
      "enabled": true
    }
  ]
}
```

### Script Pre-request (Login automático)

En la pestaña **Pre-request Script** de tu colección:

```javascript
// Auto-login si no hay token
if (!pm.environment.get("token")) {
    pm.sendRequest({
        url: pm.environment.get("baseUrl") + "/auth/login",
        method: 'POST',
        header: {
            'Content-Type': 'application/json'
        },
        body: {
            mode: 'raw',
            raw: JSON.stringify({
                email: "edwin.trigos@techcorp.com",
                password: "SecurePass123!"
            })
        }
    }, function (err, response) {
        if (!err && response.json().success) {
            pm.environment.set("token", response.json().data.token);
        }
    });
}
```

### Authorization Header (en Headers)

```
Authorization: Bearer {{token}}
```

---

## 🐛 Manejo de Errores Comunes

### 401 Unauthorized - Token Expirado
```json
{
  "success": false,
  "message": "Token expirado o inválido",
  "errorCode": "UNAUTHORIZED",
  "errors": {},
  "traceId": "0HMVQE:00000005",
  "timestamp": "2026-01-06T20:00:00.000Z"
}
```

**Solución:** Hacer login nuevamente para obtener un nuevo token.

---

### 403 Forbidden - Sin permisos
```json
{
  "success": false,
  "message": "No tiene permisos para acceder a este recurso",
  "errorCode": "FORBIDDEN",
  "errors": {},
  "traceId": "0HMVQE:00000006",
  "timestamp": "2026-01-06T20:00:00.000Z"
}
```

**Solución:** Verificar que el usuario tenga los roles necesarios.

---

### 400 Bad Request - Validación Fallida
```json
{
  "success": false,
  "message": "Error de validación",
  "errorCode": "VALIDATION_ERROR",
  "errors": {
    "Email": ["El email no es válido"],
    "Password": ["La contraseña debe tener al menos 8 caracteres"]
  },
  "traceId": "0HMVQE:00000007",
  "timestamp": "2026-01-06T20:00:00.000Z"
}
```

---

## 🔧 Herramientas Útiles

### Decode JWT Token (online)
https://jwt.io

Pegar tu token para ver los claims:
```json
{
  "nameid": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "email": "edwin.trigos@techcorp.com",
  "given_name": "Maria Rodriguez",
  "OrganizationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "jti": "abcdef12-3456-7890-abcd-ef1234567890",
  "nbf": 1736197200,
  "exp": 1736283600,
  "iss": "DevManagerAPI",
  "aud": "DevManagerClient"
}
```

### Test con HTTPie (alternativa a curl)
```bash
# Instalar: pip install httpie

# Registro
http POST localhost:5073/auth/register \
  organizationName="Test Org" \
  firstName="Test" \
  lastName="User" \
  email="test@test.com" \
  password="Test123!"

# Login
http POST localhost:5073/auth/login \
  email="test@test.com" \
  password="Test123!"

# Listar usuarios
http GET localhost:5073/users \
  "Authorization:Bearer your_token_here"
```

---

## 📊 Flujo Completo de Trabajo

```bash
# 1. Registrar organización
curl -X POST http://localhost:5073/auth/register -H "Content-Type: application/json" -d '{"organizationName":"MyOrg","firstName":"Admin","lastName":"User","email":"admin@myorg.com","password":"Admin123!"}'

# 2. Login y obtener token
TOKEN=$(curl -X POST http://localhost:5073/auth/login -H "Content-Type: application/json" -d '{"email":"admin@myorg.com","password":"Admin123!"}' | jq -r '.data.token')

# 3. Usar el token para crear un usuario
curl -X POST http://localhost:5073/users \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"firstName":"John","lastName":"Doe","email":"john@myorg.com","password":"John123!"}'

# 4. Listar usuarios
curl -X GET http://localhost:5073/users \
  -H "Authorization: Bearer $TOKEN"
```

---

*Última actualización: 6 de Enero de 2026*
