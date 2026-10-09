using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Evoluciones.Queries.ObtenerTodasEvoluciones
{
    /// <summary>
    /// Manejador que devuelve todas las evoluciones con sus relaciones.
    /// </summary>
    public class ObtenerTodasEvolucionesHandler
        : IRequestHandler<ObtenerTodasEvolucionesQuery, ResultadoAccion<IEnumerable<EvolucionDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerTodasEvolucionesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<IEnumerable<EvolucionDTO>>> Handle(
            ObtenerTodasEvolucionesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var evoluciones = await _unitOfWork.Evoluciones.ObtenerTodasConRelacionesAsync();
                var lista = _mapper.Map<IEnumerable<EvolucionDTO>>(evoluciones);
                return ResultadoAccion<IEnumerable<EvolucionDTO>>.Exito(lista);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<IEnumerable<EvolucionDTO>>.Falla(
                    $"Error al obtener evoluciones: {ex.Message}");
            }
        }
    }
}