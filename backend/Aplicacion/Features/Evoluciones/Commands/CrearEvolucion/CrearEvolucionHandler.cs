using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Helpers;
using AutoMapper;
using Dominio.Entidades.Evoluciones;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Evoluciones.Commands.CrearEvolucion
{
    /// <summary>
    /// Manejador que registra una nueva evolución dentro de una historia clínica.
    /// Si quien la registra es un médico, la evolución queda a su nombre aunque se envíe otro doctor.
    /// </summary>
    public class CrearEvolucionHandler
        : IRequestHandler<CrearEvolucionCommand, ResultadoAccion<EvolucionDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearEvolucionDTO> _validador;
        private readonly IUsuarioActual _usuarioActual;

        public CrearEvolucionHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CrearEvolucionDTO> validador,
            IUsuarioActual usuarioActual)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
            _usuarioActual = usuarioActual;
        }

        public async Task<ResultadoAccion<EvolucionDTO>> Handle(
            CrearEvolucionCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Evolucion;

            // El médico de la sesión es el autor de la evolución
            if (_usuarioActual.EsMedico && _usuarioActual.IdDoctor is int idDoctor)
                dto.IdDoctor = idDoctor;

            // 1. Validar datos de entrada
            var validacion = await _validador.ValidateAsync(dto, cancellationToken);
            if (!validacion.IsValid)
            {
                return ResultadoAccion<EvolucionDTO>.FallaValidacion(
                    validacion.Errors.Select(e => e.ErrorMessage));
            }

            // 2. Mapear y persistir
            var evolucion = _mapper.Map<Evolucion>(dto);
            await _unitOfWork.Evoluciones.AgregarAsync(evolucion);
            await _unitOfWork.GuardarCambiosAsync();

            // 3. Recuperar con relaciones para devolver el DTO completo
            var evolucionCreada = await _unitOfWork.Evoluciones.ObtenerConRelacionesAsync(evolucion.Id);
            var resultado = _mapper.Map<EvolucionDTO>(evolucionCreada!);

            return ResultadoAccion<EvolucionDTO>.Exito(resultado, "Evolución creada exitosamente.");
        }
    }
}