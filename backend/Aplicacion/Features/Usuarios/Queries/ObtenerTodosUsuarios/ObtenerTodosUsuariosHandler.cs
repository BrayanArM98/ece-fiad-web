using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Usuarios;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Usuarios.Queries.ObtenerTodosUsuarios
{
    public class ObtenerTodosUsuariosHandler
        : IRequestHandler<ObtenerTodosUsuariosQuery, ResultadoAccion<IEnumerable<UsuarioDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerTodosUsuariosHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<IEnumerable<UsuarioDTO>>> Handle(
            ObtenerTodosUsuariosQuery request,
            CancellationToken cancellationToken)
        {
            var usuarios = await _unitOfWork.Usuarios.ObtenerTodosIncluyendoInactivosAsync();
            return ResultadoAccion<IEnumerable<UsuarioDTO>>.Exito(
                _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios));
        }
    }
}
