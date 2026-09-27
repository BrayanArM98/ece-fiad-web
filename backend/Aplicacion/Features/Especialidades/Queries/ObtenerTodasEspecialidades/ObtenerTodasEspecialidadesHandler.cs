using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Especialidades;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Especialidades.Queries.ObtenerTodasEspecialidades
{
    /// <summary>
    /// Manejador que devuelve todas las especialidades junto con sus doctores,
    /// para que el DTO pueda calcular la cantidad de doctores asociados.
    /// </summary>
    public class ObtenerTodasEspecialidadesHandler
        : IRequestHandler<ObtenerTodasEspecialidadesQuery, ResultadoAccion<IEnumerable<EspecialidadDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerTodasEspecialidadesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<IEnumerable<EspecialidadDTO>>> Handle(
            ObtenerTodasEspecialidadesQuery request,
            CancellationToken cancellationToken)
        {
            var especialidades = await _unitOfWork.Especialidades.ObtenerTodosConDoctoresAsync();
            var dtos = _mapper.Map<IEnumerable<EspecialidadDTO>>(especialidades);
            return ResultadoAccion<IEnumerable<EspecialidadDTO>>.Exito(dtos);
        }
    }
}