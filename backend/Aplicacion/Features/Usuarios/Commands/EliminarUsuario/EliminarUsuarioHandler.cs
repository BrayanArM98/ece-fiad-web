using Aplicacion.Abstracciones;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Usuarios.Commands.EliminarUsuario
{
    /// <summary>
    /// Manejador que elimina una cuenta (borrado lógico).
    /// Un administrador no puede eliminar su propia cuenta.
    /// </summary>
    public class EliminarUsuarioHandler : IRequestHandler<EliminarUsuarioCommand, ResultadoAccion>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuarioActual _usuarioActual;

        public EliminarUsuarioHandler(IUnitOfWork unitOfWork, IUsuarioActual usuarioActual)
        {
            _unitOfWork = unitOfWork;
            _usuarioActual = usuarioActual;
        }

        public async Task<ResultadoAccion> Handle(EliminarUsuarioCommand request, CancellationToken cancellationToken)
        {
            if (_usuarioActual.IdUsuario == request.Id)
                return ResultadoAccion.Falla("No puedes eliminar tu propia cuenta.");

            var usuario = await _unitOfWork.Usuarios.ObtenerPorIdIncluyendoInactivoAsync(request.Id);
            if (usuario == null)
                return ResultadoAccion.Falla("Usuario no encontrado.");

            usuario.Eliminado = true;
            usuario.Activo = false;
            usuario.FechaDeEliminacion = DateTime.UtcNow;

            _unitOfWork.Usuarios.Actualizar(usuario);
            await _unitOfWork.GuardarCambiosAsync();

            return ResultadoAccion.Exito("Usuario eliminado exitosamente.");
        }
    }
}
