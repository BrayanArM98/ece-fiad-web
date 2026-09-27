using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Doctores;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Doctores.Queries.ObtenerTodosDoctores
{
    /// <summary>
    /// Manejador que devuelve todos los doctores junto con su especialidad.
    /// </summary>
    public class ObtenerTodosDoctoresHandler
        : IRequestHandler<ObtenerTodosDoctoresQuery, ResultadoAccion<IEnumerable<DoctorDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerTodosDoctoresHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<IEnumerable<DoctorDTO>>> Handle(
            ObtenerTodosDoctoresQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var doctores = await _unitOfWork.Doctores.ObtenerTodosConEspecialidadAsync();
                var lista = _mapper.Map<IEnumerable<DoctorDTO>>(doctores);
                return ResultadoAccion<IEnumerable<DoctorDTO>>.Exito(lista);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<IEnumerable<DoctorDTO>>.Falla(
                    $"Error al obtener doctores: {ex.Message}");
            }
        }
    }
}