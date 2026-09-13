using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Doctores;
using Aplicacion.Helpers;
using Aplicacion.Servicios.Interfaces;
using AutoMapper;
using Dominio.Entidades.Doctores;
using FluentValidation;

namespace Aplicacion.Servicios.Implementaciones
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearDoctorDTO> _crearValidacion;
        private readonly IValidator<ActualizarDoctorDTO> _actualizarValidator;

        public DoctorService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CrearDoctorDTO> crearValidacion,
            IValidator<ActualizarDoctorDTO> actualizarValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _crearValidacion = crearValidacion;
            _actualizarValidator = actualizarValidator;
        }

        // ============================================================
        // OBTENER POR ID
        // ============================================================
        public async Task<ResultadoAccion<DoctorDTO>> ObtenerPorIdAsync(int id)
        {
            try
            {
                var doctor = await _unitOfWork.Doctores.ObtenerConEspecialidadAsync(id);
                if (doctor == null)
                {
                    return ResultadoAccion<DoctorDTO>.Falla("Doctor no encontrado.");
                }

                var dto = _mapper.Map<DoctorDTO>(doctor);
                return ResultadoAccion<DoctorDTO>.Exito(dto);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<DoctorDTO>.Falla($"Error al obtener doctor: {ex.Message}");
            }
        }

        // ============================================================
        // OBTENER TODOS
        // ============================================================
        public async Task<ResultadoAccion<IEnumerable<DoctorDTO>>> ObtenerTodosAsync()
        {
            try
            {
                var doctores = await _unitOfWork.Doctores.ObtenerTodosConEspecialidadAsync();
                var lista = _mapper.Map<IEnumerable<DoctorDTO>>(doctores);
                return ResultadoAccion<IEnumerable<DoctorDTO>>.Exito(lista);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<IEnumerable<DoctorDTO>>.Falla($"Error al obtener doctores: {ex.Message}");
            }
        }

        // ============================================================
        // CREAR
        // ============================================================
        public async Task<ResultadoAccion<DoctorDTO>> CrearAsync(CrearDoctorDTO dto)
        {
            try
            {
                // 1. Validación de datos con FluentValidation
                var validacion = await _crearValidacion.ValidateAsync(dto);
                if (!validacion.IsValid)
                {
                    var errores = string.Join(" | ", validacion.Errors.Select(e => e.ErrorMessage));
                    return ResultadoAccion<DoctorDTO>.Falla($"Datos inválidos: {errores}");
                }

                // 2. Validación de email único
                if (await ExistePorEmailAsync(dto.Email))
                {
                    return ResultadoAccion<DoctorDTO>.Falla(
                        $"Ya existe un doctor registrado con el correo '{dto.Email}'.");
                }

                // 3. Mapeo y persistencia
                var doctor = _mapper.Map<Doctor>(dto);
                await _unitOfWork.Doctores.AgregarAsync(doctor);
                await _unitOfWork.GuardarCambiosAsync();

                // 4. Recuperar con la especialidad ya cargada para devolver el DTO completo
                var doctorCreado = await _unitOfWork.Doctores.ObtenerConEspecialidadAsync(doctor.Id);
                var resultado = _mapper.Map<DoctorDTO>(doctorCreado!);

                return ResultadoAccion<DoctorDTO>.Exito(resultado, "Doctor creado exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion<DoctorDTO>.Falla($"Error al crear doctor: {ex.Message}");
            }
        }

        // ============================================================
        // ACTUALIZAR
        // ============================================================
        public async Task<ResultadoAccion<DoctorDTO>> ActualizarAsync(ActualizarDoctorDTO dto)
        {
            try
            {
                // 1. Validación de datos
                var validacion = await _actualizarValidator.ValidateAsync(dto);
                if (!validacion.IsValid)
                {
                    var errores = string.Join(" | ", validacion.Errors.Select(e => e.ErrorMessage));
                    return ResultadoAccion<DoctorDTO>.Falla($"Datos inválidos: {errores}");
                }

                // 2. Verificar que el doctor existe
                var doctor = await _unitOfWork.Doctores.ObtenerPorIdAsync(dto.Id);
                if (doctor == null)
                {
                    return ResultadoAccion<DoctorDTO>.Falla("Doctor no encontrado.");
                }

                // 3. Validación de email único (excluyendo el doctor actual)
                if (await ExistePorEmailAsync(dto.Email, dto.Id))
                {
                    return ResultadoAccion<DoctorDTO>.Falla(
                        $"Ya existe otro doctor con el correo '{dto.Email}'.");
                }

                // 4. Aplicar cambios y persistir
                _mapper.Map(dto, doctor);
                _unitOfWork.Doctores.Actualizar(doctor);
                await _unitOfWork.GuardarCambiosAsync();

                // 5. Recuperar con especialidad cargada para el DTO completo
                var doctorActualizado = await _unitOfWork.Doctores.ObtenerConEspecialidadAsync(doctor.Id);
                var resultado = _mapper.Map<DoctorDTO>(doctorActualizado!);

                return ResultadoAccion<DoctorDTO>.Exito(resultado, "Doctor actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion<DoctorDTO>.Falla($"Error al actualizar doctor: {ex.Message}");
            }
        }

        // ============================================================
        // ELIMINAR
        // ============================================================
        public async Task<ResultadoAccion> EliminarAsync(int id)
        {
            try
            {
                // 1. Verificar que el doctor existe (con citas para validar)
                var doctor = await _unitOfWork.Doctores.ObtenerConCitasAsync(id);
                if (doctor == null)
                {
                    return ResultadoAccion.Falla("Doctor no encontrado.");
                }

                // 2. Validar que no tenga citas asociadas (regla 20 del manual)
                if (doctor.Citas != null && doctor.Citas.Any())
                {
                    return ResultadoAccion.Falla(
                        $"No se puede eliminar el doctor porque tiene {doctor.Citas.Count} cita(s) asociada(s).");
                }

                // 3. Borrado lógico
                _unitOfWork.Doctores.Eliminar(doctor);
                await _unitOfWork.GuardarCambiosAsync();

                return ResultadoAccion.Exito("Doctor eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion.Falla($"Error al eliminar doctor: {ex.Message}");
            }
        }

        // ============================================================
        // EXISTE POR EMAIL
        // ============================================================
        public async Task<bool> ExistePorEmailAsync(string email, int? idExcluir = null)
        {
            var doctor = await _unitOfWork.Doctores.ObtenerPorEmailAsync(email, idExcluir);
            return doctor != null;
        }
    }
}