# Módulo de Reportes

## Objetivo

Que la dirección de la clínica y los médicos vean cómo va la operación (citas, asistencia, productividad, pacientes y diagnósticos) en un periodo elegido, y puedan descargar esa información en PDF o Excel para compartirla.

## Actores y permisos

| Reporte | Administrador | Recepcionista | Médico | Paciente |
|---------|:-------------:|:-------------:|:------:|:--------:|
| Resumen general | Sí | — | Solo sus datos | — |
| Citas | Sí | Sí | Solo sus citas | — |
| Productividad médica | Sí | — | Solo él | — |
| Pacientes | Sí | — | — | — |
| Diagnósticos frecuentes | Sí | — | Solo sus evoluciones | — |

Para un médico, la API fija su doctor en el reporte aunque envíe otro `idDoctor`.

## Filtros comunes

Todos los reportes aceptan los mismos parámetros de consulta, todos opcionales:

| Parámetro | Regla |
|-----------|-------|
| `desde` | Fecha inicial, inclusiva |
| `hasta` | Fecha final, inclusiva (incluye todo el día) |
| `idDoctor` | Mayor que 0 |
| `idEspecialidad` | Mayor que 0 |
| `estado` | `Pendiente`, `Confirmada`, `Cancelada`, `Completada` o `NoAsistio` (solo aplica al reporte de citas) |

Periodo por defecto:
- Sin fechas: el mes actual completo.
- Solo `desde`: hasta hoy (o ese mismo día si `desde` es futuro).
- Solo `hasta`: desde el primer día de ese mes.
- El periodo no puede superar 366 días y `desde` no puede ser posterior a `hasta`.

## Reportes

### 1. Resumen general — `GET /api/Reportes/resumen`

Indicadores del periodo comparados con el periodo anterior de la misma duración (por ejemplo, octubre contra septiembre):

| Indicador | Cálculo |
|-----------|---------|
| Citas | Total de citas del periodo |
| Asistencia | Completadas ÷ (completadas + inasistencias) × 100 |
| Cancelaciones | Citas canceladas |
| Pacientes atendidos | Pacientes distintos con al menos una cita completada |
| Evoluciones registradas | Evoluciones con fecha en el periodo |
| Pacientes nuevos | Pacientes registrados en el periodo (solo en el resumen de toda la clínica) |

`variacion` es el cambio porcentual contra el periodo anterior; en el indicador de asistencia es la diferencia en puntos. Es `null` si el periodo anterior fue 0.

### 2. Citas — `GET /api/Reportes/citas`

- Total de citas.
- Conteo y porcentaje por estado (los cinco estados, aunque alguno tenga 0).
- Serie en el tiempo: por día si el periodo es de hasta 31 días y por semana si es mayor, incluyendo días o semanas sin citas. Cada punto separa completadas, inasistencias, canceladas y por atender (pendientes y confirmadas).
- Conteo por doctor y por especialidad, de mayor a menor.

### 3. Productividad médica — `GET /api/Reportes/productividad`

Una fila por doctor activo (incluidos los que no tuvieron citas): citas totales, completadas, inasistencias, canceladas, por atender, evoluciones registradas y tasa de asistencia. Ordenado por consultas completadas.

### 4. Pacientes — `GET /api/Reportes/pacientes`

- Pacientes activos y pacientes nuevos en el periodo.
- Reparto de los pacientes activos por género, por rango de edad (0–11, 12–17, 18–29, 30–44, 45–59, 60 o más) y por grupo sanguíneo.

### 5. Diagnósticos frecuentes — `GET /api/Reportes/diagnosticos`

Los 10 diagnósticos más frecuentes en las evoluciones del periodo. Como el diagnóstico se escribe a mano, se agrupan sin distinguir mayúsculas, espacios repetidos ni punto final, y se muestra la forma escrita con más frecuencia.

## Exportación

Cada reporte tiene su versión descargable con los mismos filtros:

```text
GET /api/Reportes/{resumen|citas|productividad|pacientes|diagnosticos}/exportar?formato=Pdf|Excel
```

- **PDF:** encabezado de ECE-FIAD con el título, el periodo, los filtros aplicados y la fecha de generación; tarjetas con los indicadores y una tabla por sección; número de página.
- **Excel:** hoja "Resumen" con encabezado e indicadores y una hoja por tabla; los números se guardan como números para poder sumarlos o graficarlos.
- Nombre del archivo: `ece-fiad-{reporte}-{desde}-{hasta}.pdf|xlsx`.

## Casos de error

- Filtros inválidos: 400 con la lista `errores`.
- Sin sesión: 401. Rol sin permiso para ese reporte: 403.
- Periodo sin datos: el reporte responde normalmente con ceros y listas vacías; el PDF muestra "Sin datos en el periodo".

## Criterios de aceptación

- [ ] Sin filtros, cada reporte cubre el mes actual.
- [ ] El resumen compara contra el periodo anterior de la misma duración.
- [ ] La tasa de asistencia excluye las citas canceladas y las que aún no tienen resultado.
- [ ] El reporte de citas muestra los cinco estados aunque alguno tenga 0.
- [ ] Un periodo de 31 días o menos agrupa por día y uno mayor agrupa por semana.
- [ ] La productividad incluye a los doctores sin citas en el periodo.
- [ ] "Gripe", "gripe." y "GRIPE" cuentan como el mismo diagnóstico.
- [ ] Un médico solo ve sus datos aunque envíe el `idDoctor` de otro doctor.
- [ ] La recepción recibe 403 en todos los reportes excepto el de citas.
- [ ] El PDF y el Excel muestran los mismos datos que la consulta con los mismos filtros.
- [ ] El periodo y los filtros aplicados aparecen en el encabezado del archivo.

## Decisiones

- Los totales se calculan en la capa de aplicación a partir de filas planas que filtra la base de datos (`IConsultasReportes`). Así las reglas de cálculo quedan en un solo lugar y se pueden probar sin base de datos.
- Los archivos los genera la API para que el frontend web y la app móvil descarguen exactamente lo mismo.
- PDF con **QuestPDF** (licencia Community, gratuita para uso académico) y Excel con **ClosedXML** (licencia MIT).
- Exportar reutiliza la query de cada reporte: las reglas de filtros y de rol son las mismas en la consulta y en el archivo.
- La fecha de alta de un paciente se guarda en UTC; en el reporte de pacientes nuevos puede haber una diferencia de horas en los extremos del periodo.

## Tasks

- [x] DTOs de filtros, reportes, filas y documento exportable.
- [x] Validación de filtros.
- [x] `IConsultasReportes` y su implementación con proyecciones de EF Core.
- [x] Reglas de periodo, alcance por rol y cálculos (`Periodos`, `AlcanceReporte`, `CalculadoraReportes`).
- [x] Queries y handlers de los cinco reportes.
- [x] Exportación a PDF y Excel (`IExportadorReportes`, `ExportarReporteQuery`).
- [x] `ReportesController` con permisos por rol.
- [ ] Pantalla de Reportes en el frontend web.
- [ ] Reportes del médico en la app móvil.
- [ ] Validar los criterios de aceptación.
