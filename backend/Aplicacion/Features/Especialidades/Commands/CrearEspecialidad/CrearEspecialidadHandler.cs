using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Especialidades;
using Aplicacion.Helpers;
using AutoMapper;
using Dominio.Entidades.Especialidades;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Especialidades.Commands.CrearEspecialidad
{
    /// <summary>
    /// Manejador que registra una nueva especialidad.
    /// Valida los datos, evita nombres duplicados y persiste el registro.
    /// </summary>
    public class CrearEspecialidadHandler
        : IRequestHandler<CrearEspecialidadCommand, ResultadoAccion<EspecialidadDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearEspecialidadDTO> _validador;

        public CrearEspecialidadHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CrearEspecialidadDTO> validador)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
        }

        public async Task<ResultadoAccion<EspecialidadDTO>> Handle(
            CrearEspecialidadCommand request,
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

            // 2. Verificar duplicado por nombre
            var existentes = await _unitOfWork.Especialidades
                .BuscarAsync(e => e.Nombre == dto.Nombre);

            if (existentes.Any())
                return ResultadoAccion<EspecialidadDTO>.Falla("Ya existe una especialidad con ese nombre");

            // 3. Mapear y persistir
            var especialidad = _mapper.Map<Especialidad>(dto);
            await _unitOfWork.Especialidades.AgregarAsync(especialidad);
            await _unitOfWork.GuardarCambiosAsync();

            // 4. Devolver la especialidad creada
            var especialidadCreada = await _unitOfWork.Especialidades.ObtenerPorIdAsync(especialidad.Id);
            var dtoResultado = _mapper.Map<EspecialidadDTO>(especialidadCreada);

            return ResultadoAccion<EspecialidadDTO>.Exito(dtoResultado, "Especialidad creada exitosamente");
        }
    }
}