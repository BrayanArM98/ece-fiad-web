using Aplicacion.DTOs.Pacientes;
using Aplicacion.Features.Pacientes.Commands.CrearPaciente;
using Aplicacion.Features.Pacientes.Queries.ObtenerPacientePorId;
using Aplicacion.Features.Pacientes.Queries.ObtenerTodosPacientes;
using Aplicacion.Servicios.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers;

/// <summary>
/// Controlador REST para la gestión de pacientes.
/// Expone endpoints para CRUD completo y consultas específicas del módulo de pacientes.
/// </summary>
public class PacientesController : ControladorBase
{
    private readonly IPacienteService _servicioPacientes;
    private readonly IMediator _mediator;

    public PacientesController(IPacienteService servicioPacientes, IMediator mediator)
    {
        _servicioPacientes = servicioPacientes;
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene la lista completa de pacientes.
    /// Migrado a CQRS: la petición se envía como Query a través de MediatR.
    /// </summary>
    /// <returns>Lista de pacientes registrados.</returns>
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
    /// <returns>Lista de pacientes activos.</returns>
    [HttpGet("activos")]
    public async Task<IActionResult> ObtenerActivos()
    {
        var resultado = await _servicioPacientes.ObtenerTodosAsync();

        if (!resultado.Exitoso)
            return MapearResultado(resultado);

        // Filtra solo los activos en memoria
        var activos = resultado.Datos?.Where(p => p.Activo).ToList()
                      ?? new List<PacienteDTO>();

        return Ok(new
        {
            exitoso = true,
            mensaje = "Pacientes activos obtenidos correctamente.",
            datos = activos
        });
    }

    /// <summary>
    /// Obtiene la lista de pacientes que aún no tienen historia clínica activa.
    /// Útil para alimentar el dropdown de creación de historias clínicas.
    /// </summary>
    [HttpGet("sin-historia")]
    public async Task<IActionResult> ObtenerSinHistoriaClinica()
    {
        var resultado = await _servicioPacientes.ObtenerSinHistoriaClinicaAsync();
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene un paciente específico por su ID.
    /// Migrado a CQRS: la petición se envía como Query a través de MediatR.
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
    /// <returns>True si existe, false si no.</returns>
    [HttpGet("existe/{identificacion}")]
    public async Task<IActionResult> ExistePorIdentificacion(string identificacion)
    {
        var existe = await _servicioPacientes.ExistePorIdentificacionAsync(identificacion);
        return Ok(new
        {
            exitoso = true,
            mensaje = existe ? "El paciente existe." : "El paciente no existe.",
            datos = new { existe }
        });
    }

    /// <summary>
    /// Crea un nuevo paciente.
    /// Migrado a CQRS: la petición se envía como Command a través de MediatR.
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

        var resultado = await _servicioPacientes.ActualizarAsync(dto);
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Elimina un paciente (borrado lógico).
    /// </summary>
    /// <param name="id">ID del paciente a eliminar.</param>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _servicioPacientes.EliminarAsync(id);
        return MapearResultado(resultado);
    }
}