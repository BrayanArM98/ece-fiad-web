namespace Aplicacion.DTOs.Reportes
{
    /// <summary>
    /// Contenido de un reporte listo para exportar, independiente del formato.
    /// El mismo documento se convierte en PDF o en Excel.
    /// </summary>
    public class DocumentoReporte
    {
        public string Titulo { get; set; } = string.Empty;

        /// <summary>Periodo y filtros aplicados, en texto.</summary>
        public List<string> Detalles { get; set; } = new();

        public DateTime GeneradoEl { get; set; } = DateTime.Now;

        /// <summary>Pares etiqueta-valor que se muestran como tarjetas o resumen.</summary>
        public List<(string Etiqueta, string Valor)> Indicadores { get; set; } = new();

        public List<TablaDocumento> Tablas { get; set; } = new();
    }

    /// <summary>
    /// Tabla del documento. Las celdas son texto, int o decimal,
    /// para que en Excel los números queden como números.
    /// </summary>
    public class TablaDocumento
    {
        public string Titulo { get; set; } = string.Empty;
        public List<string> Columnas { get; set; } = new();
        public List<object[]> Filas { get; set; } = new();
    }

    /// <summary>Archivo generado para descargar.</summary>
    public class ArchivoReporteDTO
    {
        public byte[] Contenido { get; set; } = Array.Empty<byte>();
        public string TipoContenido { get; set; } = string.Empty;
        public string NombreArchivo { get; set; } = string.Empty;
    }

    public enum FormatoExportacion
    {
        Pdf = 1,
        Excel = 2
    }

    public enum TipoReporte
    {
        Resumen = 1,
        Citas = 2,
        Productividad = 3,
        Pacientes = 4,
        Diagnosticos = 5
    }
}
