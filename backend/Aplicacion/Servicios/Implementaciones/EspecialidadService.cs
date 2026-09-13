using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Especialidades;
using Aplicacion.Helpers;
using Aplicacion.Servicios.Interfaces;
using AutoMapper;
using Dominio.Entidades.Especialidades;
using FluentValidation;

namespace Aplicacion.Servicios.Implementaciones
{
    public class EspecialidadService : IEspecialidadService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearEspecialidadDTO> _crearValidacion;
        private readonly IValidator<ActualizarEspecialidadDTO> _actualizarValidator;

        public EspecialidadService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CrearEspecialidadDTO> crearValidacion,
            IValidator<ActualizarEspecialidadDTO> actualizarValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _crearValidacion = crearValidacion;
            _actualizarValidator = actualizarValidator;
        }

        public async Task<ResultadoAccion<EspecialidadDTO>> ObtenerPorIdAsync(int id)
        {
            // Trae la especialidad CON sus doctores para que CantidadDoctores se calcule bien
            var especialidad = await _unitOfWork.Especialidades.ObtenerConDoctoresAsync(id);
            if (especialidad == null)
                return ResultadoAccion<EspecialidadDTO>.Falla("Especialidad no encontrada");

            var dto = _mapper.Map<EspecialidadDTO>(especialidad);
            return ResultadoAccion<EspecialidadDTO>.Exito(dto);
        }

        public async Task<ResultadoAccion<IEnumerable<EspecialidadDTO>>> ObtenerTodosAsync()
        {
            // Trae todas las especialidades CON sus doctores para que CantidadDoctores funcione en el listado
            var especialidades = await _unitOfWork.Especialidades.ObtenerTodosConDoctoresAsync();
            var dtos = _mapper.Map<IEnumerable<EspecialidadDTO>>(especialidades);
            return ResultadoAccion<IEnumerable<EspecialidadDTO>>.Exito(dtos);
        }

        public async Task<ResultadoAccion<EspecialidadDTO>> CrearAsync(CrearEspecialidadDTO dto)
        {
            // 1. Validar datos de entrada
            var validacion = await _crearValidacion.ValidateAsync(dto);
            if (!validacion.IsValid)
            {
                return ResultadoAccion<EspecialidadDTO>.Falla(
                    "Datos inválidos",
                    validacion.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // 2. Verificar duplicado por nombre
            if (await ExistePorNombreAsync(dto.Nombre))
                return ResultadoAccion<EspecialidadDTO>.Falla("Ya existe una especialidad con ese nombre");

            // 3. Mapear y crear
            var especialidad = _mapper.Map<Especialidad>(dto);
            await _unitOfWork.Especialidades.AgregarAsync(especialidad);
            await _unitOfWork.GuardarCambiosAsync();

            // 4. Devolver la especialidad creada como DTO
            var especialidadCreada = await _unitOfWork.Especialidades.ObtenerPorIdAsync(especialidad.Id);
            var dtoResultado = _mapper.Map<EspecialidadDTO>(especialidadCreada);
            return ResultadoAccion<EspecialidadDTO>.Exito(dtoResultado, "Especialidad creada exitosamente");
        }

        public async Task<ResultadoAccion<EspecialidadDTO>> ActualizarAsync(ActualizarEspecialidadDTO dto)
        {
            // 1. Validar datos de entrada
            var validacion = await _actualizarValidator.ValidateAsync(dto);
            if (!validacion.IsValid)
                return ResultadoAccion<EspecialidadDTO>.Falla(
                    "Datos inválidos",
                    validacion.Errors.Select(e => e.ErrorMessage).ToList());

            // 2. Buscar especialidad existente
            var especialidad = await _unitOfWork.Especialidades.ObtenerPorIdAsync(dto.Id);
            if (especialidad == null)
                return ResultadoAccion<EspecialidadDTO>.Falla("Especialidad no encontrada");

            // 3. Verificar que el nombre no esté usado por otra especialidad
            if (await ExistePorNombreAsync(dto.Nombre, dto.Id))
                return ResultadoAccion<EspecialidadDTO>.Falla("Ya existe otra especialidad con ese nombre");

            // 4. Aplicar cambios y guardar
            _mapper.Map(dto, especialidad);
            especialidad.FechaDeModificacion = DateTime.UtcNow;
            _unitOfWork.Especialidades.Actualizar(especialidad);
            await _unitOfWork.GuardarCambiosAsync();

            var dtoResultado = _mapper.Map<EspecialidadDTO>(especialidad);
            return ResultadoAccion<EspecialidadDTO>.Exito(dtoResultado, "Especialidad actualizada");
        }

        public async Task<ResultadoAccion> EliminarAsync(int id)
        {
            // Trae la especialidad CON sus doctores para verificar la restricción
            var especialidad = await _unitOfWork.Especialidades.ObtenerConDoctoresAsync(id);
            if (especialidad == null)
                return ResultadoAccion.Falla("Especialidad no encontrada");

            // Restricción: no permitir eliminar si tiene doctores asociados
            if (especialidad.Doctores != null && especialidad.Doctores.Any())
            {
                return ResultadoAccion.Falla(
                    "No se puede eliminar la especialidad porque tiene doctores asociados. " +
                    "Primero debe reasignar o eliminar los doctores asociados.");
            }

            // Borrado lógico
            especialidad.Eliminado = true;
            especialidad.FechaDeEliminacion = DateTime.UtcNow;
            especialidad.Activo = false;

            _unitOfWork.Especialidades.Actualizar(especialidad);
            await _unitOfWork.GuardarCambiosAsync();

            return ResultadoAccion.Exito("Especialidad eliminada (borrado lógico)");
        }

        public async Task<bool> ExistePorNombreAsync(string nombre, int? idExcluir = null)
        {
            var especialidades = await _unitOfWork.Especialidades.BuscarAsync(e =>
                e.Nombre == nombre && (idExcluir == null || e.Id != idExcluir));
            return especialidades.Any();
        }
    }
}