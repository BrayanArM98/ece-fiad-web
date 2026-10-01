using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using AutoMapper;
using Dominio.Enumeraciones;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Citas.Commands.ActualizarCita
{
    /// <summary>
    /// Manejador que actualiza una cita existente.
    /// Valida disponibilidad horaria (regla 21) y registra la fecha si se cancela (regla 23).
    /// </summary>
    public class ActualizarCitaHandler
        : IRequestHandler<ActualizarCitaCommand, ResultadoAccion<CitaDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<ActualizarCitaDTO> _validador;

        public ActualizarCitaHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<ActualizarCitaDTO> validador)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
        }

        public async Task<ResultadoAccion<CitaDTO>> Handle(
            ActualizarCitaCommand request,
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

                // 2. Verificar que la cita exista
                var cita = await _unitOfWork.Citas.ObtenerPorIdAsync(dto.Id);
                if (cita == null)
                    return ResultadoAccion<CitaDTO>.Falla("Cita no encontrada.");

                // 3. Validar disponibilidad horaria excluyendo la cita actual (regla 21)
                var ocupado = await _unitOfWork.Citas
                    .ExisteCitaEnHorarioAsync(dto.IdDoctor, dto.FechaHora, dto.Id);

                if (ocupado)
                {
                    return ResultadoAccion<CitaDTO>.Falla(
                        $"El doctor seleccionado ya tiene otra cita programada en el horario {dto.FechaHora:dd/MM/yyyy HH:mm}.");
                }

                // 4. Si cambia a Cancelada, registrar la fecha (regla 23)
                if (dto.Estado == EstadoCita.Cancelada && cita.Estado != EstadoCita.Cancelada)
                {
                    cita.FechaDeEliminacion = DateTime.UtcNow;
                }

                // 5. Aplicar cambios y persistir
                _mapper.Map(dto, cita);
                _unitOfWork.Citas.Actualizar(cita);
                await _unitOfWork.GuardarCambiosAsync();

                // 6. Recuperar con relaciones para el DTO completo
                var citaActualizada = await _unitOfWork.Citas.ObtenerConRelacionesAsync(cita.Id);
                var resultado = _mapper.Map<CitaDTO>(citaActualizada!);

                return ResultadoAccion<CitaDTO>.Exito(resultado, "Cita actualizada exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion<CitaDTO>.Falla($"Error al actualizar cita: {ex.Message}");
            }
        }
    }
}