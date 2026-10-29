# Especificaciones del proyecto (SDD)

ECE-FIAD Web se desarrolla con **Spec-Driven Development**: cada módulo nuevo se especifica aquí antes de implementarse, y la especificación se actualiza si la implementación obliga a cambiar el comportamiento.

## Índice

| Spec | Contenido | Estado |
|------|-----------|--------|
| [`reportes.md`](reportes.md) | Módulo de Reportes: indicadores, gráficas y exportación a PDF y Excel | Implementada (API) |
| [`frontend-diseno.md`](frontend-diseno.md) | Sistema de diseño del frontend: principios, paleta, tipografía y estructura | Especificada |
| [`frontend-pantallas.md`](frontend-pantallas.md) | Rutas, pantallas por rol y reglas de navegación del frontend | En desarrollo |

Estados: Especificada → En desarrollo → Implementada → Validada. Una spec pasa a **Validada** cuando todos sus criterios de aceptación están marcados.

## Estructura de cada spec

1. Objetivo y actores.
2. Comportamiento (qué calcula o qué hace).
3. Contrato de la API (endpoints, parámetros y respuesta).
4. Reglas y permisos.
5. Casos de error.
6. Criterios de aceptación (casillas verificables).
7. Decisiones técnicas.
8. Tasks.

## Arquitectura de referencia

- **Backend:** ASP.NET Core 10 en cuatro capas (Dominio, Aplicación, Infraestructura y Presentación) con CQRS mediante MediatR. Cada operación es un Command o una Query con su Handler; las validaciones usan FluentValidation y el acceso a datos pasa por repositorios y Unit of Work.
- **Seguridad:** JWT con cuatro roles (Administrador, Recepcionista, Médico y Paciente). Las reglas que dependen del usuario se aplican en los handlers, no en el cliente.
- **Respuestas:** todas con la forma `{ exitoso, mensaje, datos }` o `{ exitoso, mensaje, errores }`.
- **Clientes:** el frontend web (React) y la app móvil (React Native) consumen la misma API.
