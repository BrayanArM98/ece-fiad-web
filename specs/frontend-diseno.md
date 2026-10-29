# Frontend web: sistema de diseño

## A quién sirve y qué tiene que lograr

El frontend lo usan, varias horas al día, la recepción, los médicos y la administración de una clínica. Su trabajo principal es que cada persona encuentre en segundos lo que necesita: la agenda de hoy, el expediente de un paciente o el estado de la clínica. La interfaz debe sentirse como una herramienta clínica confiable: tranquila, legible y precisa, nunca decorativa.

## Principios

1. **El expediente es el centro.** La pantalla de un paciente se diseña como un expediente físico: un encabezado de identificación fijo, una banda de alertas siempre visible y pestañas como las de una carpeta para sus secciones. Es el único elemento con personalidad marcada; el resto de la interfaz es sobrio.
2. **Las alertas clínicas no se pierden.** Las alergias se muestran en una banda de color coral en el encabezado del paciente, como la pulsera de un hospital. Ese color se reserva solo para información clínica crítica y para errores.
3. **Jerarquía con líneas y espacio, no con sombras.** Los paneles se separan con bordes de 1 px y aire. No se usan degradados ni tarjetas con sombra como decoración.
4. **Los datos se leen en columna.** Horas, fechas y cifras usan números tabulares para que se alineen en tablas y en la agenda.
5. **Texto en minúsculas normales.** Botones y etiquetas en tipo oración, sin mayúsculas sostenidas. Cada botón dice exactamente lo que hace ("Agendar cita", "Registrar evolución").
6. **Cada rol ve solo lo suyo.** El menú y las acciones se arman a partir del rol de la sesión; lo que un rol no puede hacer no aparece.

## Paleta

| Nombre | Hex | Uso |
|--------|-----|-----|
| Marea | `#0E7490` | Color de la marca: acciones principales, elemento activo, enlaces. Es el mismo de la app móvil |
| Marea profunda | `#0B5568` | Estado presionado y texto sobre fondos claros de marea |
| Tinta | `#14262E` | Texto principal |
| Pizarra | `#5B6B73` | Texto secundario |
| Papel | `#F4F7F8` | Fondo de la aplicación |
| Línea | `#D9E3E7` | Bordes y divisores |
| Coral clínico | `#C2412D` | Alergias, errores y acciones destructivas |

Colores de estado de cita (siempre acompañados del texto del estado, nunca solo el color):

| Estado | Hex |
|--------|-----|
| Pendiente | `#B7791F` |
| Confirmada | `#0E7490` |
| Completada | `#2F855A` |
| No asistió | `#B83280` |
| Cancelada | `#718096` |

## Tipografía

- **Schibsted Grotesk** (variable) para títulos de página, nombre del paciente y cifras de los indicadores. Es una grotesca sólida y compacta, nacida en un periódico, que da carácter sin perder seriedad.
- **Public Sans** (variable) para todo el texto de la interfaz: tablas, formularios y menús. Es la tipografía del sistema de diseño del gobierno de Estados Unidos, hecha para formularios y datos.
- Ambas se sirven desde el propio proyecto (`@fontsource-variable`), sin depender de servicios externos.

Escala (rem): 0.75, 0.875, 1, 1.125, 1.375, 1.75 y 2.25. Cuerpo en 0.875 rem (14 px) con interlineado 1.5. Las cifras de tablas y agenda usan `font-variant-numeric: tabular-nums`.

## Forma y espaciado

- Radio de 6 px en controles (botones, campos) y de 10 px en paneles. Las insignias de estado son píldoras.
- Espaciado en múltiplos de 4 px.
- Contenido alineado a la izquierda, con un ancho máximo de 1280 px.
- Foco de teclado visible con un anillo de 2 px en color marea.
- Se respeta `prefers-reduced-motion`; las únicas animaciones son las que responden a una acción (abrir un diálogo, desplegar un menú).

## Estructura general

```text
┌────────────┬──────────────────────────────────────────────┐
│ ECE-FIAD   │  Título de la página          [acción principal]
│            │  ─────────────────────────────────────────────
│ Inicio     │                                              │
│ Agenda     │   Contenido                                  │
│ Pacientes  │                                              │
│ Doctores   │                                              │
│ Reportes   │                                              │
│ Usuarios   │                                              │
│            │                                              │
│ Dra. López │                                              │
│ Médico     │                                              │
└────────────┴──────────────────────────────────────────────┘
```

- Barra lateral fija con la navegación del rol y, abajo, el usuario de la sesión con la opción de cerrar sesión. En pantallas angostas se convierte en un menú desplegable.
- Cada página tiene un título y, a la derecha, su acción principal.

### Expediente del paciente

```text
  Juan Pérez Gómez                                   [Agendar cita]
  41 años   Masculino   O+   Expediente DEMO0001
  ████ Alergias: Penicilina ████████████████████████████████████
  ┌─────────┬──────────────────┬─────────────┬────────┐
  │ Datos   │ Historia clínica │ Evoluciones │ Citas  │
  └─────────┴──────────────────┴─────────────┴────────┘
  Contenido de la pestaña
```

### Agenda del día

```text
  08:00 ─┬─────────────────────────────────────────
  09:00  │▌ Juan Pérez      Consulta de control     Confirmada
  10:00  │▌ María Hernández Dolor de cabeza         Pendiente
  11:00  ┼─────────────────────────────────────────
```

Cada cita es un bloque con una barra lateral del color de su estado; la regla de horas a la izquierda usa números tabulares.

## Componentes

Componentes de **shadcn/ui** (Radix + Tailwind) con los colores y tipografías de este documento: botón, campo, selector, diálogo, hoja lateral, menú desplegable, tabla, pestañas, insignia, notificaciones (toast), esqueleto de carga y gráficas.

## Estados de cada pantalla

Toda pantalla que carga datos define cuatro estados: cargando (esqueleto con la forma del contenido), con datos, vacío (qué falta y la acción para crearlo, por ejemplo "Aún no hay citas para hoy" con "Agendar cita") y error (qué pasó y "Reintentar").
