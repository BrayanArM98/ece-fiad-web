using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Reportes;
using Aplicacion.Features.Reportes.Comun;
using Aplicacion.Features.Reportes.Queries.ObtenerReporteCitas;
using Aplicacion.Features.Reportes.Queries.ObtenerReporteDiagnosticos;
using Aplicacion.Features.Reportes.Queries.ObtenerReportePacientes;
using Aplicacion.Features.Reportes.Queries.ObtenerReporteProductividad;
using Aplicacion.Features.Reportes.Queries.ObtenerResumenGeneral;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Reportes.Queries.ExportarReporte
{
    /// <summary>
    /// Obtiene el reporte con su propia query (mismas reglas de filtros y de rol),
    /// lo convierte en documento y lo exporta al formato pedido.
    /// </summary>
    public class ExportarReporteHandler
        : IRequestHandler<ExportarReporteQuery, ResultadoAccion<ArchivoReporteDTO>>
    {
        private const string TipoPdf = "application/pdf";
        private const string TipoExcel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private readonly ISender _mediator;
        private readonly IExportadorReportes _exportador;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuarioActual _usuarioActual;

        public ExportarReporteHandler(
            ISender mediator,
            IExportadorReportes exportador,
            IUnitOfWork unitOfWork,
            IUsuarioActual usuarioActual)
        {
            _mediator = mediator;
            _exportador = exportador;
            _unitOfWork = unitOfWork;
            _usuarioActual = usuarioActual;
        }

        public async Task<ResultadoAccion<ArchivoReporteDTO>> Handle(
            ExportarReporteQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Obtener el reporte con su query (valida filtros y aplica la restricción del médico)
            var (documento, periodo, mensajeError, errores) = await ObtenerDocumentoAsync(request, cancellationToken);
            if (documento == null || periodo == null)
                return ResultadoAccion<ArchivoReporteDTO>.Falla(mensajeError ?? "No se pudo generar el reporte.", errores);

            // 2. Describir el periodo y los filtros aplicados en el encabezado del archivo
            documento.Detalles.Insert(0, Periodos.Describir(periodo));
            documento.Detalles.AddRange(await DescribirFiltrosAsync(request.Filtro));

            // 3. Exportar
            var esPdf = request.Formato == FormatoExportacion.Pdf;
            var contenido = esPdf ? _exportador.GenerarPdf(documento) : _exportador.GenerarExcel(documento);
            var nombre = $"ece-fiad-{request.Tipo.ToString().ToLowerInvariant()}-{periodo.Desde:yyyyMMdd}-{periodo.Hasta:yyyyMMdd}";

            return ResultadoAccion<ArchivoReporteDTO>.Exito(new ArchivoReporteDTO
            {
                Contenido = contenido,
                TipoContenido = esPdf ? TipoPdf : TipoExcel,
                NombreArchivo = nombre + (esPdf ? ".pdf" : ".xlsx")
            });
        }

        private async Task<(DocumentoReporte? Documento, PeriodoDTO? Periodo, string? Mensaje, List<string>? Errores)> ObtenerDocumentoAsync(
            ExportarReporteQuery request,
            CancellationToken cancellationToken)
        {
            switch (request.Tipo)
            {
                case TipoReporte.Resumen:
                {
                    var r = await _mediator.Send(new ObtenerResumenGeneralQuery(request.Filtro), cancellationToken);
                    if (!r.Exitoso) return Fallo(r.Mensaje, r.Errores);
                    return (ConvertidorDocumentos.De(r.Datos!), r.Datos!.Periodo, null, null);
                }
                case TipoReporte.Citas:
                {
                    var r = await _mediator.Send(new ObtenerReporteCitasQuery(request.Filtro), cancellationToken);
                    if (!r.Exitoso) return Fallo(r.Mensaje, r.Errores);
                    return (ConvertidorDocumentos.De(r.Datos!), r.Datos!.Periodo, null, null);
                }
                case TipoReporte.Productividad:
                {
                    var r = await _mediator.Send(new ObtenerReporteProductividadQuery(request.Filtro), cancellationToken);
                    if (!r.Exitoso) return Fallo(r.Mensaje, r.Errores);
                    return (ConvertidorDocumentos.De(r.Datos!), r.Datos!.Periodo, null, null);
                }
                case TipoReporte.Pacientes:
                {
                    var r = await _mediator.Send(new ObtenerReportePacientesQuery(request.Filtro), cancellationToken);
                    if (!r.Exitoso) return Fallo(r.Mensaje, r.Errores);
                    return (ConvertidorDocumentos.De(r.Datos!), r.Datos!.Periodo, null, null);
                }
                case TipoReporte.Diagnosticos:
                {
                    var r = await _mediator.Send(new ObtenerReporteDiagnosticosQuery(request.Filtro), cancellationToken);
                    if (!r.Exitoso) return Fallo(r.Mensaje, r.Errores);
                    return (ConvertidorDocumentos.De(r.Datos!), r.Datos!.Periodo, null, null);
                }
                default:
                    return Fallo("El tipo de reporte no es válido.", null);
            }
        }

        private static (DocumentoReporte? Documento, PeriodoDTO? Periodo, string? Mensaje, List<string>? Errores) Fallo(
            string mensaje, List<string>? errores) => (null, null, mensaje, errores);

        private async Task<List<string>> DescribirFiltrosAsync(FiltroReporteDTO filtro)
        {
            var detalles = new List<string>();

            // Para un médico, el reporte siempre es de él
            var idDoctor = _usuarioActual.EsMedico ? _usuarioActual.IdDoctor : filtro.IdDoctor;
            if (idDoctor is int id && await _unitOfWork.Doctores.ObtenerPorIdAsync(id) is { } doctor)
                detalles.Add($"Doctor: {doctor.NombreCompleto}");

            if (filtro.IdEspecialidad is int idEspecialidad &&
                await _unitOfWork.Especialidades.ObtenerPorIdAsync(idEspecialidad) is { } especialidad)
                detalles.Add($"Especialidad: {especialidad.Nombre}");

            if (filtro.Estado is { } estado)
                detalles.Add($"Estado: {CalculadoraReportes.TextoEstado(estado)}");

            return detalles;
        }
    }
}
