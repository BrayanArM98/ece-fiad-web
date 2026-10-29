using Microsoft.AspNetCore.Diagnostics;

namespace Presentacion.Middleware;

/// <summary>
/// Atrapa cualquier excepción que no se controló en los handlers o controladores.
/// Registra el detalle en el log del servidor y responde al cliente con la misma forma
/// que el resto de la API, sin exponer información interna (mensajes de SQL, rutas, etc.).
/// </summary>
public sealed class ManejadorGlobalExcepciones : IExceptionHandler
{
    private readonly ILogger<ManejadorGlobalExcepciones> _logger;

    public ManejadorGlobalExcepciones(ILogger<ManejadorGlobalExcepciones> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext contexto,
        Exception excepcion,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            excepcion,
            "Error no controlado en {Metodo} {Ruta}",
            contexto.Request.Method,
            contexto.Request.Path);

        contexto.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await contexto.Response.WriteAsJsonAsync(new
        {
            exitoso = false,
            mensaje = "Ocurrió un error inesperado en el servidor. Intenta de nuevo más tarde.",
            errores = Array.Empty<string>()
        }, cancellationToken);

        return true;
    }
}
