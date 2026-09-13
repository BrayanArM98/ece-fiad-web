using Aplicacion.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers;

/// <summary>
/// Clase base para todos los controladores REST de ECE-FIAD.
/// Provee helpers para traducir ResultadoAccion / ResultadoAccion T  a respuestas HTTP estándar.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ControladorBase : ControllerBase
{
    /// <summary>
    /// Traduce un ResultadoAccion T  a una respuesta HTTP con el código correcto.
    /// Usar para operaciones GET, PUT, DELETE que devuelven datos o confirmación.
    /// </summary>
    protected IActionResult MapearResultado<T>(ResultadoAccion<T> resultado)
    {
        if (resultado.Exitoso)
        {
            return Ok(new
            {
                exitoso = true,
                mensaje = resultado.Mensaje,
                datos = resultado.Datos
            });
        }

        return MapearError(resultado.Mensaje);
    }

    /// <summary>
    /// Traduce un ResultadoAccion sin datos a una respuesta HTTP con el código correcto.
    /// Usar para operaciones que solo confirman éxito o falla (ejemplo: eliminar).
    /// </summary>
    protected IActionResult MapearResultado(ResultadoAccion resultado)
    {
        if (resultado.Exitoso)
        {
            return Ok(new
            {
                exitoso = true,
                mensaje = resultado.Mensaje
            });
        }

        return MapearError(resultado.Mensaje);
    }

    /// <summary>
    /// Traduce un ResultadoAccion T  exitoso a una respuesta 201 Created.
    /// Usar específicamente para operaciones POST que crean recursos nuevos.
    /// </summary>
    protected IActionResult MapearCreacion<T>(ResultadoAccion<T> resultado)
    {
        if (resultado.Exitoso)
        {
            return StatusCode(StatusCodes.Status201Created, new
            {
                exitoso = true,
                mensaje = resultado.Mensaje,
                datos = resultado.Datos
            });
        }

        return MapearError(resultado.Mensaje);
    }

    /// <summary>
    /// Inspecciona el mensaje de error y decide el código HTTP más apropiado.
    /// 404 si dice "no encontrado", 409 si dice "ya existe / duplicado", 400 en cualquier otro caso.
    /// </summary>
    private IActionResult MapearError(string? mensaje)
    {
        var msg = (mensaje ?? string.Empty).ToLowerInvariant();

        // 404 Not Found
        if (msg.Contains("no encontrado") ||
            msg.Contains("no existe") ||
            msg.Contains("no se encontró") ||
            msg.Contains("no se encontro"))
        {
            return NotFound(new
            {
                exitoso = false,
                mensaje
            });
        }

        // 409 Conflict
        if (msg.Contains("ya existe") ||
            msg.Contains("duplicad") ||
            msg.Contains("ya tiene") ||
            msg.Contains("ya está") ||
            msg.Contains("ya esta") ||
            msg.Contains("ocupad"))
        {
            return Conflict(new
            {
                exitoso = false,
                mensaje
            });
        }

        // 400 Bad Request por defecto
        return BadRequest(new
        {
            exitoso = false,
            mensaje
        });
    }
}