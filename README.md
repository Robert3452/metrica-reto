# Reto Fullstack: Gestión de Pedidos

Aplicación fullstack para autenticación de usuarios y gestión (CRUD) de pedidos.

- **Backend:** .NET 8, API REST, Clean Architecture, EF Core, SQL Server, JWT
- **Frontend:** React + TypeScript (Vite), Tailwind CSS, TanStack Query
- **Base de datos:** SQL Server 2022 en Docker

---

## Estructura del repositorio

```
Metrica_Reto/
├── docker-compose.yml        # SQL Server
├── .env.example              # Variables de Docker (copiar a .env)
├── backend/
│   ├── global.json           # Fija el SDK de .NET 8
│   ├── Pedidos.sln
│   └── src/
│       ├── Pedidos.Domain/          # Entidades, enums, reglas de negocio
│       ├── Pedidos.Application/     # Casos de uso, interfaces, DTOs
│       ├── Pedidos.Infrastructure/  # EF Core, repositorios, JWT, hashing
│       └── Pedidos.Api/             # Controllers, middleware, configuración
└── frontend/
    └── src/
        ├── api/              # Cliente HTTP e interceptores
        ├── auth/             # Sesión, contexto y rutas protegidas
        ├── components/       # Layout, menú, ErrorBoundary
        └── pages/            # Login, listado, formulario
```

---

## Requisitos previos

| Herramienta | Versión | Verificar con |
|---|---|---|
| Docker Desktop | Reciente | `docker --version` |
| .NET SDK | 8.0.x | `dotnet --list-sdks` |
| Node.js | 20.19+ o 22.12+ | `node -v` |

**Mac con Apple Silicon (M1/M2/M3/M4):** la imagen de SQL Server solo existe para `amd64`. Activa en Docker Desktop:
**Settings > General > "Use Rosetta for x86_64/amd64 emulation on Apple Silicon"**.

En Windows, Linux y Mac Intel corre de forma nativa, sin configuración extra.

---

## 1. Base de datos (Docker)

Desde la raíz del repositorio, copia el archivo de variables:

```bash
# macOS / Linux
cp .env.example .env

# Windows (PowerShell)
Copy-Item .env.example .env
```

Edita `.env` si lo necesitas:

```env
DB_PLATFORM=linux/amd64
DB_PORT=1433
SA_PASSWORD=Pedidos_DEV_2026!
```

> La contraseña debe tener al menos 8 caracteres y 3 de estos 4 tipos: mayúscula, minúscula, número y símbolo. Si no cumple, el contenedor se detiene al iniciar (revisa con `docker logs pedidos-sql`).

Levanta el contenedor y espera a que figure como `(healthy)`:

```bash
docker compose up -d
docker compose ps
```

---

## 2. Secrets del backend

Los datos sensibles (connection string y clave de firma del JWT) **no están en el repositorio**. Se configuran con **User Secrets** de .NET, que los guarda fuera del proyecto, en la carpeta del usuario. El proyecto ya tiene su `UserSecretsId`, así que no hace falta ejecutar `init`.

Todos los comandos se ejecutan desde la carpeta `backend/`.

### 2.1 Connection string

Usa la misma contraseña que pusiste en `SA_PASSWORD` del `.env`.

**macOS / Linux (zsh o bash)**, con comillas simples para que el `!` no se interprete:

```bash
dotnet user-secrets set 'ConnectionStrings:Default' 'Server=localhost,1433;Database=PedidosDb;User Id=sa;Password=Pedidos_DEV_2026!;TrustServerCertificate=True' --project src/Pedidos.Api
```

**Windows (PowerShell):**

```powershell
dotnet user-secrets set "ConnectionStrings:Default" 'Server=localhost,1433;Database=PedidosDb;User Id=sa;Password=Pedidos_DEV_2026!;TrustServerCertificate=True' --project src/Pedidos.Api
```

### 2.2 Clave de firma del JWT

Debe tener al menos 32 caracteres. Genera una aleatoria:

**macOS / Linux:**

```bash
dotnet user-secrets set 'Jwt:Key' "$(openssl rand -base64 48)" --project src/Pedidos.Api
```

**Windows (PowerShell):**

```powershell
$key = [Convert]::ToBase64String((1..48 | ForEach-Object { Get-Random -Maximum 256 }))
dotnet user-secrets set "Jwt:Key" $key --project src/Pedidos.Api
```

### 2.3 Verificar

```bash
dotnet user-secrets list --project src/Pedidos.Api
```

Deben aparecer `ConnectionStrings:Default` y `Jwt:Key`. Si falta la clave del JWT, la API se niega a arrancar con un mensaje explícito.

### Configuración no sensible

`Issuer`, `Audience` y la expiración del token están en `src/Pedidos.Api/appsettings.json`:

```json
"Jwt": {
  "Issuer": "Pedidos.Api",
  "Audience": "Pedidos.Web",
  "ExpiracionMinutos": 60
}
```

