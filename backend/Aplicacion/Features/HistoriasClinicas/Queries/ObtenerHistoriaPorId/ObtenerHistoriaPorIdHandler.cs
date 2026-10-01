using Aplicacion.Abstracciones;
using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.HistoriasClinicas.Queries.ObtenerHistoriaPorId
{
    /// <summary>
    /// Manejador que recupera una historia clínica por su Id, incluyendo sus relaciones.
    /// </summary>
    public class ObtenerHistoriaPorIdHandler
        : IRequestHandler<ObtenerHistoriaPorIdQuery, ResultadoAccion<HistoriaClinicaDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerHistoriaPorIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<HistoriaClinicaDTO>> Handle(
            ObtenerHistoriaPorIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var historia = await _unitOfWork.HistoriasClinicas.ObtenerConRelacionesAsync(request.Id);
                if (historia == null)
                    return ResultadoAccion<HistoriaClinicaDTO>.Falla("Historia clínica no encontrada.");

                var dto = _mapper.Map<HistoriaClinicaDTO>(historia);
                return ResultadoAccion<HistoriaClinicaDTO>.Exito(dto);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<HistoriaClinicaDTO>.Falla(
                    $"Error al obtener historia clínica: {ex.Message}");
            }
        }
    }
}