# Frontend web: pantallas y navegación

## Rutas

| Ruta | Pantalla | Roles |
|------|----------|-------|
| `/login` | Inicio de sesión | Todos (sin sesión) |
| `/` | Inicio: indicadores del mes y agenda de hoy | Administrador, Recepcionista, Médico |
| `/agenda` | Agenda por día y semana; agendar, editar, cancelar y registrar el resultado | Administrador, Recepcionista, Médico (solo la suya) |
| `/pacientes` | Lista de pacientes con búsqueda | Administrador, Recepcionista, Médico |
| `/pacientes/:id` | Expediente: datos, historia clínica, evoluciones y citas | Administrador, Médico (Recepcionista solo datos y citas) |
| `/doctores` | Doctores y especialidades | Administrador (los demás solo consultan) |
| `/reportes` | Los cinco reportes con filtros, gráficas y descarga | Administrador; Recepcionista solo citas; Médico solo los suyos |
| `/usuarios` | Cuentas de acceso | Administrador |

Un paciente no usa el frontend web: si inicia sesión, se le indica que use la app móvil.

## Reglas de navegación

1. Sin sesión, cualquier ruta lleva a `/login`; después de entrar se regresa a la ruta que se pidió.
2. El token se guarda en `sessionStorage`: al cerrar el navegador la sesión termina, que es lo adecuado para una computadora compartida en recepción.
3. Si la API responde 401 (sesión vencida), se cierra la sesión y se lleva a `/login` con el mensaje "Tu sesión expiró. Inicia sesión de nuevo."
4. Si responde 403, se muestra "No tienes permiso para realizar esta acción." sin cerrar la sesión.
5. El menú lateral solo muestra las rutas del rol.

## Pantallas

### Inicio de sesión
Correo, contraseña y "Entrar". El botón se deshabilita mientras faltan datos o mientras se envía. Muestra el mensaje de la API ("Correo o contraseña incorrectos", "Demasiados intentos...").

### Inicio
- Franja de indicadores del mes (citas, asistencia, cancelaciones, pacientes atendidos, evoluciones y, para el administrador, pacientes nuevos) con su variación contra el mes anterior.
- Agenda de hoy: la del médico en sesión, o la de toda la clínica para administración y recepción.

### Agenda
- Vista de día (regla de horas) y de semana; selector de fecha y filtro por doctor (el médico solo ve la suya).
- "Agendar cita" abre un diálogo con paciente, doctor, fecha y hora, motivo y notas.
- Al abrir una cita: editar, cancelar (con confirmación) y registrar el resultado (Confirmada, Completada, No asistió).

### Pacientes
- Tabla con nombre, documento, edad, teléfono y estado; búsqueda por nombre o documento; paginación en el cliente.
- "Registrar paciente" abre un diálogo con los datos del paciente.

### Expediente del paciente
- Encabezado fijo con nombre, edad, género, grupo sanguíneo, número de expediente y la banda de alergias.
- Pestañas: Datos (editar), Historia clínica (abrirla si no existe; editar antecedentes y alergias), Evoluciones (lista de la más reciente a la más antigua; "Registrar evolución") y Citas.

### Doctores
- Tabla de doctores con especialidad y horario; pestaña de especialidades. Altas, ediciones y bajas solo para el administrador.

### Reportes
- Filtros comunes arriba (periodo, doctor, especialidad) y pestañas por reporte.
- Gráficas: líneas para las citas en el tiempo, barras para los conteos y tabla para la productividad.
- "Descargar PDF" y "Descargar Excel" con los mismos filtros.

### Usuarios
- Tabla de cuentas con rol, vínculo (doctor o paciente), estado y último acceso; crear, editar, desactivar y eliminar.

## Criterios de aceptación generales

- [ ] Cada rol ve en el menú exactamente las rutas de la tabla.
- [ ] Abrir una ruta sin sesión lleva a `/login` y después de entrar regresa a esa ruta.
- [ ] Una sesión vencida lleva a `/login` con el mensaje de sesión expirada.
- [ ] Toda lista tiene estados de carga, vacío y error.
- [ ] Todas las acciones se pueden hacer solo con el teclado y el foco siempre es visible.
- [ ] La interfaz funciona desde 360 px de ancho.

## Tasks

- [x] Proyecto con Vite, React, TypeScript y Tailwind; alias `@`; tipografías y paleta.
- [x] Componentes de shadcn/ui.
- [x] Cliente de la API con el token y el manejo de 401 y 403.
- [x] Sesión, inicio de sesión y rutas protegidas por rol.
- [x] Estructura con barra lateral.
- [x] Inicio.
- [ ] Agenda.
- [ ] Pacientes y expediente.
- [ ] Doctores y especialidades.
- [ ] Reportes.
- [ ] Usuarios.
- [ ] Despliegue en Vercel apuntando a la API publicada.
