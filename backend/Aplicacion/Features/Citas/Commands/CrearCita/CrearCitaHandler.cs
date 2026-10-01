using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using AutoMapper;
using Dominio.Entidades.Citas;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Citas.Commands.CrearCita
{
    /// <summary>
    /// Manejador que registra una nueva cita.
    /// Regla 21: un doctor no puede tener dos citas en el mismo horario.
    /// </summary>
    public class CrearCitaHandler
        : IRequestHandler<CrearCitaCommand, ResultadoAccion<CitaDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearCitaDTO> _validador;

        public CrearCitaHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CrearCitaDTO> validador)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
        }

        public async Task<ResultadoAccion<CitaDTO>> Handle(
            CrearCitaCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Cita;

            try
            {
                // 1. Validar datos de entrada
                var validacion = await _validador.ValidateAsync(dto, cancellationToken);
                if (!validacion.IsValid)
                {
                    var errores = string.Join(" | ", validacion.Errors.Select(e => e.ErrorMessage));
                    return ResultadoAccion<CitaDTO>.Falla($"Datos inválidos: {errores}");
                }

                // 2. Validar disponibilidad horaria del doctor (regla 21)
                var ocupado = await _unitOfWork.Citas
                    .ExisteCitaEnHorarioAsync(dto.IdDoctor, dto.FechaHora, null);

                if (ocupado)
                {
                    return ResultadoAccion<CitaDTO>.Falla(
                        $"El doctor seleccionado ya tiene una cita programada en el horario {dto.FechaHora:dd/MM/yyyy HH:mm}.");
                }

                // 3. Mapear y persistir
                var cita = _mapper.Map<Cita>(dto);
                await _unitOfWork.Citas.AgregarAsync(cita);
                await _unitOfWork.GuardarCambiosAsync();

                // 4. Recuperar con relaciones para devolver el DTO completo
                var citaCreada = await _unitOfWork.Citas.ObtenerConRelacionesAsync(cita.Id);
                var resultado = _mapper.Map<CitaDTO>(citaCreada!);

                return ResultadoAccion<CitaDTO>.Exito(resultado, "Cita creada exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion<CitaDTO>.Falla($"Error al crear cita: {ex.Message}");
            }
        }
    }
}