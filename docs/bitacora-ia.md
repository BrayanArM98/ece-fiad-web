# Bitácora de IA

**Proyecto:** ECE-FIAD Web · **Autor:** Brayan Arturo Rocha Meneses
**Herramienta:** Claude (en claude.ai y como asistente con acceso al proyecto)

## Cómo trabajé con la IA

1. Antes de programar un módulo escribía su especificación en `specs/` (pantallas, diseño, reportes, despliegue).
2. Le pedía a la IA un módulo a la vez, señalando la spec y la capa donde debía ir el cambio.
3. Revisaba lo que proponía, compilaba la API, probaba en Swagger y en el navegador, y revisaba capturas de cada pantalla en escritorio y en celular.
4. Hacía un commit por cada paso pequeño; el historial muestra la migración módulo por módulo.

## Registro

### 2026-09-13 · Capas del backend y migración a CQRS
- **Pedí:** separar la API en Dominio, Aplicación, Infraestructura y Presentación, y migrar los servicios al patrón CQRS con MediatR.
- **Decidí:** migrar un módulo a la vez (pacientes, especialidades, doctores, citas, historias, evoluciones) y borrar el servicio viejo solo cuando el módulo nuevo funcionara.
- **Corregí:** se quitaron restos de Blazor que no se usaban en el proyecto de la API.

### 2026-10-29 · Seguridad y calidad de la API
- **Problema detectado:** AutoMapper y una dependencia indirecta de SqlClient tenían vulnerabilidades reportadas. Se actualizaron y se fijó la versión corregida.
- **Acepté:** un manejador global de excepciones y respuestas de validación unificadas, en lugar de `try/catch` repetidos en cada handler.
- **Pedí:** inicio de sesión con JWT y roles. **Verifiqué** que el médico solo ve sus citas y que el paciente solo ve las suyas, aunque pida otras por la URL.
- **Error detectado:** la API devolvía fechas con 7 decimales en los segundos y el navegador las leía mal; se corrigió la lectura y se agregó una pantalla de error.

### 2026-11-04 · Pantallas del frontend
- **Revisé con capturas** cada pantalla en escritorio y en tamaño de celular. Errores que encontré y se corrigieron:
  - La leyenda de la gráfica de citas no mostraba el patrón rayado de las canceladas; se hizo una leyenda propia.
  - Las pestañas se salían de la pantalla en el celular.
  - El selector de rol mostraba la descripción dentro del campo; se movió como texto de ayuda.
  - Los selectores no respetaban la altura de 36 px del diseño; se corrigió en el componente base.

### 2026-11-05 · Despliegue en Azure y Vercel
- **Problema:** Azure no permitió crear la base de datos en las regiones de Estados Unidos por la política de la suscripción de estudiante. Se creó en Brazil South.
- **Rechacé:** el formulario de App Service traía preseleccionado un plan B1 que ya existía (unos 14.60 dólares al mes). Se creó un plan gratuito F1.
- **Problema:** la primera ejecución de GitHub Actions falló con "No credentials found" porque faltaba el secreto del perfil de publicación. Lo agregué y la siguiente ejecución publicó la API.
- **Problema:** la base gratuita de Azure se pausa sola; la primera conexión fallaba. Se agregaron reintentos de conexión (`EnableRetryOnFailure` en `Program.cs`).
- **Mejora:** las pantallas se cargan bajo demanda; el archivo principal del frontend bajó a 282 kB.
- **Regla que mantuve:** las contraseñas, la clave del JWT y la cadena de conexión las escribí yo en Azure; no están en el repositorio ni las tecleó la IA.

## Lo que aprendí sobre trabajar con IA
- Revisar capturas en tamaño celular encontró errores que en escritorio no se veían.
- Revisar los costos: la IA detectó a tiempo que el plan preseleccionado cobraba.
- Pedir cambios por capa (por ejemplo "solo el handler") evita que toque archivos que no corresponden.
