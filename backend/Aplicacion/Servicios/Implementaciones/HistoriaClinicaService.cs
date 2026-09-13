using Aplicacion.Abstracciones;
using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Helpers;
using Aplicacion.Servicios.Interfaces;
using AutoMapper;
using Dominio.Entidades.HistoriasClinicas;
using FluentValidation;

namespace Aplicacion.Servicios.Implementaciones
{
    public class HistoriaClinicaService : IHistoriaClinicaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearHistoriaDTO> _crearValidacion;
        private readonly IValidator<ActualizarHistoriaDTO> _actualizarValidator;

        public HistoriaClinicaService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CrearHistoriaDTO> crearValidacion,
            IValidator<ActualizarHistoriaDTO> actualizarValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _crearValidacion = crearValidacion;
            _actualizarValidator = actualizarValidator;
        }

        // ============================================================
        // OBTENER POR ID
        // ============================================================
        public async Task<ResultadoAccion<HistoriaClinicaDTO>> ObtenerPorIdAsync(int id)
        {
            try
            {
                var historia = await _unitOfWork.HistoriasClinicas.ObtenerConRelacionesAsync(id);
                if (historia == null)
                {
                    return ResultadoAccion<HistoriaClinicaDTO>.Falla("Historia clínica no encontrada.");
                }

                var dto = _mapper.Map<HistoriaClinicaDTO>(historia);
                return ResultadoAccion<HistoriaClinicaDTO>.Exito(dto);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<HistoriaClinicaDTO>.Falla($"Error al obtener historia clínica: {ex.Message}");
            }
        }

        // ============================================================
        // OBTENER TODAS
        // ============================================================
        public async Task<ResultadoAccion<IEnumerable<HistoriaClinicaDTO>>> ObtenerTodasAsync()
        {
            try
            {
                var historias = await _unitOfWork.HistoriasClinicas.ObtenerTodasConRelacionesAsync();
                var lista = _mapper.Map<IEnumerable<HistoriaClinicaDTO>>(historias);
                return ResultadoAccion<IEnumerable<HistoriaClinicaDTO>>.Exito(lista);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<IEnumerable<HistoriaClinicaDTO>>.Falla($"Error al obtener historias clínicas: {ex.Message}");
            }
        }

        // ============================================================
        // CREAR
        // ============================================================
        public async Task<ResultadoAccion<HistoriaClinicaDTO>> CrearAsync(CrearHistoriaDTO dto)
        {
            try
            {
                // 1. Validación de datos con FluentValidation
                var validacion = await _crearValidacion.ValidateAsync(dto);
                if (!validacion.IsValid)
                {
                    var errores = string.Join(" | ", validacion.Errors.Select(e => e.ErrorMessage));
                    return ResultadoAccion<HistoriaClinicaDTO>.Falla($"Datos inválidos: {errores}");
                }

                // 2. Validación de unicidad por paciente (regla 22 del manual)
                if (await ExisteHistoriaParaPacienteAsync(dto.IdPaciente))
                {
                    return ResultadoAccion<HistoriaClinicaDTO>.Falla(
                        "El paciente seleccionado ya tiene una historia clínica activa. " +
                        "Cada paciente solo puede tener una.");
                }

                // 3. Mapeo y persistencia
                var historia = _mapper.Map<HistoriaClinica>(dto);
                await _unitOfWork.HistoriasClinicas.AgregarAsync(historia);
                await _unitOfWork.GuardarCambiosAsync();

                // 4. Recuperar con relaciones cargadas para devolver el DTO completo
                var historiaCreada = await _unitOfWork.HistoriasClinicas.ObtenerConRelacionesAsync(historia.Id);
                var resultado = _mapper.Map<HistoriaClinicaDTO>(historiaCreada!);

                return ResultadoAccion<HistoriaClinicaDTO>.Exito(resultado, "Historia clínica creada exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion<HistoriaClinicaDTO>.Falla($"Error al crear historia clínica: {ex.Message}");
            }
        }

        // ============================================================
        // ACTUALIZAR
        // ============================================================
        public async Task<ResultadoAccion<HistoriaClinicaDTO>> ActualizarAsync(ActualizarHistoriaDTO dto)
        {
            try
            {
                // 1. Validación de datos con FluentValidation
                var validacion = await _actualizarValidator.ValidateAsync(dto);
                if (!validacion.IsValid)
                {
                    var errores = string.Join(" | ", validacion.Errors.Select(e => e.ErrorMessage));
                    return ResultadoAccion<HistoriaClinicaDTO>.Falla($"Datos inválidos: {errores}");
                }

                // 2. Verificamos que la historia exista
                var historiaExistente = await _unitOfWork.HistoriasClinicas.ObtenerPorIdAsync(dto.Id);
                if (historiaExistente == null)
                {
                    return ResultadoAccion<HistoriaClinicaDTO>.Falla("La historia clínica que intenta actualizar no existe.");
                }

                // 3. Validación de unicidad por paciente, excluyendo la propia historia (regla 22)
                if (await ExisteHistoriaParaPacienteAsync(dto.IdPaciente, dto.Id))
                {
                    return ResultadoAccion<HistoriaClinicaDTO>.Falla(
                        "El paciente seleccionado ya tiene otra historia clínica activa.");
                }

                // 4. Mapeo sobre la entidad existente y persistencia
                _mapper.Map(dto, historiaExistente);
                historiaExistente.FechaDeModificacion = DateTime.UtcNow;

                _unitOfWork.HistoriasClinicas.Actualizar(historiaExistente);
                await _unitOfWork.GuardarCambiosAsync();

                // 5. Recuperar con relaciones cargadas para devolver el DTO completo
                var historiaActualizada = await _unitOfWork.HistoriasClinicas.ObtenerConRelacionesAsync(dto.Id);
                var resultado = _mapper.Map<HistoriaClinicaDTO>(historiaActualizada!);

                return ResultadoAccion<HistoriaClinicaDTO>.Exito(resultado, "Historia clínica actualizada exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion<HistoriaClinicaDTO>.Falla($"Error al actualizar historia clínica: {ex.Message}");
            }
        }

        // ============================================================
        // ELIMINAR (borrado lógico)
        // ============================================================
        public async Task<ResultadoAccion> EliminarAsync(int id)
        {
            try
            {
                var historia = await _unitOfWork.HistoriasClinicas.ObtenerPorIdAsync(id);
                if (historia == null)
                {
                    return ResultadoAccion.Falla("La historia clínica no existe o ya fue eliminada.");
                }

                historia.Eliminado = true;
                historia.FechaDeEliminacion = DateTime.UtcNow;

                _unitOfWork.HistoriasClinicas.Actualizar(historia);
                await _unitOfWork.GuardarCambiosAsync();

                return ResultadoAccion.Exito("Historia clínica eliminada exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion.Falla($"Error al eliminar historia clínica: {ex.Message}");
            }
        }

        // ============================================================
        // EXISTE HISTORIA PARA PACIENTE (regla 22)
        // ============================================================
        public async Task<bool> ExisteHistoriaParaPacienteAsync(int idPaciente, int? idExcluir = null)
        {
            return await _unitOfWork.HistoriasClinicas.ExisteHistoriaParaPacienteAsync(idPaciente, idExcluir);
        }
    }
}