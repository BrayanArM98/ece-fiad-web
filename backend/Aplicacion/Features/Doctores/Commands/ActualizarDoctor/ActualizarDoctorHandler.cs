using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Doctores;
using Aplicacion.Helpers;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Doctores.Commands.ActualizarDoctor
{
    /// <summary>
    /// Manejador que actualiza un doctor existente.
    /// Valida la entrada, verifica que exista y que su correo no lo use otro doctor.
    /// </summary>
    public class ActualizarDoctorHandler
        : IRequestHandler<ActualizarDoctorCommand, ResultadoAccion<DoctorDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<ActualizarDoctorDTO> _validador;

        public ActualizarDoctorHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<ActualizarDoctorDTO> validador)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
        }

        public async Task<ResultadoAccion<DoctorDTO>> Handle(
            ActualizarDoctorCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Doctor;

            // 1. Validar datos de entrada
            var validacion = await _validador.ValidateAsync(dto, cancellationToken);
            if (!validacion.IsValid)
            {
                return ResultadoAccion<DoctorDTO>.FallaValidacion(
                    validacion.Errors.Select(e => e.ErrorMessage));
            }

            // 2. Verificar que el doctor exista
            var doctor = await _unitOfWork.Doctores.ObtenerPorIdAsync(dto.Id);
            if (doctor == null)
                return ResultadoAccion<DoctorDTO>.Falla("Doctor no encontrado.");

            // 3. Verificar que el correo no lo use otro doctor
            var otroConEmail = await _unitOfWork.Doctores.ObtenerPorEmailAsync(dto.Email, dto.Id);
            if (otroConEmail != null)
            {
                return ResultadoAccion<DoctorDTO>.Falla(
                    $"Ya existe otro doctor con el correo '{dto.Email}'.");
            }

            // 4. Aplicar cambios y persistir
            _mapper.Map(dto, doctor);
            _unitOfWork.Doctores.Actualizar(doctor);
            await _unitOfWork.GuardarCambiosAsync();

            // 5. Recuperar con la especialidad cargada
            var doctorActualizado = await _unitOfWork.Doctores.ObtenerConEspecialidadAsync(doctor.Id);
            var resultado = _mapper.Map<DoctorDTO>(doctorActualizado!);

            return ResultadoAccion<DoctorDTO>.Exito(resultado, "Doctor actualizado exitosamente.");
        }
    }
}