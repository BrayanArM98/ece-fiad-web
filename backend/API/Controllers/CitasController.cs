using Aplicacion.DTOs.Citas;
using Aplicacion.Servicios.Interfaces;
using Dominio.Enumeraciones;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers;

/// <summary>
/// Controlador REST para la gestión de citas médicas.
/// Cada cita asocia a un paciente con un doctor en una fecha y hora específicas.
/// Aplica la regla 21: un doctor no puede tener dos citas en el mismo horario.
/// </summary>
public class CitasController : ControladorBase
{
    private readonly ICitaService _servicioCitas;

    public CitasController(ICitaService servicioCitas)
    {
        _servicioCitas = servicioCitas;
    }

    /// <summary>
    /// Obtiene la lista completa de citas con datos de paciente y doctor incluidos.
    /// El frontend aplica los filtros por fecha, paciente y estado en memoria.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var resultado = await _servicioCitas.ObtenerTodasAsync();
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene una cita específica por su ID.
    /// </summary>
    /// <param name="id">Identificador de la cita.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var resultado = await _servicioCitas.ObtenerPorIdAsync(id);
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

        var resultado = await _servicioCitas.CrearAsync(dto);
        return MapearCreacion(resultado);
    }

    /// <summary>
    /// Actualiza una cita existente.
    /// Permite cambiar paciente, doctor, fecha, motivo, notas y estado.
    /// Valida disponibilidad horaria del nuevo doctor (regla 21).
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

        var resultado = await _servicioCitas.ActualizarAsync(dto);
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
        // 1. Obtener la cita actual
        var resultadoCita = await _servicioCitas.ObtenerPorIdAsync(id);
        if (!resultadoCita.Exitoso || resultadoCita.Datos == null)
            return MapearResultado(resultadoCita);

        var citaActual = resultadoCita.Datos;

        // 2. Construir el DTO de actualización solo cambiando el estado a Cancelada
        var dtoActualizar = new ActualizarCitaDTO
        {
            Id = citaActual.Id,
            IdPaciente = citaActual.IdPaciente,
            IdDoctor = citaActual.IdDoctor,
            FechaHora = citaActual.FechaHora,
            Motivo = citaActual.Motivo,
            Notas = citaActual.Notas,
            Estado = EstadoCita.Cancelada
        };

        // 3. Llamar al servicio de actualización
        var resultado = await _servicioCitas.ActualizarAsync(dtoActualizar);
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Elimina una cita (borrado lógico).
    /// </summary>
    /// <param name="id">ID de la cita a eliminar.</param>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _servicioCitas.EliminarAsync(id);
        return MapearResultado(resultado);
    }
}