using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Evoluciones.Queries.ObtenerEvolucionPorId
{
    /// <summary>
    /// Manejador que recupera una evolución por su Id, incluyendo sus relaciones.
    /// </summary>
    public class ObtenerEvolucionPorIdHandler
        : IRequestHandler<ObtenerEvolucionPorIdQuery, ResultadoAccion<EvolucionDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerEvolucionPorIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<EvolucionDTO>> Handle(
            ObtenerEvolucionPorIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var evolucion = await _unitOfWork.Evoluciones.ObtenerConRelacionesAsync(request.Id);
                if (evolucion == null)
                    return ResultadoAccion<EvolucionDTO>.Falla("Evolución no encontrada.");

                var dto = _mapper.Map<EvolucionDTO>(evolucion);
                return ResultadoAccion<EvolucionDTO>.Exito(dto);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<EvolucionDTO>.Falla($"Error al obtener evolución: {ex.Message}");
            }
        }
    }
}