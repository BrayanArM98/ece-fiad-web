using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Evoluciones;
using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Evoluciones.Queries.ObtenerHistorialPaciente
{
    /// <summary>
    /// Manejador que compone el historial clínico de un paciente.
    /// Antes esta lógica vivía en el controlador y dependía de dos servicios;
    /// ahora es una consulta de negocio propia que usa directamente los repositorios.
    /// </summary>
    public class ObtenerHistorialPacienteHandler
        : IRequestHandler<ObtenerHistorialPacienteQuery, ResultadoAccion<HistorialPacienteDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ObtenerHistorialPacienteHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultadoAccion<HistorialPacienteDTO>> Handle(
            ObtenerHistorialPacienteQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1. Buscar la historia clínica activa del paciente
                var historias = await _unitOfWork.HistoriasClinicas.ObtenerTodasConRelacionesAsync();
                var historia = historias
                    .FirstOrDefault(h => h.IdPaciente == request.IdPaciente && h.Activo);

                // 2. Si no tiene historia activa, devolver un historial vacío
                if (historia == null)
                {
                    return ResultadoAccion<HistorialPacienteDTO>.Exito(
                        new HistorialPacienteDTO { TieneHistoria = false },
                        "El paciente no tiene historia clínica activa.");
                }

                // 3. Obtener sus evoluciones ordenadas por fecha descendente (regla H3)
                var evoluciones = await _unitOfWork.Evoluciones
                    .ObtenerPorHistoriaClinicaAsync(historia.Id);

                var evolucionesOrdenadas = _mapper.Map<IEnumerable<EvolucionDTO>>(evoluciones)
                    .OrderByDescending(e => e.Fecha)
                    .ToList();

                // 4. Componer el historial
                var historial = new HistorialPacienteDTO
                {
                    TieneHistoria = true,
                    Historia = _mapper.Map<HistoriaClinicaDTO>(historia),
                    Evoluciones = evolucionesOrdenadas,
                    TotalEvoluciones = evolucionesOrdenadas.Count
                };

                return ResultadoAccion<HistorialPacienteDTO>.Exito(
                    historial, "Historial obtenido correctamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion<HistorialPacienteDTO>.Falla(
                    $"Error al obtener el historial del paciente: {ex.Message}");
            }
        }
    }
}