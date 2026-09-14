using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Pacientes.Queries.ObtenerPacientePorId
{
    /// <summary>
    /// Manejador que recupera un paciente por su Id.
    /// Devuelve falla si el paciente no existe.
    /// </summary>
    public class ObtenerPacientePorIdHandler
        : IRequestHandler<ObtenerPacientePorIdQuery, ResultadoAccion<PacienteDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerPacientePorIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<PacienteDTO>> Handle(
            ObtenerPacientePorIdQuery request,
            CancellationToken cancellationToken)
        {
            var paciente = await _unitOfWork.Pacientes.ObtenerPorIdAsync(request.Id);

            if (paciente == null)
                return ResultadoAccion<PacienteDTO>.Falla("Paciente no encontrado");

            var dto = _mapper.Map<PacienteDTO>(paciente);
            return ResultadoAccion<PacienteDTO>.Exito(dto);
        }
    }
}