using Aplicacion.DTOs.Usuarios;
using Aplicacion.Features.Usuarios.Commands.ActualizarUsuario;
using Aplicacion.Features.Usuarios.Commands.CrearUsuario;
using Aplicacion.Features.Usuarios.Commands.EliminarUsuario;
using Aplicacion.Features.Usuarios.Queries.ObtenerTodosUsuarios;
using Aplicacion.Features.Usuarios.Queries.ObtenerUsuarioPorId;
using Aplicacion.Seguridad;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers;

/// <summary>
/// Gestión de cuentas de acceso. Solo para administradores.
/// </summary>
[Authorize(Roles = Roles.Administrador)]
public class UsuariosController : ControladorBase
{
    private readonly IMediator _mediator;

    public UsuariosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todos los usuarios, incluidos los desactivados.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var resultado = await _mediator.Send(new ObtenerTodosUsuariosQuery());
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene un usuario por su ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var resultado = await _mediator.Send(new ObtenerUsuarioPorIdQuery(id));
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Crea una cuenta. Un médico se vincula con un doctor (IdDoctor) y un paciente con un paciente (IdPaciente).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearUsuarioDTO dto)
    {
        var resultado = await _mediator.Send(new CrearUsuarioCommand(dto));
        return MapearCreacion(resultado);
    }

    /// <summary>
    /// Actualiza nombre, rol, vínculos y estado. La contraseña solo cambia si se envía NuevaContrasena.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarUsuarioDTO dto)
    {
        if (id != dto.Id)
            return BadRequest(new
            {
                exitoso = false,
                mensaje = "El ID de la ruta no coincide con el ID del cuerpo.",
                errores = Array.Empty<string>()
            });

        var resultado = await _mediator.Send(new ActualizarUsuarioCommand(dto));
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Elimina una cuenta (borrado lógico). No se puede eliminar la cuenta propia.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _mediator.Send(new EliminarUsuarioCommand(id));
        return MapearResultado(resultado);
    }
}
