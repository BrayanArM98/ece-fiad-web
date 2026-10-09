using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Evoluciones.Queries.ObtenerEvolucionesPorHistoria
{
    /// <summary>
    /// Manejador que devuelve todas las evoluciones de una historia clínica.
    /// </summary>
    public class ObtenerEvolucionesPorHistoriaHandler
        : IRequestHandler<ObtenerEvolucionesPorHistoriaQuery, ResultadoAccion<IEnumerable<EvolucionDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerEvolucionesPorHistoriaHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<IEnumerable<EvolucionDTO>>> Handle(
            ObtenerEvolucionesPorHistoriaQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var evoluciones = await _unitOfWork.Evoluciones
                    .ObtenerPorHistoriaClinicaAsync(request.IdHistoriaClinica);

                var lista = _mapper.Map<IEnumerable<EvolucionDTO>>(evoluciones);
                return ResultadoAccion<IEnumerable<EvolucionDTO>>.Exito(lista);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<IEnumerable<EvolucionDTO>>.Falla(
                    $"Error al obtener evoluciones de la historia clínica: {ex.Message}");
            }
        }
    }
}