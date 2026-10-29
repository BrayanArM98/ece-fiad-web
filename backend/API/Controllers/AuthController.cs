using Aplicacion.DTOs.Auth;
using Aplicacion.Features.Auth.Commands.IniciarSesion;
using Aplicacion.Features.Auth.Queries.ObtenerSesionActual;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Presentacion.Controllers;

/// <summary>
/// Inicio de sesión y datos de la sesión actual.
/// </summary>
public class AuthController : ControladorBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Valida correo y contraseña y devuelve el token de acceso.
    /// El token se envía después en el encabezado Authorization: Bearer {token}.
    /// Limitado a 5 intentos por minuto por dirección IP.
    /// </summary>
    [AllowAnonymous]
    [EnableRateLimiting("login")]
    [HttpPost("login")]
    public async Task<IActionResult> IniciarSesion([FromBody] IniciarSesionDTO dto)
    {
        var resultado = await _mediator.Send(new IniciarSesionCommand(dto));

        // Credenciales incorrectas: 401 (los errores de formato siguen siendo 400)
        if (!resultado.Exitoso && resultado.Errores.Count == 0)
        {
            return Unauthorized(new
            {
                exitoso = false,
                mensaje = resultado.Mensaje,
                errores = resultado.Errores
            });
        }

        return MapearResultado(resultado);
    }

    /// <summary>
    /// Devuelve los datos del usuario dueño del token.
    /// </summary>
    [HttpGet("yo")]
    public async Task<IActionResult> ObtenerSesionActual()
    {
        var resultado = await _mediator.Send(new ObtenerSesionActualQuery());
        return MapearResultado(resultado);
    }
}
