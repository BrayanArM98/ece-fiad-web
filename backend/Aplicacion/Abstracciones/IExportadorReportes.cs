using Aplicacion.DTOs.Reportes;

namespace Aplicacion.Abstracciones
{
    /// <summary>
    /// Convierte un documento de reporte en archivo. La implementación usa QuestPDF y ClosedXML.
    /// </summary>
    public interface IExportadorReportes
    {
        byte[] GenerarPdf(DocumentoReporte documento);
        byte[] GenerarExcel(DocumentoReporte documento);
    }
}
