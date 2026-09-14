using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Helpers;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Pacientes.Commands.ActualizarPaciente
{
    /// <summary>
    /// Manejador que actualiza los datos de un paciente existente.
    /// Valida la entrada, verifica que el paciente exista y persiste los cambios.
    /// </summary>
    public class ActualizarPacienteHandler
        : IRequestHandler<ActualizarPacienteCommand, ResultadoAccion<PacienteDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<ActualizarPacienteDTO> _validador;

        public ActualizarPacienteHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<ActualizarPacienteDTO> validador)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
        }

        public async Task<ResultadoAccion<PacienteDTO>> Handle(
            ActualizarPacienteCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Paciente;

            // 1. Validar datos de entrada
            var validacion = await _validador.ValidateAsync(dto, cancellationToken);
            if (!validacion.IsValid)
            {
                return ResultadoAccion<PacienteDTO>.Falla(
                    "Datos inválidos",
                    validacion.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // 2. Buscar el paciente existente
            var paciente = await _unitOfWork.Pacientes.ObtenerPorIdAsync(dto.Id);
            if (paciente == null)
                return ResultadoAccion<PacienteDTO>.Falla("Paciente no encontrado");

            // 3. Aplicar cambios y guardar
            _mapper.Map(dto, paciente);
            paciente.FechaDeModificacion = DateTime.UtcNow;

            _unitOfWork.Pacientes.Actualizar(paciente);
            await _unitOfWork.GuardarCambiosAsync();

            var dtoResultado = _mapper.Map<PacienteDTO>(paciente);
            return ResultadoAccion<PacienteDTO>.Exito(dtoResultado, "Paciente actualizado");
        }
    }
}
