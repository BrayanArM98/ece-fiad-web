using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using AutoMapper;
using Dominio.Enumeraciones;
using MediatR;

namespace Aplicacion.Features.Citas.Commands.CancelarCita
{
    /// <summary>
    /// Manejador que cambia el estado de una cita a Cancelada.
    /// Regla 23: al cancelar se registra la fecha de cancelación.
    /// </summary>
    public class CancelarCitaHandler
        : IRequestHandler<CancelarCitaCommand, ResultadoAccion<CitaDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CancelarCitaHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<CitaDTO>> Handle(
            CancelarCitaCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1. Verificar que la cita exista
                var cita = await _unitOfWork.Citas.ObtenerPorIdAsync(request.Id);
                if (cita == null)
                    return ResultadoAccion<CitaDTO>.Falla("Cita no encontrada.");

                // 2. Evitar cancelar dos veces la misma cita
                if (cita.Estado == EstadoCita.Cancelada)
                    return ResultadoAccion<CitaDTO>.Falla("La cita ya se encuentra cancelada.");

                // 3. Cambiar estado y registrar la fecha (regla 23)
                cita.Estado = EstadoCita.Cancelada;
                cita.FechaDeEliminacion = DateTime.UtcNow;

                _unitOfWork.Citas.Actualizar(cita);
                await _unitOfWork.GuardarCambiosAsync();

                // 4. Recuperar con relaciones para el DTO completo
                var citaCancelada = await _unitOfWork.Citas.ObtenerConRelacionesAsync(cita.Id);
                var resultado = _mapper.Map<CitaDTO>(citaCancelada!);

                return ResultadoAccion<CitaDTO>.Exito(resultado, "Cita cancelada exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion<CitaDTO>.Falla($"Error al cancelar cita: {ex.Message}");
            }
        }
    }
}