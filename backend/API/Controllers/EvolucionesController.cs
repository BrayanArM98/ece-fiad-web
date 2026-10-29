using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Features.Evoluciones.Commands.ActualizarEvolucion;
using Aplicacion.Features.Evoluciones.Commands.CrearEvolucion;
using Aplicacion.Features.Evoluciones.Commands.EliminarEvolucion;
using Aplicacion.Features.Evoluciones.Queries.ContarEvolucionesPorPaciente;
using Aplicacion.Features.Evoluciones.Queries.ObtenerEvolucionesPorHistoria;
using Aplicacion.Features.Evoluciones.Queries.ObtenerEvolucionPorId;
using Aplicacion.Features.Evoluciones.Queries.ObtenerHistorialPaciente;
using Aplicacion.Features.Evoluciones.Queries.ObtenerTodasEvoluciones;
using Aplicacion.Seguridad;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers;

/// <summary>
/// Controlador REST para la gestión de evoluciones clínicas.
/// Cada evolución pertenece a una historia clínica y registra los avances
/// de un paciente: diagnóstico, tratamiento y notas en una fecha específica.
/// Todas las operaciones se resuelven mediante el patrón CQRS a través de MediatR.
/// Acceso: administrador y médico (la recepción no ve información clínica).
/// </summary>
[Authorize(Roles = Roles.AdministradorOMedico)]
public class EvolucionesController : ControladorBase
{
    private readonly IMediator _mediator;

    public EvolucionesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene la lista completa de evoluciones con sus relaciones.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var resultado = await _mediator.Send(new ObtenerTodasEvolucionesQuery());
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene una evolución específica por su ID.
    /// </summary>
    /// <param name="id">Identificador de la evolución.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var resultado = await _mediator.Send(new ObtenerEvolucionPorIdQuery(id));
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene todas las evoluciones de una historia clínica específica.
    /// </summary>
    /// <param name="idHistoria">ID de la historia clínica.</param>
    [HttpGet("historia/{idHistoria:int}")]
    public async Task<IActionResult> ObtenerPorHistoria(int idHistoria)
    {
        var resultado = await _mediator.Send(new ObtenerEvolucionesPorHistoriaQuery(idHistoria));
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Cuenta cuántas evoluciones activas tiene un paciente (regla 29).
    /// </summary>
    /// <param name="idPaciente">ID del paciente.</param>
    [HttpGet("paciente/{idPaciente:int}/contar")]
    public async Task<IActionResult> ContarPorPaciente(int idPaciente)
    {
        var resultado = await _mediator.Send(new ContarEvolucionesPorPacienteQuery(idPaciente));
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene el historial clínico completo de un paciente: su historia clínica
    /// junto con todas sus evoluciones ordenadas por fecha descendente.
    /// Es el endpoint que consume el módulo de Historial del frontend.
    /// </summary>
    /// <param name="idPaciente">ID del paciente.</param>
    [HttpGet("/api/historial/paciente/{idPaciente:int}")]
    public async Task<IActionResult> ObtenerHistorialPorPaciente(int idPaciente)
    {
        var resultado = await _mediator.Send(new ObtenerHistorialPacienteQuery(idPaciente));
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Crea una nueva evolución.
    /// </summary>
    /// <param name="dto">Datos de la evolución a crear.</param>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearEvolucionDTO dto)
    {
        if (dto == null)
            return BadRequest(new { exitoso = false, mensaje = "El cuerpo de la petición es requerido." });

        var resultado = await _mediator.Send(new CrearEvolucionCommand(dto));
        return MapearCreacion(resultado);
    }

    /// <summary>
    /// Actualiza una evolución existente.
    /// </summary>
    /// <param name="id">ID de la evolución a actualizar.</param>
    /// <param name="dto">Datos actualizados.</param>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarEvolucionDTO dto)
    {
        if (dto == null)
            return BadRequest(new { exitoso = false, mensaje = "El cuerpo de la petición es requerido." });

        if (id != dto.Id)
            return BadRequest(new
            {
                exitoso = false,
                mensaje = "El ID de la ruta no coincide con el ID del cuerpo."
            });

        var resultado = await _mediator.Send(new ActualizarEvolucionCommand(dto));
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Elimina una evolución (borrado lógico).
    /// </summary>
    /// <param name="id">ID de la evolución a eliminar.</param>
    [Authorize(Roles = Roles.Administrador)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _mediator.Send(new EliminarEvolucionCommand(id));
        return MapearResultado(resultado);
    }
}