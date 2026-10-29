using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Helpers;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Evoluciones.Commands.ActualizarEvolucion
{
    /// <summary>
    /// Manejador que actualiza una evolución existente.
    /// Un médico solo puede editar las evoluciones que él registró y no puede cambiar su autor.
    /// </summary>
    public class ActualizarEvolucionHandler
        : IRequestHandler<ActualizarEvolucionCommand, ResultadoAccion<EvolucionDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<ActualizarEvolucionDTO> _validador;
        private readonly IUsuarioActual _usuarioActual;

        public ActualizarEvolucionHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<ActualizarEvolucionDTO> validador,
            IUsuarioActual usuarioActual)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
            _usuarioActual = usuarioActual;
        }

        public async Task<ResultadoAccion<EvolucionDTO>> Handle(
            ActualizarEvolucionCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Evolucion;

            // 1. Validar datos de entrada
            var validacion = await _validador.ValidateAsync(dto, cancellationToken);
            if (!validacion.IsValid)
            {
                return ResultadoAccion<EvolucionDTO>.FallaValidacion(
                    validacion.Errors.Select(e => e.ErrorMessage));
            }

            // 2. Verificar que la evolución exista
            var evolucion = await _unitOfWork.Evoluciones.ObtenerPorIdAsync(dto.Id);
            if (evolucion == null)
            {
                return ResultadoAccion<EvolucionDTO>.Falla(
                    "La evolución que intenta actualizar no existe.");
            }

            if (_usuarioActual.EsMedico)
            {
                if (evolucion.IdDoctor != _usuarioActual.IdDoctor)
                    return ResultadoAccion<EvolucionDTO>.Falla(
                        "Solo puedes editar las evoluciones que registraste.");

                // El autor no cambia
                dto.IdDoctor = evolucion.IdDoctor;
            }

            // 3. Aplicar cambios y persistir
            _mapper.Map(dto, evolucion);
            evolucion.FechaDeModificacion = DateTime.UtcNow;

            _unitOfWork.Evoluciones.Actualizar(evolucion);
            await _unitOfWork.GuardarCambiosAsync();

            // 4. Recuperar con relaciones para el DTO completo
            var evolucionActualizada = await _unitOfWork.Evoluciones.ObtenerConRelacionesAsync(dto.Id);
            var resultado = _mapper.Map<EvolucionDTO>(evolucionActualizada!);

            return ResultadoAccion<EvolucionDTO>.Exito(resultado, "Evolución actualizada exitosamente.");
        }
    }
}