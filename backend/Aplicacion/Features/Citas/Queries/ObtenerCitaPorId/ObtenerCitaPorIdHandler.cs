using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Citas.Queries.ObtenerCitaPorId
{
    /// <summary>
    /// Manejador que recupera una cita por su Id, incluyendo paciente y doctor.
    /// </summary>
    public class ObtenerCitaPorIdHandler
        : IRequestHandler<ObtenerCitaPorIdQuery, ResultadoAccion<CitaDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerCitaPorIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<CitaDTO>> Handle(
            ObtenerCitaPorIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var cita = await _unitOfWork.Citas.ObtenerConRelacionesAsync(request.Id);
                if (cita == null)
                    return ResultadoAccion<CitaDTO>.Falla("Cita no encontrada.");

                var dto = _mapper.Map<CitaDTO>(cita);
                return ResultadoAccion<CitaDTO>.Exito(dto);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<CitaDTO>.Falla($"Error al obtener cita: {ex.Message}");
            }
        }
    }
}