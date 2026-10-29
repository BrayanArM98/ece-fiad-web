using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Usuarios;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Usuarios.Queries.ObtenerUsuarioPorId
{
    public class ObtenerUsuarioPorIdHandler
        : IRequestHandler<ObtenerUsuarioPorIdQuery, ResultadoAccion<UsuarioDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerUsuarioPorIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<UsuarioDTO>> Handle(
            ObtenerUsuarioPorIdQuery request,
            CancellationToken cancellationToken)
        {
            var usuario = await _unitOfWork.Usuarios.ObtenerPorIdIncluyendoInactivoAsync(request.Id);
            if (usuario == null)
                return ResultadoAccion<UsuarioDTO>.Falla("Usuario no encontrado.");

            return ResultadoAccion<UsuarioDTO>.Exito(_mapper.Map<UsuarioDTO>(usuario));
        }
    }
}
