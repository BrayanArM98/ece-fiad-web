using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Helpers;
using Aplicacion.Servicios.Interfaces;
using AutoMapper;
using Dominio.Entidades.Evoluciones;
using FluentValidation;

namespace Aplicacion.Servicios.Implementaciones
{
    public class EvolucionService : IEvolucionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearEvolucionDTO> _crearValidacion;
        private readonly IValidator<ActualizarEvolucionDTO> _actualizarValidator;

        public EvolucionService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CrearEvolucionDTO> crearValidacion,
            IValidator<ActualizarEvolucionDTO> actualizarValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _crearValidacion = crearValidacion;
            _actualizarValidator = actualizarValidator;
        }

        // ============================================================
        // OBTENER POR ID
        // ============================================================
        public async Task<ResultadoAccion<EvolucionDTO>> ObtenerPorIdAsync(int id)
        {
            try
            {
                var evolucion = await _unitOfWork.Evoluciones.ObtenerConRelacionesAsync(id);
                if (evolucion == null)
                {
                    return ResultadoAccion<EvolucionDTO>.Falla("Evolución no encontrada.");
                }

                var dto = _mapper.Map<EvolucionDTO>(evolucion);
                return ResultadoAccion<EvolucionDTO>.Exito(dto);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<EvolucionDTO>.Falla($"Error al obtener evolución: {ex.Message}");
            }
        }

        // ============================================================
        // OBTENER TODAS
        // ============================================================
        public async Task<ResultadoAccion<IEnumerable<EvolucionDTO>>> ObtenerTodasAsync()
        {
            try
            {
                var evoluciones = await _unitOfWork.Evoluciones.ObtenerTodasConRelacionesAsync();
                var lista = _mapper.Map<IEnumerable<EvolucionDTO>>(evoluciones);
                return ResultadoAccion<IEnumerable<EvolucionDTO>>.Exito(lista);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<IEnumerable<EvolucionDTO>>.Falla($"Error al obtener evoluciones: {ex.Message}");
            }
        }

        // ============================================================
        // OBTENER POR HISTORIA CLÍNICA (regla 27)
        // ============================================================
        public async Task<ResultadoAccion<IEnumerable<EvolucionDTO>>> ObtenerPorHistoriaClinicaAsync(int idHistoriaClinica)
        {
            try
            {
                var evoluciones = await _unitOfWork.Evoluciones.ObtenerPorHistoriaClinicaAsync(idHistoriaClinica);
                var lista = _mapper.Map<IEnumerable<EvolucionDTO>>(evoluciones);
                return ResultadoAccion<IEnumerable<EvolucionDTO>>.Exito(lista);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<IEnumerable<EvolucionDTO>>.Falla(
                    $"Error al obtener evoluciones de la historia clínica: {ex.Message}");
            }
        }

        // ============================================================
        // CONTAR POR PACIENTE (regla 29)
        // ============================================================
        public async Task<ResultadoAccion<int>> ContarPorPacienteAsync(int idPaciente)
        {
            try
            {
                var cantidad = await _unitOfWork.Evoluciones.ContarPorPacienteAsync(idPaciente);
                return ResultadoAccion<int>.Exito(cantidad);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<int>.Falla($"Error al contar evoluciones: {ex.Message}");
            }
        }

        // ============================================================
        // CREAR
        // ============================================================
        public async Task<ResultadoAccion<EvolucionDTO>> CrearAsync(CrearEvolucionDTO dto)
        {
            try
            {
                // 1. Validación de datos con FluentValidation
                var validacion = await _crearValidacion.ValidateAsync(dto);
                if (!validacion.IsValid)
                {
                    var errores = string.Join(" | ", validacion.Errors.Select(e => e.ErrorMessage));
                    return ResultadoAccion<EvolucionDTO>.Falla($"Datos inválidos: {errores}");
                }

                // 2. Mapeo y persistencia
                var evolucion = _mapper.Map<Evolucion>(dto);
                await _unitOfWork.Evoluciones.AgregarAsync(evolucion);
                await _unitOfWork.GuardarCambiosAsync();

                // 3. Recuperar con relaciones cargadas para devolver el DTO completo
                var evolucionCreada = await _unitOfWork.Evoluciones.ObtenerConRelacionesAsync(evolucion.Id);
                var resultado = _mapper.Map<EvolucionDTO>(evolucionCreada!);

                return ResultadoAccion<EvolucionDTO>.Exito(resultado, "Evolución creada exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion<EvolucionDTO>.Falla($"Error al crear evolución: {ex.Message}");
            }
        }

        // ============================================================
        // ACTUALIZAR
        // ============================================================
        public async Task<ResultadoAccion<EvolucionDTO>> ActualizarAsync(ActualizarEvolucionDTO dto)
        {
            try
            {
                // 1. Validación de datos con FluentValidation
                var validacion = await _actualizarValidator.ValidateAsync(dto);
                if (!validacion.IsValid)
                {
                    var errores = string.Join(" | ", validacion.Errors.Select(e => e.ErrorMessage));
                    return ResultadoAccion<EvolucionDTO>.Falla($"Datos inválidos: {errores}");
                }

                // 2. Verificamos que la evolución exista
                var evolucionExistente = await _unitOfWork.Evoluciones.ObtenerPorIdAsync(dto.Id);
                if (evolucionExistente == null)
                {
                    return ResultadoAccion<EvolucionDTO>.Falla("La evolución que intenta actualizar no existe.");
                }

                // 3. Mapeo sobre la entidad existente y persistencia
                _mapper.Map(dto, evolucionExistente);
                evolucionExistente.FechaDeModificacion = DateTime.UtcNow;

                _unitOfWork.Evoluciones.Actualizar(evolucionExistente);
                await _unitOfWork.GuardarCambiosAsync();

                // 4. Recuperar con relaciones cargadas para devolver el DTO completo
                var evolucionActualizada = await _unitOfWork.Evoluciones.ObtenerConRelacionesAsync(dto.Id);
                var resultado = _mapper.Map<EvolucionDTO>(evolucionActualizada!);

                return ResultadoAccion<EvolucionDTO>.Exito(resultado, "Evolución actualizada exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion<EvolucionDTO>.Falla($"Error al actualizar evolución: {ex.Message}");
            }
        }

        // ============================================================
        // ELIMINAR (borrado lógico)
        // ============================================================
        public async Task<ResultadoAccion> EliminarAsync(int id)
        {
            try
            {
                var evolucion = await _unitOfWork.Evoluciones.ObtenerPorIdAsync(id);
                if (evolucion == null)
                {
                    return ResultadoAccion.Falla("La evolución no existe o ya fue eliminada.");
                }

                evolucion.Eliminado = true;
                evolucion.FechaDeEliminacion = DateTime.UtcNow;

                _unitOfWork.Evoluciones.Actualizar(evolucion);
                await _unitOfWork.GuardarCambiosAsync();

                return ResultadoAccion.Exito("Evolución eliminada exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion.Falla($"Error al eliminar evolución: {ex.Message}");
            }
        }
    }
}