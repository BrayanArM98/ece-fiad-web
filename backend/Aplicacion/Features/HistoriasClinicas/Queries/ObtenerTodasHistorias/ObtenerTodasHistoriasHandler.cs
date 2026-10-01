using Aplicacion.Abstracciones;
using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.HistoriasClinicas.Queries.ObtenerTodasHistorias
{
    /// <summary>
    /// Manejador que devuelve todas las historias clínicas con su paciente asociado.
    /// </summary>
    public class ObtenerTodasHistoriasHandler
        : IRequestHandler<ObtenerTodasHistoriasQuery, ResultadoAccion<IEnumerable<HistoriaClinicaDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerTodasHistoriasHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<IEnumerable<HistoriaClinicaDTO>>> Handle(
            ObtenerTodasHistoriasQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var historias = await _unitOfWork.HistoriasClinicas.ObtenerTodasConRelacionesAsync();
                var lista = _mapper.Map<IEnumerable<HistoriaClinicaDTO>>(historias);
                return ResultadoAccion<IEnumerable<HistoriaClinicaDTO>>.Exito(lista);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<IEnumerable<HistoriaClinicaDTO>>.Falla(
                    $"Error al obtener historias clínicas: {ex.Message}");
            }
        }
    }
}