using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Helpers;
using Aplicacion.Servicios.Interfaces;
using AutoMapper;
using Dominio.Entidades.Pacientes;
using FluentValidation;

namespace Aplicacion.Servicios.Implementaciones
{
    public class PacienteService : IPacienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearPacienteDTO> _crearValidacion;
        private readonly IValidator<ActualizarPacienteDTO> _actualizarValidator;

        public PacienteService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CrearPacienteDTO> crearValidacion,
            IValidator<ActualizarPacienteDTO> actualizarValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _crearValidacion = crearValidacion;
            _actualizarValidator = actualizarValidator;
        }

        public async Task<ResultadoAccion<PacienteDTO>> ObtenerPorIdAsync(int id)
        {
            var paciente = await _unitOfWork.Pacientes.ObtenerPorIdAsync(id);
            if (paciente == null)
                return ResultadoAccion<PacienteDTO>.Falla("Paciente no encontrado");

            var dto = _mapper.Map<PacienteDTO>(paciente);
            return ResultadoAccion<PacienteDTO>.Exito(dto);
        }

        public async Task<ResultadoAccion<IEnumerable<PacienteDTO>>> ObtenerTodosAsync()
        {
            var pacientes = await _unitOfWork.Pacientes.ObtenerTodosAsync();
            var dtos = _mapper.Map<IEnumerable<PacienteDTO>>(pacientes);
            return ResultadoAccion<IEnumerable<PacienteDTO>>.Exito(dtos);
        }

        public async Task<ResultadoAccion<PacienteDTO>> CrearAsync(CrearPacienteDTO dto)
        {
            // 1. Validar datos de entrada
            var validacion = await _crearValidacion.ValidateAsync(dto);
            if (!validacion.IsValid)
            {
                return ResultadoAccion<PacienteDTO>.Falla(
                    "Datos inválidos",
                    validacion.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // 2. Verificar duplicado por identificación
            if (await ExistePorIdentificacionAsync(dto.NumeroDocumento))
                return ResultadoAccion<PacienteDTO>.Falla("Ya existe un paciente con ese número de identificación");

            // 3. Mapear y crear paciente
            var paciente = _mapper.Map<Paciente>(dto);
            await _unitOfWork.Pacientes.AgregarAsync(paciente);
            await _unitOfWork.GuardarCambiosAsync();

            // 4. Devolver el paciente creado como DTO
            var pacienteCreado = await _unitOfWork.Pacientes.ObtenerPorIdAsync(paciente.Id);
            var dtoResultado = _mapper.Map<PacienteDTO>(pacienteCreado);
            return ResultadoAccion<PacienteDTO>.Exito(dtoResultado, "Paciente creado exitosamente");
        }

        public async Task<ResultadoAccion<PacienteDTO>> ActualizarAsync(ActualizarPacienteDTO dto)
        {
            // 1. Validar datos de entrada
            var validacion = await _actualizarValidator.ValidateAsync(dto);
            if (!validacion.IsValid)
                return ResultadoAccion<PacienteDTO>.Falla(
                    "Datos inválidos",
                    validacion.Errors.Select(e => e.ErrorMessage).ToList());

            // 2. Buscar paciente existente
            var paciente = await _unitOfWork.Pacientes.ObtenerPorIdAsync(dto.Id);
            if (paciente == null)
                return ResultadoAccion<PacienteDTO>.Falla("Paciente no encontrado");

            // 3. Aplicar cambios y guardar
            _mapper.Map(dto, paciente);
            paciente.FechaDeModificacion = DateTime.UtcNow;
            _unitOfWork.Pacientes.Actualizar(paciente);
            await _unitOfWork.GuardarCambiosAsync();

            var dtoResultado = _mapper.Map<PacienteDTO>(paciente);
            return ResultadoAccion<PacienteDTO>.Exito(dtoResultado, "Paciente actualizado");
        }

        public async Task<ResultadoAccion> EliminarAsync(int id)
        {
            var paciente = await _unitOfWork.Pacientes.ObtenerPorIdAsync(id);
            if (paciente == null)
                return ResultadoAccion.Falla("Paciente no encontrado");

            // Borrado lógico: marcamos como eliminado pero el registro se queda en la BD
            paciente.Eliminado = true;
            paciente.FechaDeEliminacion = DateTime.UtcNow;
            paciente.Activo = false;

            _unitOfWork.Pacientes.Actualizar(paciente);
            await _unitOfWork.GuardarCambiosAsync();

            return ResultadoAccion.Exito("Paciente eliminado (borrado lógico)");
        }

        public async Task<bool> ExistePorIdentificacionAsync(string identificacion)
        {
            var pacientes = await _unitOfWork.Pacientes.BuscarAsync(p => p.NumeroDocumento == identificacion);
            return pacientes.Any();
        }

        // ============================================================
        // OBTENER PACIENTES SIN HISTORIA CLÍNICA (regla 22)
        // ============================================================
        public async Task<ResultadoAccion<IEnumerable<PacienteDTO>>> ObtenerSinHistoriaClinicaAsync()
        {
            try
            {
                // Traemos todos los pacientes activos
                var todosLosPacientes = await _unitOfWork.Pacientes.ObtenerTodosAsync();

                // Filtramos los que NO tienen historia clínica activa.
                // Reutilizamos el método del repositorio de historias clínicas.
                var pacientesSinHistoria = new List<Paciente>();

                foreach (var paciente in todosLosPacientes)
                {
                    var tieneHistoria = await _unitOfWork.HistoriasClinicas
                        .ExisteHistoriaParaPacienteAsync(paciente.Id);

                    if (!tieneHistoria)
                    {
                        pacientesSinHistoria.Add(paciente);
                    }
                }

                var lista = _mapper.Map<IEnumerable<PacienteDTO>>(pacientesSinHistoria);
                return ResultadoAccion<IEnumerable<PacienteDTO>>.Exito(lista);
            }
            catch (Exception ex)
            {
                return ResultadoAccion<IEnumerable<PacienteDTO>>.Falla(
                    $"Error al obtener pacientes sin historia clínica: {ex.Message}");
            }
        }
    }
}