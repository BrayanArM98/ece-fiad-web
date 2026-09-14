using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Pacientes.Queries.ObtenerTodosPacientes
{
    /// <summary>
    /// Manejador de la query. Contiene la lógica que antes vivía
    /// en PacienteService.ObtenerTodosAsync().
    /// </summary>
    public class ObtenerTodosPacientesHandler
        : IRequestHandler<ObtenerTodosPacientesQuery, ResultadoAccion<IEnumerable<PacienteDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerTodosPacientesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<IEnumerable<PacienteDTO>>> Handle(
            ObtenerTodosPacientesQuery request,
            CancellationToken cancellationToken)
        {
            var pacientes = await _unitOfWork.Pacientes.ObtenerTodosAsync();
            var dtos = _mapper.Map<IEnumerable<PacienteDTO>>(pacientes);
            return ResultadoAccion<IEnumerable<PacienteDTO>>.Exito(dtos);
        }
    }
}