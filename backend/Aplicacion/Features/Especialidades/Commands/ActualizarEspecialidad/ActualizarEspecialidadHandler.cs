using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Especialidades;
using Aplicacion.Helpers;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Especialidades.Commands.ActualizarEspecialidad
{
    /// <summary>
    /// Manejador que actualiza una especialidad existente.
    /// Valida la entrada, verifica que exista y que su nuevo nombre no lo use otra especialidad.
    /// </summary>
    public class ActualizarEspecialidadHandler
        : IRequestHandler<ActualizarEspecialidadCommand, ResultadoAccion<EspecialidadDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<ActualizarEspecialidadDTO> _validador;

        public ActualizarEspecialidadHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<ActualizarEspecialidadDTO> validador)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
        }

        public async Task<ResultadoAccion<EspecialidadDTO>> Handle(
            ActualizarEspecialidadCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Especialidad;

            // 1. Validar datos de entrada
            var validacion = await _validador.ValidateAsync(dto, cancellationToken);
            if (!validacion.IsValid)
            {
                return ResultadoAccion<EspecialidadDTO>.Falla(
                    "Datos inválidos",
                    validacion.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // 2. Buscar la especialidad existente
            var especialidad = await _unitOfWork.Especialidades.ObtenerPorIdAsync(dto.Id);
            if (especialidad == null)
                return ResultadoAccion<EspecialidadDTO>.Falla("Especialidad no encontrada");

            // 3. Verificar que el nombre no lo use otra especialidad
            var duplicadas = await _unitOfWork.Especialidades
                .BuscarAsync(e => e.Nombre == dto.Nombre && e.Id != dto.Id);

            if (duplicadas.Any())
                return ResultadoAccion<EspecialidadDTO>.Falla("Ya existe otra especialidad con ese nombre");

            // 4. Aplicar cambios y guardar
            _mapper.Map(dto, especialidad);
            especialidad.FechaDeModificacion = DateTime.UtcNow;

            _unitOfWork.Especialidades.Actualizar(especialidad);
            await _unitOfWork.GuardarCambiosAsync();

            var dtoResultado = _mapper.Map<EspecialidadDTO>(especialidad);
            return ResultadoAccion<EspecialidadDTO>.Exito(dtoResultado, "Especialidad actualizada");
        }
    }
}