### Orden de prioridad de la configuración

.NET combina las fuentes en este orden (la última gana):

1. `appsettings.json`
2. `appsettings.Development.json`
3. User Secrets (solo en entorno Development)
4. Variables de entorno (por ejemplo `ConnectionStrings__Default`, con doble guion bajo)
5. Argumentos de línea de comandos

---

## 3. Levantar el backend

Desde `backend/`:

```bash
dotnet tool restore
dotnet run --project src/Pedidos.Api --launch-profile http
```

Al arrancar, la API automáticamente:

1. **Aplica las migraciones** pendientes de EF Core (crea la base `PedidosDb` y sus tablas).
2. **Crea los usuarios de prueba** si la tabla de usuarios está vacía.

Swagger queda disponible en: **http://localhost:5228/swagger**

Para autenticarte en Swagger: ejecuta `POST /auth/login`, copia el `accessToken` de la respuesta, pulsa **Authorize** y pega solo el token (sin la palabra `Bearer`).

### Migraciones manuales (opcional)

Solo si necesitas crear una migración nueva:

```bash
dotnet ef migrations add NombreDescriptivo -p src/Pedidos.Infrastructure -s src/Pedidos.Api -o Persistence/Migrations
dotnet ef database update -p src/Pedidos.Infrastructure -s src/Pedidos.Api
```

---

## 4. Levantar el frontend

Desde `frontend/`, verifica que `.env.development` apunte al puerto de la API:

```env
VITE_API_URL=http://localhost:5228
```

Instala dependencias y arranca:

```bash
npm install
npm run dev
```

Abre **http://localhost:5173**

---

## Usuarios de prueba

| Rol | Email | Contraseña | Permisos |
|---|---|---|---|
| Admin | `admin@pedidos.com` | `Admin123!` | Ver, crear, editar, eliminar y cambiar estado |
| User | `user@pedidos.com` | `User123!` | Solo ver pedidos |

---

## Endpoints de la API

| Método | Ruta | Acceso | Descripción |
|---|---|---|---|
| POST | `/auth/login` | Público | Devuelve el JWT |
| GET | `/auth/me` | Autenticado | Datos del usuario del token |
| GET | `/api/pedidos` | Autenticado | Lista de pedidos |
| GET | `/api/pedidos/{id}` | Autenticado | Detalle de un pedido |
| POST | `/api/pedidos` | Admin | Crea un pedido |
| PUT | `/api/pedidos/{id}` | Admin | Edita cliente y total |
| PATCH | `/api/pedidos/{id}/estado` | Admin | Cambia el estado |
| DELETE | `/api/pedidos/{id}` | Admin | Elimina un pedido |

### Flujo de estados

```
Pendiente ──► EnProceso ──► Enviado ──► Entregado
    │             │
    └─────────────┴──► Cancelado
```

`Entregado` y `Cancelado` son estados finales: el pedido ya no se puede editar ni cambiar de estado.

### Códigos de respuesta

Todos los errores se devuelven en formato `ProblemDetails`.

| Código | Cuándo |
|---|---|
| 400 | Datos inválidos o regla de negocio incumplida |
| 401 | Sin token, token vencido o credenciales inválidas |
| 403 | Autenticado pero sin el rol necesario |
| 404 | El pedido no existe |
| 409 | Conflicto de datos (por ejemplo, número de pedido duplicado) |
| 429 | Se superó el límite de solicitudes |
| 500 | Error interno (sin exponer detalles) |

---

## Seguridad y resiliencia implementadas

**Seguridad**
- Autenticación con **JWT Bearer**: validación de emisor, audiencia, firma y expiración (tolerancia de reloj de 30 segundos).
- Autorización por **roles** (`Admin`, `User`).
- **Cerrado por defecto**: una política global exige autenticación en todos los endpoints; solo el login está marcado como público.
- Contraseñas almacenadas con **hash PBKDF2 con salt** (`PasswordHasher` de ASP.NET Core Identity).
- Mismo mensaje de error para usuario inexistente y contraseña incorrecta (evita enumeración de usuarios).
- Secrets fuera del repositorio (User Secrets).
- **CORS** restringido al origen del frontend.

**Resiliencia**
- **Rate limiting** nativo de .NET: login limitado a 5 intentos por minuto por IP; resto de la API a 100 solicitudes por minuto por usuario.
- **Reintentos automáticos** ante errores transitorios de SQL Server (`EnableRetryOnFailure` de EF Core).
- **Manejo global de excepciones** con `IExceptionHandler`, mapeando cada tipo de error a su código HTTP.
- `CancellationToken` propagado desde el controller hasta la base de datos.
- Validación de configuración al arranque (*fail fast*).

**Frontend**
- Token adjuntado automáticamente por un interceptor de Axios.
- Rutas protegidas por sesión y por rol.