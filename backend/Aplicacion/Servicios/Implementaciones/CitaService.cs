using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using Aplicacion.Servicios.Interfaces;
using AutoMapper;
using Dominio.Entidades.Citas;
using Dominio.Enumeraciones;
using FluentValidation;

namespace Aplicacion.Servicios.Implementaciones
{
    public class CitaService : ICitaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearCitaDTO> _crearValidacion;
        private readonly IValidator<ActualizarCitaDTO> _actualizarValidator;

        public CitaService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CrearCitaDTO> crearValidacion,
            IValidator<ActualizarCitaDTO> actualizarValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _crearValidacion = crearValidacion;
            _actualizarValidator = actualizarValidator;
        }

        // ============================================================
        // OBTENER POR ID
        // ============================================================
        public async Task<ResultadoAccion<CitaDTO>> ObtenerPorIdAsync(int id)
        {
            try
            {
                var cita = await _unitOfWork.Citas.ObtenerConRelacionesAsync(id);
                if (cita == null)
                {
                    return ResultadoAccion<CitaDTO>.Falla("Cita no encontrada.");
                }

                var dto = _mapper.Map<CitaDTO>(cita);
                return ResultadoAccion<CitaDTO>.Exito(dto);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<CitaDTO>.Falla($"Error al obtener cita: {ex.Message}");
            }
        }

        // ============================================================
        // OBTENER TODAS
        // ============================================================
        public async Task<ResultadoAccion<IEnumerable<CitaDTO>>> ObtenerTodasAsync()
        {
            try
            {
                var citas = await _unitOfWork.Citas.ObtenerTodasConRelacionesAsync();
                var lista = _mapper.Map<IEnumerable<CitaDTO>>(citas);
                return ResultadoAccion<IEnumerable<CitaDTO>>.Exito(lista);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<IEnumerable<CitaDTO>>.Falla($"Error al obtener citas: {ex.Message}");
            }
        }

        // ============================================================
        // CREAR
        // ============================================================
        public async Task<ResultadoAccion<CitaDTO>> CrearAsync(CrearCitaDTO dto)
        {
            try
            {
                // 1. Validación de datos con FluentValidation
                var validacion = await _crearValidacion.ValidateAsync(dto);
                if (!validacion.IsValid)
                {
                    var errores = string.Join(" | ", validacion.Errors.Select(e => e.ErrorMessage));
                    return ResultadoAccion<CitaDTO>.Falla($"Datos inválidos: {errores}");
                }

                // 2. Validación de disponibilidad horaria (regla 21 del manual)
                if (await ExisteCitaEnHorarioAsync(dto.IdDoctor, dto.FechaHora))
                {
                    return ResultadoAccion<CitaDTO>.Falla(
                        $"El doctor seleccionado ya tiene una cita programada en el horario {dto.FechaHora:dd/MM/yyyy HH:mm}.");
                }

                // 3. Mapeo y persistencia
                var cita = _mapper.Map<Cita>(dto);
                await _unitOfWork.Citas.AgregarAsync(cita);
                await _unitOfWork.GuardarCambiosAsync();

                // 4. Recuperar con relaciones cargadas para devolver el DTO completo
                var citaCreada = await _unitOfWork.Citas.ObtenerConRelacionesAsync(cita.Id);
                var resultado = _mapper.Map<CitaDTO>(citaCreada!);

                return ResultadoAccion<CitaDTO>.Exito(resultado, "Cita creada exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion<CitaDTO>.Falla($"Error al crear cita: {ex.Message}");
            }
        }

        // ============================================================
        // ACTUALIZAR
        // ============================================================
        public async Task<ResultadoAccion<CitaDTO>> ActualizarAsync(ActualizarCitaDTO dto)
        {
            try
            {
                // 1. Validación de datos
                var validacion = await _actualizarValidator.ValidateAsync(dto);
                if (!validacion.IsValid)
                {
                    var errores = string.Join(" | ", validacion.Errors.Select(e => e.ErrorMessage));
                    return ResultadoAccion<CitaDTO>.Falla($"Datos inválidos: {errores}");
                }

                // 2. Verificar que la cita existe
                var cita = await _unitOfWork.Citas.ObtenerPorIdAsync(dto.Id);
                if (cita == null)
                {
                    return ResultadoAccion<CitaDTO>.Falla("Cita no encontrada.");
                }

                // 3. Validación de disponibilidad horaria (excluyendo la cita actual)
                if (await ExisteCitaEnHorarioAsync(dto.IdDoctor, dto.FechaHora, dto.Id))
                {
                    return ResultadoAccion<CitaDTO>.Falla(
                        $"El doctor seleccionado ya tiene otra cita programada en el horario {dto.FechaHora:dd/MM/yyyy HH:mm}.");
                }

                // 4. Si el estado cambia a Cancelada, registrar fecha de eliminación (regla 23)
                if (dto.Estado == EstadoCita.Cancelada && cita.Estado != EstadoCita.Cancelada)
                {
                    cita.FechaDeEliminacion = DateTime.UtcNow;
                }

                // 5. Aplicar cambios y persistir
                _mapper.Map(dto, cita);
                _unitOfWork.Citas.Actualizar(cita);
                await _unitOfWork.GuardarCambiosAsync();

                // 6. Recuperar con relaciones cargadas para el DTO completo
                var citaActualizada = await _unitOfWork.Citas.ObtenerConRelacionesAsync(cita.Id);
                var resultado = _mapper.Map<CitaDTO>(citaActualizada!);

                return ResultadoAccion<CitaDTO>.Exito(resultado, "Cita actualizada exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion<CitaDTO>.Falla($"Error al actualizar cita: {ex.Message}");
            }
        }

        // ============================================================
        // ELIMINAR
        // ============================================================
        public async Task<ResultadoAccion> EliminarAsync(int id)
        {
            try
            {
                // 1. Verificar que la cita existe
                var cita = await _unitOfWork.Citas.ObtenerPorIdAsync(id);
                if (cita == null)
                {
                    return ResultadoAccion.Falla("Cita no encontrada.");
                }

                // 2. Borrado lógico (la cita queda con Activo=false y FechaDeEliminacion)
                _unitOfWork.Citas.Eliminar(cita);
                await _unitOfWork.GuardarCambiosAsync();

                return ResultadoAccion.Exito("Cita eliminada correctamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion.Falla($"Error al eliminar cita: {ex.Message}");
            }
        }

        // ============================================================
        // EXISTE CITA EN HORARIO
        // ============================================================
        public async Task<bool> ExisteCitaEnHorarioAsync(int idDoctor, DateTime fechaHora, int? idExcluir = null)
        {
            return await _unitOfWork.Citas.ExisteCitaEnHorarioAsync(idDoctor, fechaHora, idExcluir);
        }
    }
}