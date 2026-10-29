using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Reportes;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infraestructura.Reportes
{
    /// <summary>
    /// Genera los archivos de los reportes: PDF con QuestPDF y Excel con ClosedXML.
    /// Ambos formatos muestran el mismo contenido: encabezado, indicadores y tablas.
    /// </summary>
    public class ExportadorReportes : IExportadorReportes
    {
        private const string ColorPrimario = "#0E7490";
        private const string ColorPrimarioSuave = "#E0F2F7";
        private const string ColorTexto = "#0F172A";
        private const string ColorSecundario = "#64748B";
        private const string ColorBorde = "#E2E8F0";
        private const string ColorFilaAlterna = "#F8FAFC";
        private const string ColorBlanco = "#FFFFFF";

        static ExportadorReportes()
        {
            // Licencia gratuita de QuestPDF para uso académico y empresas pequeñas
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // ============================================
        // PDF
        // ============================================

        public byte[] GenerarPdf(DocumentoReporte documento)
        {
            return Document.Create(contenedor =>
            {
                contenedor.Page(pagina =>
                {
                    pagina.Size(PageSizes.Letter);
                    pagina.Margin(36);
                    pagina.DefaultTextStyle(estilo => estilo.FontSize(10).FontColor(ColorTexto));

                    pagina.Header().Element(c => EncabezadoPdf(c, documento));

                    pagina.Content().PaddingVertical(14).Column(columna =>
                    {
                        columna.Spacing(16);

                        if (documento.Indicadores.Count > 0)
                            columna.Item().Element(c => IndicadoresPdf(c, documento.Indicadores));

                        foreach (var tabla in documento.Tablas)
                            columna.Item().Element(c => TablaPdf(c, tabla));
                    });

                    pagina.Footer().AlignCenter().Text(texto =>
                    {
                        texto.DefaultTextStyle(estilo => estilo.FontSize(8).FontColor(ColorSecundario));
                        texto.Span("Página ");
                        texto.CurrentPageNumber();
                        texto.Span(" de ");
                        texto.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        private static void EncabezadoPdf(IContainer contenedor, DocumentoReporte documento)
        {
            contenedor.Column(columna =>
            {
                columna.Item().Row(fila =>
                {
                    fila.RelativeItem().Column(izquierda =>
                    {
                        izquierda.Item().Text("ECE-FIAD · Expediente Clínico Electrónico")
                            .FontSize(9).FontColor(ColorPrimario).SemiBold();
                        izquierda.Item().Text(documento.Titulo).FontSize(18).Bold();

                        foreach (var detalle in documento.Detalles)
                            izquierda.Item().Text(detalle).FontSize(9).FontColor(ColorSecundario);
                    });

                    fila.ConstantItem(150).AlignRight().Text($"Generado el {documento.GeneradoEl:dd/MM/yyyy HH:mm}")
                        .FontSize(8).FontColor(ColorSecundario);
                });

                columna.Item().PaddingTop(8).LineHorizontal(1).LineColor(ColorBorde);
            });
        }

        private static void IndicadoresPdf(IContainer contenedor, List<(string Etiqueta, string Valor)> indicadores)
        {
            contenedor.Row(fila =>
            {
                fila.Spacing(8);
                foreach (var (etiqueta, valor) in indicadores)
                {
                    fila.RelativeItem().Background(ColorPrimarioSuave).Padding(8).Column(tarjeta =>
                    {
                        tarjeta.Item().Text(etiqueta).FontSize(8).FontColor(ColorSecundario);
                        tarjeta.Item().Text(valor).FontSize(15).Bold().FontColor(ColorPrimario);
                    });
                }
            });
        }

        private static void TablaPdf(IContainer contenedor, TablaDocumento tabla)
        {
            contenedor.Column(columna =>
            {
                columna.Item().PaddingBottom(6).Text(tabla.Titulo).FontSize(12).SemiBold();

                if (tabla.Filas.Count == 0)
                {
                    columna.Item().Text("Sin datos en el periodo.").Italic().FontColor(ColorSecundario);
                    return;
                }

                columna.Item().Table(t =>
                {
                    t.ColumnsDefinition(definicion =>
                    {
                        // La primera columna (nombre, estado, fecha...) es más ancha
                        definicion.RelativeColumn(3);
                        for (var i = 1; i < tabla.Columnas.Count; i++)
                            definicion.RelativeColumn(2);
                    });

                    t.Header(encabezado =>
                    {
                        for (var i = 0; i < tabla.Columnas.Count; i++)
                        {
                            var celda = encabezado.Cell().Background(ColorPrimario).PaddingVertical(4).PaddingHorizontal(5);
                            (i == 0 ? celda : celda.AlignRight())
                                .Text(tabla.Columnas[i]).FontSize(8.5f).SemiBold().FontColor(Colors.White);
                        }
                    });

                    for (var r = 0; r < tabla.Filas.Count; r++)
                    {
                        var fondo = r % 2 == 0 ? ColorBlanco : ColorFilaAlterna;
                        var valores = tabla.Filas[r];

                        for (var i = 0; i < tabla.Columnas.Count; i++)
                        {
                            var valor = i < valores.Length ? valores[i] : null;
                            var celda = t.Cell().Background(fondo).BorderBottom(0.5f).BorderColor(ColorBorde)
                                .PaddingVertical(3).PaddingHorizontal(5);

                            (EsNumero(valor) ? celda.AlignRight() : celda).Text(FormatearCelda(valor)).FontSize(9);
                        }
                    }
                });
            });
        }

        // ============================================
        // EXCEL
        // ============================================

        public byte[] GenerarExcel(DocumentoReporte documento)
        {
            using var libro = new XLWorkbook();

            // Hoja de resumen: encabezado, filtros e indicadores
            var resumen = libro.Worksheets.Add("Resumen");
            var titulo = resumen.Cell(1, 1);
            titulo.Value = $"ECE-FIAD · {documento.Titulo}";
            titulo.Style.Font.Bold = true;
            titulo.Style.Font.FontSize = 16;
            titulo.Style.Font.FontColor = XLColor.FromHtml(ColorPrimario);

            var renglon = 2;
            foreach (var detalle in documento.Detalles)
                resumen.Cell(renglon++, 1).Value = detalle;
            resumen.Cell(renglon++, 1).Value = $"Generado el {documento.GeneradoEl:dd/MM/yyyy HH:mm}";

            if (documento.Indicadores.Count > 0)
            {
                renglon++;
                EncabezadoExcel(resumen, renglon, new[] { "Indicador", "Valor" });
                foreach (var (etiqueta, valor) in documento.Indicadores)
                {
                    renglon++;
                    resumen.Cell(renglon, 1).Value = etiqueta;
                    resumen.Cell(renglon, 2).Value = valor;
                }
            }

            resumen.Columns().AdjustToContents();

            // Una hoja por tabla
            var nombres = new HashSet<string> { "Resumen" };
            foreach (var tabla in documento.Tablas)
            {
                var hoja = libro.Worksheets.Add(NombreHoja(tabla.Titulo, nombres));
                hoja.Cell(1, 1).Value = tabla.Titulo;
                hoja.Cell(1, 1).Style.Font.Bold = true;
                hoja.Cell(1, 1).Style.Font.FontSize = 13;

                EncabezadoExcel(hoja, 3, tabla.Columnas);

                for (var r = 0; r < tabla.Filas.Count; r++)
                {
                    var valores = tabla.Filas[r];
                    for (var i = 0; i < tabla.Columnas.Count && i < valores.Length; i++)
                    {
                        var celda = hoja.Cell(4 + r, 1 + i);
                        switch (valores[i])
                        {
                            case int entero:
                                celda.Value = (double)entero;
                                break;
                            case decimal numero:
                                celda.Value = (double)numero;
                                celda.Style.NumberFormat.Format = "0.0";
                                break;
                            default:
                                celda.Value = valores[i]?.ToString() ?? string.Empty;
                                break;
                        }
                    }
                }

                if (tabla.Filas.Count == 0)
                    hoja.Cell(4, 1).Value = "Sin datos en el periodo.";

                hoja.SheetView.FreezeRows(3);
                hoja.Columns().AdjustToContents();
            }

            using var memoria = new MemoryStream();
            libro.SaveAs(memoria);
            return memoria.ToArray();
        }

        private static void EncabezadoExcel(IXLWorksheet hoja, int renglon, IReadOnlyList<string> columnas)
        {
            for (var i = 0; i < columnas.Count; i++)
            {
                var celda = hoja.Cell(renglon, 1 + i);
                celda.Value = columnas[i];
                celda.Style.Font.Bold = true;
                celda.Style.Font.FontColor = XLColor.White;
                celda.Style.Fill.BackgroundColor = XLColor.FromHtml(ColorPrimario);
            }
        }

        /// <summary>Nombre válido y único de hoja: máximo 31 caracteres y sin símbolos prohibidos.</summary>
        private static string NombreHoja(string titulo, HashSet<string> usados)
        {
            var limpio = new string(titulo.Where(c => !"[]:*?/\\".Contains(c)).ToArray()).Trim();
            if (limpio.Length > 28) limpio = limpio[..28];
            if (limpio.Length == 0) limpio = "Tabla";

            var nombre = limpio;
            for (var n = 2; !usados.Add(nombre); n++)
                nombre = $"{limpio} {n}";

            return nombre;
        }

        // ============================================
        // COMUNES
        // ============================================

        private static bool EsNumero(object? valor) => valor is int or decimal or double;

        private static string FormatearCelda(object? valor) => valor switch
        {
            null => "",
            decimal numero => numero.ToString("0.0"),
            double numero => numero.ToString("0.0"),
            _ => valor.ToString() ?? ""
        };
    }
}
