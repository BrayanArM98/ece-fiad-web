# Despliegue

**Estado:** Implementada

ECE-FIAD Web se publica en dos partes:

| Parte | Dónde | Cómo se actualiza |
|-------|-------|-------------------|
| Frontend (React) | Vercel | Automático al subir a `main` |
| API (.NET 10) | Azure App Service (Linux, plan F1 gratuito) | GitHub Actions al subir cambios de `backend/` a `main` |
| Base de datos | Azure SQL Database (oferta gratuita) | La API aplica las migraciones al arrancar |

La app móvil usa la misma API publicada.

**Publicación actual**

| Recurso | Valor |
|---------|-------|
| Frontend | https://ece-fiad-web.vercel.app |
| API | https://ece-fiad-api-cpcce9f2d0hffmeq.brazilsouth-01.azurewebsites.net (documentación en `/swagger`) |
| Grupo de recursos | `rg-ece-fiad`, región Brazil South |
| Base de datos | `ece-fiad` en el servidor `ece-fiad-sql-brayan` (oferta gratuita, pausa al llegar al límite) |
| App Service | `ece-fiad-api` en el plan `plan-ece-fiad` (F1, Linux, .NET 10) |

Azure for Students solo permite crear recursos en algunas regiones; East US, West US 2 y West US 3 fueron rechazadas por la política de la suscripción, Brazil South sí está permitida.

## 1. Requisitos

- Una suscripción de Azure (Azure for Students sirve; la versión "Starter" no permite la base gratuita).
- Una cuenta de Vercel conectada a GitHub.
- Permiso de administrador en el repositorio de GitHub para crear secretos.

## 2. Base de datos en Azure SQL

