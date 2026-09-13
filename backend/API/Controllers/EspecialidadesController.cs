using Aplicacion.DTOs.Especialidades;
using Aplicacion.Servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers;

/// <summary>
/// Controlador REST para la gestión de especialidades médicas.
/// Expone endpoints para CRUD completo y consultas auxiliares.
/// </summary>
public class EspecialidadesController : ControladorBase
{
    private readonly IEspecialidadService _servicioEspecialidades;

    public EspecialidadesController(IEspecialidadService servicioEspecialidades)
    {
        _servicioEspecialidades = servicioEspecialidades;
    }

    /// <summary>
    /// Obtiene la lista completa de especialidades médicas.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var resultado = await _servicioEspecialidades.ObtenerTodosAsync();
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene una especialidad específica por su ID.
    /// </summary>
    /// <param name="id">Identificador de la especialidad.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var resultado = await _servicioEspecialidades.ObtenerPorIdAsync(id);
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
        var existe = await _servicioEspecialidades.ExistePorNombreAsync(nombre, idExcluir);
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

        var resultado = await _servicioEspecialidades.CrearAsync(dto);
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

        var resultado = await _servicioEspecialidades.ActualizarAsync(dto);
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Elimina una especialidad (borrado lógico).
    /// </summary>
    /// <param name="id">ID de la especialidad a eliminar.</param>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _servicioEspecialidades.EliminarAsync(id);
        return MapearResultado(resultado);
    }
}