using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Pacientes.Queries.ObtenerPacientesSinHistoria
{
    /// <summary>
    /// Manejador que devuelve los pacientes activos que todavía no tienen historia clínica.
    /// Regla 22: cada paciente puede tener una sola historia clínica activa,
    /// así que solo estos pacientes pueden elegirse al abrir una historia nueva.
    /// </summary>
    public class ObtenerPacientesSinHistoriaHandler
        : IRequestHandler<ObtenerPacientesSinHistoriaQuery, ResultadoAccion<IEnumerable<PacienteDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerPacientesSinHistoriaHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<IEnumerable<PacienteDTO>>> Handle(
            ObtenerPacientesSinHistoriaQuery request,
            CancellationToken cancellationToken)
        {
            var historias = await _unitOfWork.HistoriasClinicas.ObtenerTodosAsync();
            var pacientesConHistoria = historias.Select(h => h.IdPaciente).ToHashSet();

            var pacientes = await _unitOfWork.Pacientes.ObtenerTodosAsync();
            var sinHistoria = pacientes
                .Where(p => !pacientesConHistoria.Contains(p.Id))
                .OrderBy(p => p.Apellidos)
                .ThenBy(p => p.Nombres);

            var dtos = _mapper.Map<IEnumerable<PacienteDTO>>(sinHistoria);
            return ResultadoAccion<IEnumerable<PacienteDTO>>.Exito(
                dtos, "Pacientes sin historia clínica obtenidos correctamente.");
        }
    }
}
