using Aplicacion.Abstracciones;
using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Helpers;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.HistoriasClinicas.Commands.ActualizarHistoria
{
    /// <summary>
    /// Manejador que actualiza una historia clínica existente.
    /// Mantiene la regla 22 excluyendo la propia historia de la verificación.
    /// </summary>
    public class ActualizarHistoriaHandler
        : IRequestHandler<ActualizarHistoriaCommand, ResultadoAccion<HistoriaClinicaDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<ActualizarHistoriaDTO> _validador;

        public ActualizarHistoriaHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<ActualizarHistoriaDTO> validador)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
        }

        public async Task<ResultadoAccion<HistoriaClinicaDTO>> Handle(
            ActualizarHistoriaCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Historia;

            try
            {
                // 1. Validar datos de entrada
                var validacion = await _validador.ValidateAsync(dto, cancellationToken);
                if (!validacion.IsValid)
                {
                    var errores = string.Join(" | ", validacion.Errors.Select(e => e.ErrorMessage));
                    return ResultadoAccion<HistoriaClinicaDTO>.Falla($"Datos inválidos: {errores}");
                }

                // 2. Verificar que la historia exista
                var historia = await _unitOfWork.HistoriasClinicas.ObtenerPorIdAsync(dto.Id);
                if (historia == null)
                {
                    return ResultadoAccion<HistoriaClinicaDTO>.Falla(
                        "La historia clínica que intenta actualizar no existe.");
                }

                // 3. Verificar regla 22 excluyendo la propia historia
                var otraHistoria = await _unitOfWork.HistoriasClinicas
                    .ExisteHistoriaParaPacienteAsync(dto.IdPaciente, dto.Id);

                if (otraHistoria)
                {
                    return ResultadoAccion<HistoriaClinicaDTO>.Falla(
                        "El paciente seleccionado ya tiene otra historia clínica activa.");
                }

                // 4. Aplicar cambios y persistir
                _mapper.Map(dto, historia);
                historia.FechaDeModificacion = DateTime.UtcNow;

                _unitOfWork.HistoriasClinicas.Actualizar(historia);
                await _unitOfWork.GuardarCambiosAsync();

                // 5. Recuperar con relaciones para el DTO completo
                var historiaActualizada = await _unitOfWork.HistoriasClinicas.ObtenerConRelacionesAsync(dto.Id);
                var resultado = _mapper.Map<HistoriaClinicaDTO>(historiaActualizada!);

                return ResultadoAccion<HistoriaClinicaDTO>.Exito(
                    resultado, "Historia clínica actualizada exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion<HistoriaClinicaDTO>.Falla(
                    $"Error al actualizar historia clínica: {ex.Message}");
            }
        }
    }
}