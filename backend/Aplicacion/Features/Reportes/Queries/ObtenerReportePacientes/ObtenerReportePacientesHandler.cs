using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Reportes;
using Aplicacion.Features.Reportes.Comun;
using Aplicacion.Helpers;
using Dominio.Enumeraciones;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Reportes.Queries.ObtenerReportePacientes
{
    /// <summary>
    /// Pacientes: altas del periodo y reparto de los pacientes activos por género, edad y grupo sanguíneo.
    /// </summary>
    public class ObtenerReportePacientesHandler
        : IRequestHandler<ObtenerReportePacientesQuery, ResultadoAccion<ReportePacientesDTO>>
    {
        private static readonly string[] OrdenGeneros =
            Enum.GetValues<Genero>().Select(CalculadoraReportes.TextoGenero).ToArray();

        private static readonly string[] OrdenGrupos =
            Enum.GetValues<GrupoSanguineo>().Select(CalculadoraReportes.TextoGrupoSanguineo).ToArray();

        private readonly IConsultasReportes _consultas;
        private readonly IValidator<FiltroReporteDTO> _validador;
        private readonly IUsuarioActual _usuarioActual;

        public ObtenerReportePacientesHandler(
            IConsultasReportes consultas,
            IValidator<FiltroReporteDTO> validador,
            IUsuarioActual usuarioActual)
        {
            _consultas = consultas;
            _validador = validador;
            _usuarioActual = usuarioActual;
        }

        public async Task<ResultadoAccion<ReportePacientesDTO>> Handle(
            ObtenerReportePacientesQuery request,
            CancellationToken cancellationToken)
        {
            var validacion = await _validador.ValidateAsync(request.Filtro, cancellationToken);
            if (!validacion.IsValid)
                return ResultadoAccion<ReportePacientesDTO>.FallaValidacion(validacion.Errors.Select(e => e.ErrorMessage));

            var hoy = DateTime.Now;
            var alcance = AlcanceReporte.Crear(request.Filtro, _usuarioActual, hoy);
            var pacientes = await _consultas.ObtenerPacientesActivosAsync();
            var nuevos = await _consultas.ContarPacientesNuevosAsync(
                alcance.Periodo.Desde, Periodos.HastaExclusivo(alcance.Periodo));

            return ResultadoAccion<ReportePacientesDTO>.Exito(new ReportePacientesDTO
            {
                Periodo = alcance.Periodo,
                PacientesActivos = pacientes.Count,
                NuevosEnPeriodo = nuevos,
                PorGenero = CalculadoraReportes.Contar(
                    pacientes.Select(p => CalculadoraReportes.TextoGenero(p.Genero)), OrdenGeneros),
                PorRangoEdad = CalculadoraReportes.Contar(
                    pacientes.Select(p => CalculadoraReportes.RangoEdad(p.FechaNacimiento, hoy)), CalculadoraReportes.RangosEdad),
                PorGrupoSanguineo = CalculadoraReportes.Contar(
                    pacientes.Select(p => CalculadoraReportes.TextoGrupoSanguineo(p.GrupoSanguineo)), OrdenGrupos)
            });
        }
    }
}
