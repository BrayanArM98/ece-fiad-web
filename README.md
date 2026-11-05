# ECE-FIAD Web

Sistema de Expediente Clínico Electrónico para clínicas: agenda de citas, pacientes con su historia clínica y evoluciones, doctores, reportes con exportación a PDF y Excel, y cuentas por rol.

La misma API la consume la app móvil [ece-fiad-movil](https://github.com/BrayanArM98/ece-fiad-movil).

## Tecnologías

| Parte | Tecnologías |
|-------|-------------|
| API (`backend/`) | .NET 10, ASP.NET Core, CQRS con MediatR, FluentValidation, AutoMapper, Entity Framework Core, SQL Server, JWT, QuestPDF y ClosedXML |
| Frontend (`frontend/`) | React 19, TypeScript, Vite, Tailwind CSS, shadcn/ui, TanStack Query, React Router y Recharts |
| Despliegue | Vercel (frontend), Azure App Service y Azure SQL (API), GitHub Actions |

La API sigue una arquitectura en cuatro capas: `Dominio`, `Aplicacion` (comandos, consultas y validaciones), `Infraestructura` (EF Core, repositorios y exportación de reportes) y `API` (controladores y seguridad).

## Roles

| Rol | Puede |
|-----|-------|
| Administrador | Todo, incluidos usuarios, doctores y especialidades |
| Recepcionista | Agenda, pacientes y reporte de citas |
| Médico | Su agenda, expedientes clínicos, evoluciones y sus reportes |
| Paciente | Solo la app móvil, con sus propias citas |

## Ejecutar en local

Requisitos: .NET 10 SDK, Node.js 22 o superior y SQL Server LocalDB.

1. Crea `backend/API/appsettings.Development.json` (no se sube al repositorio) con al menos:

   ```json
   {
     "Jwt": { "Clave": "una-clave-de-al-menos-32-caracteres" },
     "AdministradorInicial": {
       "Email": "admin@ecefiad.mx",
       "Contrasena": "Admin123",
       "NombreCompleto": "Administrador ECE-FIAD"
     },
     "BaseDeDatos": { "AplicarMigraciones": true },
     "DatosDemostracion": { "Habilitar": true, "Contrasena": "Demo1234" }
   }
   ```

2. Levanta la API:

   ```bash
   cd backend/API
   dotnet run --launch-profile http
   ```

   La primera vez crea la base, el administrador y los datos de demostración. La documentación queda en http://localhost:5200/swagger.

3. En otra terminal, levanta el frontend:

   ```bash
   cd frontend
   cp .env.example .env.local
   npm install
   npm run dev
   ```

   Abre http://localhost:5173.

Con los datos de demostración se crean recepcion@, medico@ y paciente@ecefiad.mx con la contraseña de `DatosDemostracion:Contrasena`.

## Despliegue

Los pasos para publicar en Vercel y Azure están en [`specs/despliegue.md`](specs/despliegue.md).

## Especificaciones

El proyecto se desarrolla con Spec-Driven Development. Las especificaciones de cada módulo están en [`specs/`](specs/README.md).
