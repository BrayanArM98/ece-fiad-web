using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Doctores;
using Aplicacion.Helpers;
using AutoMapper;
using Dominio.Entidades.Doctores;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Doctores.Commands.CrearDoctor
{
    /// <summary>
    /// Manejador que registra un nuevo doctor.
    /// Valida los datos, verifica que el correo no esté registrado y persiste el registro.
    /// </summary>
    public class CrearDoctorHandler
        : IRequestHandler<CrearDoctorCommand, ResultadoAccion<DoctorDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearDoctorDTO> _validador;

        public CrearDoctorHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CrearDoctorDTO> validador)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
        }

        public async Task<ResultadoAccion<DoctorDTO>> Handle(
            CrearDoctorCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Doctor;

            try
            {
                // 1. Validar datos de entrada
                var validacion = await _validador.ValidateAsync(dto, cancellationToken);
                if (!validacion.IsValid)
                {
                    var errores = string.Join(" | ", validacion.Errors.Select(e => e.ErrorMessage));
                    return ResultadoAccion<DoctorDTO>.Falla($"Datos inválidos: {errores}");
                }

                // 2. Verificar que el correo no esté registrado
                var existente = await _unitOfWork.Doctores.ObtenerPorEmailAsync(dto.Email, null);
                if (existente != null)
                {
                    return ResultadoAccion<DoctorDTO>.Falla(
                        $"Ya existe un doctor registrado con el correo '{dto.Email}'.");
                }

                // 3. Mapear y persistir
                var doctor = _mapper.Map<Doctor>(dto);
                await _unitOfWork.Doctores.AgregarAsync(doctor);
                await _unitOfWork.GuardarCambiosAsync();

                // 4. Recuperar con la especialidad cargada para devolver el DTO completo
                var doctorCreado = await _unitOfWork.Doctores.ObtenerConEspecialidadAsync(doctor.Id);
                var resultado = _mapper.Map<DoctorDTO>(doctorCreado!);

                return ResultadoAccion<DoctorDTO>.Exito(resultado, "Doctor creado exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion<DoctorDTO>.Falla($"Error al crear doctor: {ex.Message}");
            }
        }
    }
}