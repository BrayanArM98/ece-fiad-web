using Aplicacion.DTOs.Pacientes;
using Aplicacion.Features.Pacientes.Commands.ActualizarPaciente;
using Aplicacion.Features.Pacientes.Commands.CrearPaciente;
using Aplicacion.Features.Pacientes.Commands.EliminarPaciente;
using Aplicacion.Features.Pacientes.Queries.ExistePacientePorDocumento;
using Aplicacion.Features.Pacientes.Queries.ObtenerPacientePorId;
using Aplicacion.Features.Pacientes.Queries.ObtenerPacientesActivos;
using Aplicacion.Features.Pacientes.Queries.ObtenerPacientesSinHistoria;
using Aplicacion.Features.Pacientes.Queries.ObtenerTodosPacientes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers;

/// <summary>
/// Controlador REST para la gestión de pacientes.
/// Todas las operaciones se resuelven mediante el patrón CQRS:
/// las lecturas se envían como Queries y las escrituras como Commands a través de MediatR.
/// </summary>
public class PacientesController : ControladorBase
{
    private readonly IMediator _mediator;

    public PacientesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene la lista completa de pacientes.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var resultado = await _mediator.Send(new ObtenerTodosPacientesQuery());
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene la lista de pacientes activos.
    /// Endpoint pensado para alimentar selects del frontend (citas, evoluciones, etc.).
    /// </summary>
    [HttpGet("activos")]
    public async Task<IActionResult> ObtenerActivos()
    {
        var resultado = await _mediator.Send(new ObtenerPacientesActivosQuery());
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene la lista de pacientes que aún no tienen historia clínica.
    /// Útil para alimentar el dropdown de creación de historias clínicas.
    /// </summary>
    [HttpGet("sin-historia")]
    public async Task<IActionResult> ObtenerSinHistoriaClinica()
    {
        var resultado = await _mediator.Send(new ObtenerPacientesSinHistoriaQuery());
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene un paciente específico por su ID.
    /// </summary>
    /// <param name="id">Identificador del paciente.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var resultado = await _mediator.Send(new ObtenerPacientePorIdQuery(id));
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Verifica si existe un paciente con el número de identificación dado.
    /// </summary>
    /// <param name="identificacion">Número de documento a verificar.</param>
    [HttpGet("existe/{identificacion}")]
    public async Task<IActionResult> ExistePorIdentificacion(string identificacion)
    {
        var existe = await _mediator.Send(new ExistePacientePorDocumentoQuery(identificacion));
        return Ok(new
        {
            exitoso = true,
            mensaje = existe ? "El paciente existe." : "El paciente no existe.",
            datos = new { existe }
        });
    }

    /// <summary>
    /// Crea un nuevo paciente.
    /// </summary>
    /// <param name="dto">Datos del paciente a crear.</param>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearPacienteDTO dto)
    {
        if (dto == null)
            return BadRequest(new { exitoso = false, mensaje = "El cuerpo de la petición es requerido." });

        var resultado = await _mediator.Send(new CrearPacienteCommand(dto));
        return MapearCreacion(resultado);
    }

    /// <summary>
    /// Actualiza un paciente existente.
    /// </summary>
    /// <param name="id">ID del paciente a actualizar.</param>
    /// <param name="dto">Datos actualizados del paciente.</param>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarPacienteDTO dto)
    {
        if (dto == null)
            return BadRequest(new { exitoso = false, mensaje = "El cuerpo de la petición es requerido." });

        // Verificación de coherencia entre la ruta y el cuerpo
        if (id != dto.Id)
            return BadRequest(new
            {
                exitoso = false,
                mensaje = "El ID de la ruta no coincide con el ID del cuerpo."
            });

        var resultado = await _mediator.Send(new ActualizarPacienteCommand(dto));
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Elimina un paciente (borrado lógico).
    /// </summary>
    /// <param name="id">ID del paciente a eliminar.</param>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _mediator.Send(new EliminarPacienteCommand(id));
        return MapearResultado(resultado);
    }
}