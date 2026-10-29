using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Reportes;
using Aplicacion.Features.Reportes.Comun;
using Aplicacion.Helpers;
using Dominio.Enumeraciones;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Reportes.Queries.ObtenerResumenGeneral
{
    /// <summary>
    /// Calcula los indicadores del periodo y del periodo anterior de la misma duración.
    /// Para un médico, los indicadores son solo de sus citas y evoluciones.
    /// </summary>
    public class ObtenerResumenGeneralHandler
        : IRequestHandler<ObtenerResumenGeneralQuery, ResultadoAccion<ResumenGeneralDTO>>
    {
        private readonly IConsultasReportes _consultas;
        private readonly IValidator<FiltroReporteDTO> _validador;
        private readonly IUsuarioActual _usuarioActual;

        public ObtenerResumenGeneralHandler(
            IConsultasReportes consultas,
            IValidator<FiltroReporteDTO> validador,
            IUsuarioActual usuarioActual)
        {
            _consultas = consultas;
            _validador = validador;
            _usuarioActual = usuarioActual;
        }

        public async Task<ResultadoAccion<ResumenGeneralDTO>> Handle(
            ObtenerResumenGeneralQuery request,
            CancellationToken cancellationToken)
        {
            var validacion = await _validador.ValidateAsync(request.Filtro, cancellationToken);
            if (!validacion.IsValid)
                return ResultadoAccion<ResumenGeneralDTO>.FallaValidacion(validacion.Errors.Select(e => e.ErrorMessage));

            var alcance = AlcanceReporte.Crear(request.Filtro, _usuarioActual, DateTime.Now);
            var anterior = Periodos.Anterior(alcance.Periodo);

            var actual = await CalcularAsync(alcance.Periodo, alcance);
            var previo = await CalcularAsync(anterior, alcance);

            var indicadores = new List<IndicadorDTO>
            {
                CalculadoraReportes.Indicador("citas", "Citas", actual.Citas, previo.Citas),
                CalculadoraReportes.Indicador("asistencia", "Asistencia", actual.Asistencia ?? 0, previo.Asistencia, "porcentaje"),
                CalculadoraReportes.Indicador("cancelaciones", "Cancelaciones", actual.Canceladas, previo.Canceladas),
                CalculadoraReportes.Indicador("pacientesAtendidos", "Pacientes atendidos", actual.PacientesAtendidos, previo.PacientesAtendidos),
                CalculadoraReportes.Indicador("evoluciones", "Evoluciones registradas", actual.Evoluciones, previo.Evoluciones)
            };

            // Las altas de pacientes no dependen de un doctor: solo en el resumen general de la clínica
            if (!alcance.EsDeUnDoctor)
            {
                var nuevos = await _consultas.ContarPacientesNuevosAsync(
                    alcance.Periodo.Desde, Periodos.HastaExclusivo(alcance.Periodo));
                var nuevosAntes = await _consultas.ContarPacientesNuevosAsync(
                    anterior.Desde, Periodos.HastaExclusivo(anterior));
                indicadores.Add(CalculadoraReportes.Indicador("pacientesNuevos", "Pacientes nuevos", nuevos, nuevosAntes));
            }

            return ResultadoAccion<ResumenGeneralDTO>.Exito(new ResumenGeneralDTO
            {
                Periodo = alcance.Periodo,
                PeriodoAnterior = anterior,
                Indicadores = indicadores
            });
        }

        private async Task<(int Citas, decimal? Asistencia, int Canceladas, int PacientesAtendidos, int Evoluciones)> CalcularAsync(
            PeriodoDTO periodo, AlcanceReporte alcance)
        {
            var hasta = Periodos.HastaExclusivo(periodo);
            var citas = await _consultas.ObtenerCitasAsync(periodo.Desde, hasta, alcance.IdDoctor, alcance.IdEspecialidad, null);
            var evoluciones = await _consultas.ObtenerEvolucionesAsync(periodo.Desde, hasta, alcance.IdDoctor);

            var completadas = citas.Count(c => c.Estado == EstadoCita.Completada);
            var noAsistio = citas.Count(c => c.Estado == EstadoCita.NoAsistio);

            return (
                citas.Count,
                CalculadoraReportes.TasaAsistencia(completadas, noAsistio),
                citas.Count(c => c.Estado == EstadoCita.Cancelada),
                citas.Where(c => c.Estado == EstadoCita.Completada).Select(c => c.IdPaciente).Distinct().Count(),
                evoluciones.Count);
        }
    }
}
