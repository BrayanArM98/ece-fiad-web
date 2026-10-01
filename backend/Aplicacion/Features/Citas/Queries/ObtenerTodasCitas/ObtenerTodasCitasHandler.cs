using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Citas.Queries.ObtenerTodasCitas
{
    /// <summary>
    /// Manejador que devuelve todas las citas con sus relaciones (paciente y doctor).
    /// </summary>
    public class ObtenerTodasCitasHandler
        : IRequestHandler<ObtenerTodasCitasQuery, ResultadoAccion<IEnumerable<CitaDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerTodasCitasHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<IEnumerable<CitaDTO>>> Handle(
            ObtenerTodasCitasQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var citas = await _unitOfWork.Citas.ObtenerTodasConRelacionesAsync();
                var lista = _mapper.Map<IEnumerable<CitaDTO>>(citas);
                return ResultadoAccion<IEnumerable<CitaDTO>>.Exito(lista);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<IEnumerable<CitaDTO>>.Falla(
                    $"Error al obtener citas: {ex.Message}");
            }
        }
    }
}