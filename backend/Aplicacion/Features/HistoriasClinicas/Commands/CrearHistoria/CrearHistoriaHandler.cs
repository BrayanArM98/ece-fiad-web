using Aplicacion.Abstracciones;
using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Helpers;
using AutoMapper;
using Dominio.Entidades.HistoriasClinicas;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.HistoriasClinicas.Commands.CrearHistoria
{
    /// <summary>
    /// Manejador que registra una nueva historia clínica.
    /// Regla 22: un paciente solo puede tener una historia clínica activa.
    /// </summary>
    public class CrearHistoriaHandler
        : IRequestHandler<CrearHistoriaCommand, ResultadoAccion<HistoriaClinicaDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearHistoriaDTO> _validador;

        public CrearHistoriaHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CrearHistoriaDTO> validador)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
        }

        public async Task<ResultadoAccion<HistoriaClinicaDTO>> Handle(
            CrearHistoriaCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Historia;

            try
            {
                // 1. Validar datos de entrada
                var validacion = await _validador.ValidateAsync(dto, cancellationToken);
                if (!validacion.IsValid)
                {
                    var errores = string.Join(" | ", validacion.Errors.Select(e => e.ErrorMessage));
                    return ResultadoAccion<HistoriaClinicaDTO>.Falla($"Datos inválidos: {errores}");
                }

                // 2. Verificar que el paciente no tenga ya una historia (regla 22)
                var yaTieneHistoria = await _unitOfWork.HistoriasClinicas
                    .ExisteHistoriaParaPacienteAsync(dto.IdPaciente, null);

                if (yaTieneHistoria)
                {
                    return ResultadoAccion<HistoriaClinicaDTO>.Falla(
                        "El paciente seleccionado ya tiene una historia clínica activa. " +
                        "Cada paciente solo puede tener una.");
                }

                // 3. Mapear y persistir
                var historia = _mapper.Map<HistoriaClinica>(dto);
                await _unitOfWork.HistoriasClinicas.AgregarAsync(historia);
                await _unitOfWork.GuardarCambiosAsync();

                // 4. Recuperar con relaciones para devolver el DTO completo
                var historiaCreada = await _unitOfWork.HistoriasClinicas.ObtenerConRelacionesAsync(historia.Id);
                var resultado = _mapper.Map<HistoriaClinicaDTO>(historiaCreada!);

                return ResultadoAccion<HistoriaClinicaDTO>.Exito(resultado, "Historia clínica creada exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion<HistoriaClinicaDTO>.Falla(
                    $"Error al crear historia clínica: {ex.Message}");
            }
        }
    }
}