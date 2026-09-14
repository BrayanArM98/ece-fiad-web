using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Helpers;
using AutoMapper;
using Dominio.Entidades.Pacientes;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Pacientes.Commands.CrearPaciente
{
    /// <summary>
    /// Manejador que registra un nuevo paciente.
    /// Valida los datos, verifica duplicados por número de documento y persiste el registro.
    /// </summary>
    public class CrearPacienteHandler
        : IRequestHandler<CrearPacienteCommand, ResultadoAccion<PacienteDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearPacienteDTO> _validador;

        public CrearPacienteHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CrearPacienteDTO> validador)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
        }

        public async Task<ResultadoAccion<PacienteDTO>> Handle(
            CrearPacienteCommand request,
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

            // 2. Verificar duplicado por número de documento
            var existentes = await _unitOfWork.Pacientes
                .BuscarAsync(p => p.NumeroDocumento == dto.NumeroDocumento);

            if (existentes.Any())
                return ResultadoAccion<PacienteDTO>.Falla(
                    "Ya existe un paciente con ese número de identificación");

            // 3. Mapear y persistir
            var paciente = _mapper.Map<Paciente>(dto);
            await _unitOfWork.Pacientes.AgregarAsync(paciente);
            await _unitOfWork.GuardarCambiosAsync();

            // 4. Devolver el paciente creado
            var pacienteCreado = await _unitOfWork.Pacientes.ObtenerPorIdAsync(paciente.Id);
            var dtoResultado = _mapper.Map<PacienteDTO>(pacienteCreado);

            return ResultadoAccion<PacienteDTO>.Exito(dtoResultado, "Paciente creado exitosamente");
        }
    }
}