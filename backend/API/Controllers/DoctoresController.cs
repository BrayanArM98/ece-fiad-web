using Aplicacion.DTOs.Doctores;
using Aplicacion.Servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers;

/// <summary>
/// Controlador REST para la gestión de doctores.
/// Cada doctor está asociado a una especialidad médica.
/// </summary>
public class DoctoresController : ControladorBase
{
    private readonly IDoctorService _servicioDoctores;

    public DoctoresController(IDoctorService servicioDoctores)
    {
        _servicioDoctores = servicioDoctores;
    }

    /// <summary>
    /// Obtiene la lista completa de doctores con su especialidad asociada.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var resultado = await _servicioDoctores.ObtenerTodosAsync();
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene un doctor específico por su ID.
    /// </summary>
    /// <param name="id">Identificador del doctor.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var resultado = await _servicioDoctores.ObtenerPorIdAsync(id);
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Verifica si ya existe un doctor con el email dado.
    /// </summary>
    /// <param name="email">Email a verificar.</param>
    /// <param name="idExcluir">ID a excluir de la verificación (útil al editar).</param>
    [HttpGet("existe-email/{email}")]
    public async Task<IActionResult> ExistePorEmail(string email, [FromQuery] int? idExcluir = null)
    {
        var existe = await _servicioDoctores.ExistePorEmailAsync(email, idExcluir);
        return Ok(new
        {
            exitoso = true,
            mensaje = existe ? "El email ya está registrado." : "El email está disponible.",
            datos = new { existe }
        });
    }

    /// <summary>
    /// Crea un nuevo doctor.
    /// </summary>
    /// <param name="dto">Datos del doctor a crear, incluyendo su especialidad.</param>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearDoctorDTO dto)
    {
        if (dto == null)
            return BadRequest(new { exitoso = false, mensaje = "El cuerpo de la petición es requerido." });

        var resultado = await _servicioDoctores.CrearAsync(dto);
        return MapearCreacion(resultado);
    }

    /// <summary>
    /// Actualiza un doctor existente.
    /// </summary>
    /// <param name="id">ID del doctor a actualizar.</param>
    /// <param name="dto">Datos actualizados del doctor.</param>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarDoctorDTO dto)
    {
        if (dto == null)
            return BadRequest(new { exitoso = false, mensaje = "El cuerpo de la petición es requerido." });

        if (id != dto.Id)
            return BadRequest(new
            {
                exitoso = false,
                mensaje = "El ID de la ruta no coincide con el ID del cuerpo."
            });

        var resultado = await _servicioDoctores.ActualizarAsync(dto);
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Elimina un doctor (borrado lógico).
    /// El servicio valida que el doctor no tenga citas asociadas antes de eliminar.
    /// </summary>
    /// <param name="id">ID del doctor a eliminar.</param>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _servicioDoctores.EliminarAsync(id);
        return MapearResultado(resultado);
    }
}