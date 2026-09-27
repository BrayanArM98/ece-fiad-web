using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Doctores;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Doctores.Queries.ObtenerDoctorPorId
{
    /// <summary>
    /// Manejador que recupera un doctor por su Id, incluyendo su especialidad.
    /// </summary>
    public class ObtenerDoctorPorIdHandler
        : IRequestHandler<ObtenerDoctorPorIdQuery, ResultadoAccion<DoctorDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerDoctorPorIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<DoctorDTO>> Handle(
            ObtenerDoctorPorIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var doctor = await _unitOfWork.Doctores.ObtenerConEspecialidadAsync(request.Id);
                if (doctor == null)
                    return ResultadoAccion<DoctorDTO>.Falla("Doctor no encontrado.");

                var dto = _mapper.Map<DoctorDTO>(doctor);
                return ResultadoAccion<DoctorDTO>.Exito(dto);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<DoctorDTO>.Falla($"Error al obtener doctor: {ex.Message}");
            }
        }
    }
}