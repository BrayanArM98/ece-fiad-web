using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Pacientes.Queries.ObtenerPacientesActivos
{
    /// <summary>
    /// Manejador que devuelve solo los pacientes marcados como activos.
    /// El filtrado se hace aquí y no en el controlador, que solo debe
    /// recibir la petición y devolver la respuesta.
    /// </summary>
    public class ObtenerPacientesActivosHandler
        : IRequestHandler<ObtenerPacientesActivosQuery, ResultadoAccion<IEnumerable<PacienteDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerPacientesActivosHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<IEnumerable<PacienteDTO>>> Handle(
            ObtenerPacientesActivosQuery request,
            CancellationToken cancellationToken)
        {
            var pacientes = await _unitOfWork.Pacientes.ObtenerTodosAsync();
            var activos = pacientes.Where(p => p.Activo);

            var dtos = _mapper.Map<IEnumerable<PacienteDTO>>(activos);
            return ResultadoAccion<IEnumerable<PacienteDTO>>.Exito(
                dtos, "Pacientes activos obtenidos correctamente.");
        }
    }
}