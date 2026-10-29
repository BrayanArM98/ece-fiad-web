using Aplicacion.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers;

/// <summary>
/// Clase base para todos los controladores REST de ECE-FIAD.
/// Traduce ResultadoAccion / ResultadoAccion T a respuestas HTTP con la misma forma:
/// { exitoso, mensaje, datos } cuando sale bien y { exitoso, mensaje, errores } cuando falla.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ControladorBase : ControllerBase
{
    /// <summary>
    /// Traduce un ResultadoAccion T a una respuesta HTTP con el código correcto.
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

        return MapearError(resultado.Mensaje, resultado.Errores);
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

        return MapearError(resultado.Mensaje, resultado.Errores);
    }

    /// <summary>
    /// Traduce un ResultadoAccion T exitoso a una respuesta 201 Created.
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

        return MapearError(resultado.Mensaje, resultado.Errores);
    }

    /// <summary>
    /// Inspecciona el mensaje de error y decide el código HTTP más apropiado.
    /// 404 si dice "no encontrado", 409 si dice "ya existe / duplicado", 400 en cualquier otro caso.
    /// La lista de errores (por ejemplo, de validación) siempre se incluye para que el cliente pueda mostrarlos.
    /// </summary>
    private IActionResult MapearError(string? mensaje, List<string>? errores)
    {
        var msg = (mensaje ?? string.Empty).ToLowerInvariant();
        var cuerpo = new
        {
            exitoso = false,
            mensaje,
            errores = errores ?? new List<string>()
        };

        // 404 Not Found
        if (msg.Contains("no encontrado") ||
            msg.Contains("no existe") ||
            msg.Contains("no se encontró") ||
            msg.Contains("no se encontro"))
        {
            return NotFound(cuerpo);
        }

        // 409 Conflict
        if (msg.Contains("ya existe") ||
            msg.Contains("duplicad") ||
            msg.Contains("ya tiene") ||
            msg.Contains("ya está") ||
            msg.Contains("ya esta") ||
            msg.Contains("ocupad"))
        {
            return Conflict(cuerpo);
        }

        // 400 Bad Request por defecto
        return BadRequest(cuerpo);
    }
}
