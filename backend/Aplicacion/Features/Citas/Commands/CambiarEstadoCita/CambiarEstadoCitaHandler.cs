using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using AutoMapper;
using Dominio.Enumeraciones;
using MediatR;

namespace Aplicacion.Features.Citas.Commands.CambiarEstadoCita
{
    /// <summary>
    /// Manejador que cambia el estado de una cita usando los métodos de negocio de la entidad.
    /// Un médico solo puede cambiar el estado de sus propias citas.
    /// Una cita cancelada ya no cambia de estado.
    /// </summary>
    public class CambiarEstadoCitaHandler
        : IRequestHandler<CambiarEstadoCitaCommand, ResultadoAccion<CitaDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUsuarioActual _usuarioActual;

        public CambiarEstadoCitaHandler(IUnitOfWork unitOfWork, IMapper mapper, IUsuarioActual usuarioActual)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _usuarioActual = usuarioActual;
        }

        public async Task<ResultadoAccion<CitaDTO>> Handle(
            CambiarEstadoCitaCommand request,
            CancellationToken cancellationToken)
        {
            var cita = await _unitOfWork.Citas.ObtenerPorIdAsync(request.Id);

            // Para un médico, una cita de otro doctor es como si no existiera
            if (cita == null || (_usuarioActual.EsMedico && cita.IdDoctor != _usuarioActual.IdDoctor))
                return ResultadoAccion<CitaDTO>.Falla("Cita no encontrada.");

            if (cita.Estado == EstadoCita.Cancelada)
                return ResultadoAccion<CitaDTO>.Falla("Una cita cancelada no puede cambiar de estado.");

            switch (request.Datos.Estado)
            {
                case EstadoCita.Confirmada:
                    cita.Confirmar();
                    break;
                case EstadoCita.Completada:
                    cita.Completar();
                    break;
                case EstadoCita.NoAsistio:
                    cita.RegistrarNoAsistencia();
                    break;
                default:
                    return ResultadoAccion<CitaDTO>.FallaValidacion(new[]
                    {
                        "El estado debe ser Confirmada, Completada o NoAsistio. Para cancelar usa la opción Cancelar."
                    });
            }

            _unitOfWork.Citas.Actualizar(cita);
            await _unitOfWork.GuardarCambiosAsync();

            var actualizada = await _unitOfWork.Citas.ObtenerConRelacionesAsync(cita.Id);
            return ResultadoAccion<CitaDTO>.Exito(
                _mapper.Map<CitaDTO>(actualizada!), "Estado de la cita actualizado.");
        }
    }
}