1. Entra a [aka.ms/azuresqlhub](https://aka.ms/azuresqlhub) y en **Create a database** elige **Start free**.
2. En **Basics**:
   - Grupo de recursos: crea `rg-ece-fiad`.
   - Nombre de la base: `ece-fiad`.
   - Servidor: crea uno nuevo (por ejemplo `ece-fiad-sql-<tu-usuario>`), con **SQL authentication**. Guarda el usuario y la contraseña del administrador del servidor.
   - Comportamiento al llegar al límite gratuito: **Auto-pause the database until next month** (así nunca genera cobros).
3. En **Networking**: **Allow Azure services and resources to access this server = Yes**. Si quieres consultar la base desde tu computadora, agrega también tu IP.
4. Confirma que el costo estimado sea $0 y crea la base.
5. En la base, ve a **Connection strings → ADO.NET (SQL authentication)** y copia la cadena. Reemplaza `{your_password}` por la contraseña real.

La base gratuita se pausa sola tras 15 minutos sin uso y tarda unos segundos en despertar. La API reintenta la conexión (`EnableRetryOnFailure` en `Program.cs`), así que la primera petición solo tarda más.

## 3. API en Azure App Service

1. En el portal, **Create a resource → Web App**:
   - Grupo de recursos: `rg-ece-fiad`.
   - Nombre: `ece-fiad-api` o el que esté libre. La API queda en `https://<nombre>.azurewebsites.net`.
   - Publish: **Code**. Runtime stack: **.NET 10 (LTS)**. Operating System: **Linux**.
   - Plan: **Free F1**.
2. En la Web App, **Settings → Environment variables → App settings**, agrega:

   | Nombre | Valor |
   |--------|-------|
   | `Jwt__Clave` | Una clave nueva de al menos 32 caracteres (ver abajo). No reutilices la de desarrollo. |
   | `Cors__OrigenesPermitidos__0` | La URL del frontend en Vercel, sin `/` al final. Ejemplo: `https://ece-fiad-web.vercel.app` |
   | `BaseDeDatos__AplicarMigraciones` | `true` |
   | `AdministradorInicial__Email` | `admin@ecefiad.mx` |
   | `AdministradorInicial__Contrasena` | La contraseña del administrador en producción |
   | `AdministradorInicial__NombreCompleto` | `Administrador ECE-FIAD` |
   | `DatosDemostracion__Habilitar` | `true` para la demostración, `false` para una clínica real |
   | `DatosDemostracion__Contrasena` | Contraseña de recepcion@, medico@ y paciente@ecefiad.mx |
   | `Swagger__Habilitar` | `true` para mostrar la documentación en `/swagger` |
   | `TZ` | `America/Tijuana` (las citas se guardan en hora local de la clínica) |
   | `ASPNETCORE_FORWARDEDHEADERS_ENABLED` | `true` (para que el límite de intentos de inicio de sesión vea la IP real) |

   En la pestaña **Connection strings** agrega `ConexionBD` con la cadena del paso 2.5 y tipo **SQLAzure**.

   Para generar la clave de los tokens, en PowerShell:

   ```powershell
   $b = New-Object byte[] 48; [Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($b); [Convert]::ToBase64String($b)
   ```

3. En **Settings → Configuration → General settings**:
   - **SCM Basic Auth Publishing Credentials: On** (lo necesita el perfil de publicación de GitHub Actions).
   - **HTTPS Only: On**.
   - **Startup Command:** `dotnet Presentacion.dll`
4. Guarda los cambios.

## 4. Publicación automática con GitHub Actions

1. En la Web App, **Overview → Download publish profile**.
2. En GitHub, **Settings → Secrets and variables → Actions → New repository secret**:
   - Nombre: `AZURE_WEBAPP_PUBLISH_PROFILE`
   - Valor: todo el contenido del archivo descargado.
3. Si el nombre de tu Web App no es `ece-fiad-api`, cámbialo en `AZURE_WEBAPP_NAME` dentro de `.github/workflows/api-azure.yml`.
4. Sube un cambio a `main` o lanza el flujo desde **Actions → API a Azure → Run workflow**.

Al terminar, `https://<nombre>.azurewebsites.net/salud` responde `{"estado":"en línea"}`. El primer arranque tarda más porque crea las tablas, el administrador y los datos de demostración.

## 5. Frontend en Vercel

1. En Vercel, importa el repositorio (o abre el proyecto existente) con:
   - **Root Directory:** `frontend`
   - **Framework Preset:** Vite
2. En **Settings → Environment Variables** agrega `VITE_API_URL` con la URL de la API (`https://<nombre>.azurewebsites.net`, sin `/` al final) para **Production**.
3. Vuelve a desplegar (**Deployments → Redeploy**). La variable se lee al compilar, así que cualquier cambio en ella necesita un nuevo despliegue.
4. Copia la URL final del proyecto y ponla en `Cors__OrigenesPermitidos__0` de la API.

`frontend/vercel.json` redirige todas las rutas a `index.html`, así que recargar `/agenda` o `/pacientes/5` funciona.

## 6. App móvil

En `ece-fiad-movil`, `EXPO_PUBLIC_API_URL` debe apuntar a la misma URL de la API. La app móvil no necesita estar en la lista de CORS: esa restricción solo aplica a navegadores.

## 7. Limitaciones del plan gratuito

- **App Service F1:** 60 minutos de CPU al día y la app se duerme tras 20 minutos sin peticiones. La primera petición después de dormir tarda entre 20 y 40 segundos.
- **Azure SQL gratuito:** 100,000 segundos de vCore y 32 GB al mes. Con pausa automática al llegar al límite, la base deja de responder hasta el mes siguiente en lugar de cobrar.
- **Antes de una presentación:** abre la página e inicia sesión unos minutos antes para despertar la API y la base.

## Criterios de aceptación

- [x] `GET /salud` en la URL de Azure responde 200 con `{"estado":"en línea"}`.
- [ ] El frontend publicado en Vercel inicia sesión con admin@ecefiad.mx y carga Inicio sin errores de CORS.
- [ ] Recargar el navegador en `/agenda` o en un expediente muestra la misma pantalla, no un 404 de Vercel.
- [ ] Un origen que no está en `Cors__OrigenesPermitidos` no puede llamar a la API desde el navegador.
- [ ] Las citas creadas desde el frontend publicado aparecen con la misma hora en la agenda (zona horaria `TZ`).
- [ ] Los reportes se descargan en PDF y Excel desde el frontend publicado.
- [x] Un cambio en `backend/` subido a `main` se publica solo mediante GitHub Actions.
- [x] `appsettings.Development.json` y las claves de producción no están en el repositorio.

## Tareas

- [x] Reintentos de conexión a SQL para la base gratuita de Azure (`Program.cs`).
- [x] Flujo de GitHub Actions para publicar la API (`.github/workflows/api-azure.yml`).
- [x] Carga diferida de las pantallas del frontend para reducir el tamaño inicial.
- [x] Crear la base de datos en Azure SQL.
- [x] Crear el App Service y configurar sus variables.
- [x] Configurar el secreto del perfil de publicación y publicar la API.
- [x] Configurar `VITE_API_URL` en Vercel y volver a desplegar.
- [ ] Apuntar la app móvil a la API publicada.
- [ ] Validar los criterios de aceptación.
