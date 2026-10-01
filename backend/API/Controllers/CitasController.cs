using Aplicacion.DTOs.Citas;
using Aplicacion.Features.Citas.Commands.ActualizarCita;
using Aplicacion.Features.Citas.Commands.CancelarCita;
using Aplicacion.Features.Citas.Commands.CrearCita;
using Aplicacion.Features.Citas.Commands.EliminarCita;
using Aplicacion.Features.Citas.Queries.ObtenerCitaPorId;
using Aplicacion.Features.Citas.Queries.ObtenerTodasCitas;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers;

/// <summary>
/// Controlador REST para la gestión de citas médicas.
/// Cada cita asocia a un paciente con un doctor en una fecha y hora específicas.
/// Todas las operaciones se resuelven mediante el patrón CQRS a través de MediatR.
/// </summary>
public class CitasController : ControladorBase
{
    private readonly IMediator _mediator;

    public CitasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene la lista completa de citas con datos de paciente y doctor incluidos.
    /// El frontend aplica los filtros por fecha, paciente y estado en memoria.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var resultado = await _mediator.Send(new ObtenerTodasCitasQuery());
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene una cita específica por su ID.
    /// </summary>
    /// <param name="id">Identificador de la cita.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var resultado = await _mediator.Send(new ObtenerCitaPorIdQuery(id));
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Crea una nueva cita.
    /// Valida que el doctor no tenga otra cita en el mismo horario (regla 21).
    /// </summary>
    /// <param name="dto">Datos de la cita a crear.</param>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearCitaDTO dto)
    {
        if (dto == null)
            return BadRequest(new { exitoso = false, mensaje = "El cuerpo de la petición es requerido." });

        var resultado = await _mediator.Send(new CrearCitaCommand(dto));
        return MapearCreacion(resultado);
    }

    /// <summary>
    /// Actualiza una cita existente.
    /// Permite cambiar paciente, doctor, fecha, motivo, notas y estado.
    /// </summary>
    /// <param name="id">ID de la cita a actualizar.</param>
    /// <param name="dto">Datos actualizados de la cita.</param>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarCitaDTO dto)
    {
        if (dto == null)
            return BadRequest(new { exitoso = false, mensaje = "El cuerpo de la petición es requerido." });

        if (id != dto.Id)
            return BadRequest(new
            {
                exitoso = false,
                mensaje = "El ID de la ruta no coincide con el ID del cuerpo."
            });

        var resultado = await _mediator.Send(new ActualizarCitaCommand(dto));
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Cancela una cita (cambia su estado a Cancelada).
    /// Endpoint pensado para el botón "Cancelar" del frontend.
    /// </summary>
    /// <param name="id">ID de la cita a cancelar.</param>
    [HttpPatch("{id:int}/cancelar")]
    public async Task<IActionResult> Cancelar(int id)
    {
        var resultado = await _mediator.Send(new CancelarCitaCommand(id));
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Elimina una cita (borrado lógico).
    /// </summary>
    /// <param name="id">ID de la cita a eliminar.</param>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _mediator.Send(new EliminarCitaCommand(id));
        return MapearResultado(resultado);
    }
}