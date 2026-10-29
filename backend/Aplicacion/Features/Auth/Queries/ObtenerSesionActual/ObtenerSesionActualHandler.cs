using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Usuarios;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Auth.Queries.ObtenerSesionActual
{
    public class ObtenerSesionActualHandler
        : IRequestHandler<ObtenerSesionActualQuery, ResultadoAccion<UsuarioDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUsuarioActual _usuarioActual;

        public ObtenerSesionActualHandler(IUnitOfWork unitOfWork, IMapper mapper, IUsuarioActual usuarioActual)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _usuarioActual = usuarioActual;
        }

        public async Task<ResultadoAccion<UsuarioDTO>> Handle(
            ObtenerSesionActualQuery request,
            CancellationToken cancellationToken)
        {
            if (_usuarioActual.IdUsuario is not int idUsuario)
                return ResultadoAccion<UsuarioDTO>.Falla("Usuario no encontrado.");

            // Si el usuario se desactivó después de iniciar sesión, la sesión ya no es válida
            var usuario = await _unitOfWork.Usuarios.ObtenerPorIdAsync(idUsuario);
            if (usuario == null)
                return ResultadoAccion<UsuarioDTO>.Falla("Usuario no encontrado.");

            return ResultadoAccion<UsuarioDTO>.Exito(_mapper.Map<UsuarioDTO>(usuario));
        }
    }
}
