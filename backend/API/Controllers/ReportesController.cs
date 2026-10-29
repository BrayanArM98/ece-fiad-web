using Aplicacion.DTOs.Reportes;
using Aplicacion.Features.Reportes.Queries.ExportarReporte;
using Aplicacion.Features.Reportes.Queries.ObtenerReporteCitas;
using Aplicacion.Features.Reportes.Queries.ObtenerReporteDiagnosticos;
using Aplicacion.Features.Reportes.Queries.ObtenerReportePacientes;
using Aplicacion.Features.Reportes.Queries.ObtenerReporteProductividad;
using Aplicacion.Features.Reportes.Queries.ObtenerResumenGeneral;
using Aplicacion.Seguridad;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers;

/// <summary>
/// Reportes del expediente clínico (specs/reportes.md).
/// Todos aceptan los filtros desde, hasta, idDoctor, idEspecialidad y estado; sin fechas cubren el mes actual.
/// Cada reporte se puede descargar en PDF o Excel con /exportar?formato=Pdf|Excel.
/// Acceso: el administrador ve todo, la recepción solo el reporte de citas
/// y el médico solo sus propias estadísticas.
/// </summary>
public class ReportesController : ControladorBase
{
    private readonly IMediator _mediator;

    public ReportesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Indicadores generales del periodo comparados con el periodo anterior.</summary>
    [Authorize(Roles = Roles.AdministradorOMedico)]
    [HttpGet("resumen")]
    public async Task<IActionResult> Resumen([FromQuery] FiltroReporteDTO filtro) =>
        MapearResultado(await _mediator.Send(new ObtenerResumenGeneralQuery(filtro)));

    [Authorize(Roles = Roles.AdministradorOMedico)]
    [HttpGet("resumen/exportar")]
    public Task<IActionResult> ExportarResumen([FromQuery] FiltroReporteDTO filtro, [FromQuery] FormatoExportacion formato = FormatoExportacion.Pdf) =>
        Exportar(TipoReporte.Resumen, formato, filtro);

    /// <summary>Citas por estado, por día o semana, por doctor y por especialidad.</summary>
    [Authorize(Roles = Roles.Personal)]
    [HttpGet("citas")]
    public async Task<IActionResult> Citas([FromQuery] FiltroReporteDTO filtro) =>
        MapearResultado(await _mediator.Send(new ObtenerReporteCitasQuery(filtro)));

    [Authorize(Roles = Roles.Personal)]
    [HttpGet("citas/exportar")]
    public Task<IActionResult> ExportarCitas([FromQuery] FiltroReporteDTO filtro, [FromQuery] FormatoExportacion formato = FormatoExportacion.Pdf) =>
        Exportar(TipoReporte.Citas, formato, filtro);

    /// <summary>Citas, inasistencias, evoluciones y tasa de asistencia de cada doctor.</summary>
    [Authorize(Roles = Roles.AdministradorOMedico)]
    [HttpGet("productividad")]
    public async Task<IActionResult> Productividad([FromQuery] FiltroReporteDTO filtro) =>
        MapearResultado(await _mediator.Send(new ObtenerReporteProductividadQuery(filtro)));

    [Authorize(Roles = Roles.AdministradorOMedico)]
    [HttpGet("productividad/exportar")]
    public Task<IActionResult> ExportarProductividad([FromQuery] FiltroReporteDTO filtro, [FromQuery] FormatoExportacion formato = FormatoExportacion.Pdf) =>
        Exportar(TipoReporte.Productividad, formato, filtro);

    /// <summary>Pacientes nuevos del periodo y reparto por género, edad y grupo sanguíneo.</summary>
    [Authorize(Roles = Roles.Administrador)]
    [HttpGet("pacientes")]
    public async Task<IActionResult> Pacientes([FromQuery] FiltroReporteDTO filtro) =>
        MapearResultado(await _mediator.Send(new ObtenerReportePacientesQuery(filtro)));

    [Authorize(Roles = Roles.Administrador)]
    [HttpGet("pacientes/exportar")]
    public Task<IActionResult> ExportarPacientes([FromQuery] FiltroReporteDTO filtro, [FromQuery] FormatoExportacion formato = FormatoExportacion.Pdf) =>
        Exportar(TipoReporte.Pacientes, formato, filtro);

    /// <summary>Los 10 diagnósticos más frecuentes en las evoluciones del periodo.</summary>
    [Authorize(Roles = Roles.AdministradorOMedico)]
    [HttpGet("diagnosticos")]
    public async Task<IActionResult> Diagnosticos([FromQuery] FiltroReporteDTO filtro) =>
        MapearResultado(await _mediator.Send(new ObtenerReporteDiagnosticosQuery(filtro)));

    [Authorize(Roles = Roles.AdministradorOMedico)]
    [HttpGet("diagnosticos/exportar")]
    public Task<IActionResult> ExportarDiagnosticos([FromQuery] FiltroReporteDTO filtro, [FromQuery] FormatoExportacion formato = FormatoExportacion.Pdf) =>
        Exportar(TipoReporte.Diagnosticos, formato, filtro);

    private async Task<IActionResult> Exportar(TipoReporte tipo, FormatoExportacion formato, FiltroReporteDTO filtro)
    {
        var resultado = await _mediator.Send(new ExportarReporteQuery(tipo, formato, filtro));
        if (!resultado.Exitoso || resultado.Datos == null)
            return MapearResultado(resultado);

        return File(resultado.Datos.Contenido, resultado.Datos.TipoContenido, resultado.Datos.NombreArchivo);
    }
}
