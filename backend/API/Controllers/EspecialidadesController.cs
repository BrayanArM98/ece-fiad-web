using Aplicacion.DTOs.Especialidades;
using Aplicacion.Features.Especialidades.Commands.ActualizarEspecialidad;
using Aplicacion.Features.Especialidades.Commands.CrearEspecialidad;
using Aplicacion.Features.Especialidades.Commands.EliminarEspecialidad;
using Aplicacion.Features.Especialidades.Queries.ExisteEspecialidadPorNombre;
using Aplicacion.Features.Especialidades.Queries.ObtenerEspecialidadPorId;
using Aplicacion.Features.Especialidades.Queries.ObtenerTodasEspecialidades;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers;

/// <summary>
/// Controlador REST para la gestión de especialidades médicas.
/// Todas las operaciones se resuelven mediante el patrón CQRS:
/// las lecturas se envían como Queries y las escrituras como Commands a través de MediatR.
/// </summary>
public class EspecialidadesController : ControladorBase
{
    private readonly IMediator _mediator;

    public EspecialidadesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene la lista completa de especialidades médicas.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var resultado = await _mediator.Send(new ObtenerTodasEspecialidadesQuery());
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene una especialidad específica por su ID.
    /// </summary>
    /// <param name="id">Identificador de la especialidad.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var resultado = await _mediator.Send(new ObtenerEspecialidadPorIdQuery(id));
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Verifica si existe una especialidad con el nombre dado.
    /// </summary>
    /// <param name="nombre">Nombre de la especialidad a verificar.</param>
    /// <param name="idExcluir">ID a excluir de la verificación (útil al editar).</param>
    [HttpGet("existe/{nombre}")]
    public async Task<IActionResult> ExistePorNombre(string nombre, [FromQuery] int? idExcluir = null)
    {
        var existe = await _mediator.Send(new ExisteEspecialidadPorNombreQuery(nombre, idExcluir));
        return Ok(new
        {
            exitoso = true,
            mensaje = existe ? "La especialidad ya existe." : "El nombre está disponible.",
            datos = new { existe }
        });
    }

    /// <summary>
    /// Crea una nueva especialidad.
    /// </summary>
    /// <param name="dto">Datos de la especialidad a crear.</param>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearEspecialidadDTO dto)
    {
        if (dto == null)
            return BadRequest(new { exitoso = false, mensaje = "El cuerpo de la petición es requerido." });

        var resultado = await _mediator.Send(new CrearEspecialidadCommand(dto));
        return MapearCreacion(resultado);
    }

    /// <summary>
    /// Actualiza una especialidad existente.
    /// </summary>
    /// <param name="id">ID de la especialidad a actualizar.</param>
    /// <param name="dto">Datos actualizados.</param>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarEspecialidadDTO dto)
    {
        if (dto == null)
            return BadRequest(new { exitoso = false, mensaje = "El cuerpo de la petición es requerido." });

        if (id != dto.Id)
            return BadRequest(new
            {
                exitoso = false,
                mensaje = "El ID de la ruta no coincide con el ID del cuerpo."
            });

        var resultado = await _mediator.Send(new ActualizarEspecialidadCommand(dto));
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Elimina una especialidad (borrado lógico).
    /// </summary>
    /// <param name="id">ID de la especialidad a eliminar.</param>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _mediator.Send(new EliminarEspecialidadCommand(id));
        return MapearResultado(resultado);
    }
}