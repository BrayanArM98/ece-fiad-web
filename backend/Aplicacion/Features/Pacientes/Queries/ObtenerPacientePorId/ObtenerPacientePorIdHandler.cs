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
    /// Un usuario paciente solo puede consultar su propio registro.
    /// </summary>
    public class ObtenerPacientePorIdHandler
        : IRequestHandler<ObtenerPacientePorIdQuery, ResultadoAccion<PacienteDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUsuarioActual _usuarioActual;

        public ObtenerPacientePorIdHandler(IUnitOfWork unitOfWork, IMapper mapper, IUsuarioActual usuarioActual)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _usuarioActual = usuarioActual;
        }

        public async Task<ResultadoAccion<PacienteDTO>> Handle(
            ObtenerPacientePorIdQuery request,
            CancellationToken cancellationToken)
        {
            if (_usuarioActual.EsPaciente && request.Id != _usuarioActual.IdPaciente)
                return ResultadoAccion<PacienteDTO>.Falla("Paciente no encontrado");

            var paciente = await _unitOfWork.Pacientes.ObtenerPorIdAsync(request.Id);

            if (paciente == null)
                return ResultadoAccion<PacienteDTO>.Falla("Paciente no encontrado");

            var dto = _mapper.Map<PacienteDTO>(paciente);
            return ResultadoAccion<PacienteDTO>.Exito(dto);
        }
    }
}