# Diario de decisiones

**Proyecto:** ECE-FIAD Web · **Autor:** Brayan Arturo Rocha Meneses

Cada decisión indica qué se eligió, qué alternativas había y por qué.

### 1. Clean Architecture en cuatro capas
- **Alternativa:** un solo proyecto con controladores que acceden directo a la base.
- **Por qué:** cada capa tiene una responsabilidad. El Dominio no depende de nada, y la Aplicación solo conoce interfaces (`Aplicacion/Abstracciones/`), que Infraestructura implementa.

### 2. CQRS con MediatR en lugar de servicios
- **Alternativa:** una clase de servicio por módulo con todos los métodos.
- **Por qué:** cada caso de uso es un Command o Query con su Handler (`Aplicacion/Features/`). Los archivos son pequeños y fáciles de encontrar por nombre.

### 3. Repository y Unit of Work
- **Por qué:** los handlers no usan el `DbContext` directamente; guardan todos los cambios de una operación en una sola llamada (`GuardarCambiosAsync`).

### 4. Entity Framework Core Code First con Fluent API
- **Alternativa:** anotaciones en las entidades, o SQL a mano.
- **Por qué:** las entidades quedan limpias y el mapeo a tablas vive en `Infraestructura/Data/Configuraciones/`. Las migraciones crean y actualizan la base.

### 5. Borrado lógico
- **Por qué:** en un expediente clínico no se debe perder información. `EntidadBase` tiene `Eliminado` y `Activo`; los repositorios filtran los registros eliminados.

### 6. Validaciones con FluentValidation en la capa de Aplicación
- **Por qué:** las reglas viven en un solo lugar (`Aplicacion/Validaciones/`) y aplican igual para la web y la app móvil.

### 7. JWT con roles y reglas por rol dentro de los handlers
- **Por qué:** además de proteger los endpoints, cada handler revisa el usuario del token. Así un médico no puede ver citas de otro médico cambiando el id en la URL.

### 8. Límite de intentos de inicio de sesión
- **Por qué:** evita ataques de fuerza bruta; la API responde 429 después de varios intentos por minuto.

### 9. React con TypeScript, TanStack Query y shadcn/ui
- **Por qué:** TanStack Query maneja carga, errores y caché de las peticiones; shadcn/ui da componentes accesibles que adapté al diseño "Marea".

### 10. Reportes calculados en el servidor y exportados a PDF y Excel
- **Por qué:** los datos se consultan y se calculan en la API, no en el navegador; QuestPDF y ClosedXML generan los archivos en el servidor.

### 11. Despliegue: Vercel para el frontend y Azure para la API y la base
- **Por qué:** los dos tienen plan gratuito. GitHub Actions publica la API automáticamente al subir cambios del backend.
- **Consecuencia:** el plan gratuito duerme la API; antes de presentar hay que abrirla unos minutos antes.

### 12. Secretos fuera del repositorio
- **Por qué:** `appsettings.Development.json` está en `.gitignore`, y en producción la clave del JWT y la cadena de conexión están en la configuración de Azure.
