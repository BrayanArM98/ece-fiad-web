using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Especialidades;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Especialidades.Queries.ObtenerEspecialidadPorId
{
    /// <summary>
    /// Manejador que recupera una especialidad por su Id, incluyendo sus doctores.
    /// </summary>
    public class ObtenerEspecialidadPorIdHandler
        : IRequestHandler<ObtenerEspecialidadPorIdQuery, ResultadoAccion<EspecialidadDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerEspecialidadPorIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<EspecialidadDTO>> Handle(
            ObtenerEspecialidadPorIdQuery request,
            CancellationToken cancellationToken)
        {
            var especialidad = await _unitOfWork.Especialidades.ObtenerConDoctoresAsync(request.Id);

            if (especialidad == null)
                return ResultadoAccion<EspecialidadDTO>.Falla("Especialidad no encontrada");

            var dto = _mapper.Map<EspecialidadDTO>(especialidad);
            return ResultadoAccion<EspecialidadDTO>.Exito(dto);
        }
    }
}