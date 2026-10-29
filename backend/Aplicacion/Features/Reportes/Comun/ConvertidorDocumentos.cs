using Aplicacion.DTOs.Reportes;

namespace Aplicacion.Features.Reportes.Comun
{
    /// <summary>
    /// Convierte cada reporte en un DocumentoReporte (indicadores y tablas)
    /// que después se exporta a PDF o Excel con el mismo contenido.
    /// </summary>
    internal static class ConvertidorDocumentos
    {
        private static string Porcentaje(decimal? valor) => valor.HasValue ? $"{valor:0.#}%" : "—";

        private static TablaDocumento TablaConteos(string titulo, string columna, IEnumerable<ConteoDTO> conteos) => new()
        {
            Titulo = titulo,
            Columnas = new() { columna, "Cantidad", "Porcentaje" },
            Filas = conteos.Select(c => new object[] { c.Etiqueta, c.Cantidad, c.Porcentaje }).ToList()
        };

        public static DocumentoReporte De(ResumenGeneralDTO r) => new()
        {
            Titulo = "Resumen general",
            Detalles = { $"Periodo anterior comparado: {r.PeriodoAnterior.Desde:dd/MM/yyyy} al {r.PeriodoAnterior.Hasta:dd/MM/yyyy}" },
            Indicadores = r.Indicadores
                .Select(i => (i.Titulo, i.Formato == "porcentaje" ? Porcentaje(i.Valor) : $"{i.Valor:0}"))
                .ToList(),
            Tablas =
            {
                new TablaDocumento
                {
                    Titulo = "Comparación con el periodo anterior",
                    Columnas = new() { "Indicador", "Periodo", "Periodo anterior", "Variación" },
                    Filas = r.Indicadores.Select(i => new object[]
                    {
                        i.Titulo,
                        i.Valor,
                        (object?)i.ValorAnterior ?? "—",
                        i.Variacion.HasValue
                            ? (i.Formato == "porcentaje" ? $"{i.Variacion:+0.#;-0.#;0} pts" : $"{i.Variacion:+0.#;-0.#;0}%")
                            : "—"
                    }).ToList()
                }
            }
        };

        public static DocumentoReporte De(ReporteCitasDTO r) => new()
        {
            Titulo = "Reporte de citas",
            Indicadores = new() { ("Total de citas", r.Total.ToString()) },
            Tablas =
            {
                TablaConteos("Citas por estado", "Estado", r.PorEstado),
                new TablaDocumento
                {
                    Titulo = r.Agrupacion == "dia" ? "Citas por día" : "Citas por semana",
                    Columnas = new() { r.Agrupacion == "dia" ? "Día" : "Semana del", "Total", "Completadas", "No asistió", "Canceladas", "Por atender" },
                    Filas = r.Serie.Select(p => new object[]
                    {
                        p.Fecha.ToString("dd/MM/yyyy"), p.Total, p.Completadas, p.NoAsistio, p.Canceladas, p.PorAtender
                    }).ToList()
                },
                TablaConteos("Citas por doctor", "Doctor", r.PorDoctor),
                TablaConteos("Citas por especialidad", "Especialidad", r.PorEspecialidad)
            }
        };

        public static DocumentoReporte De(ReporteProductividadDTO r) => new()
        {
            Titulo = "Productividad médica",
            Indicadores = new()
            {
                ("Doctores", r.Doctores.Count.ToString()),
                ("Consultas completadas", r.Doctores.Sum(d => d.Completadas).ToString()),
                ("Evoluciones registradas", r.Doctores.Sum(d => d.Evoluciones).ToString())
            },
            Tablas =
            {
                new TablaDocumento
                {
                    Titulo = "Desempeño por doctor",
                    Columnas = new() { "Doctor", "Especialidad", "Citas", "Completadas", "No asistió", "Canceladas", "Por atender", "Evoluciones", "Asistencia" },
                    Filas = r.Doctores.Select(d => new object[]
                    {
                        d.NombreDoctor, d.Especialidad, d.CitasTotales, d.Completadas, d.NoAsistio,
                        d.Canceladas, d.PorAtender, d.Evoluciones, Porcentaje(d.TasaAsistencia)
                    }).ToList()
                }
            }
        };

        public static DocumentoReporte De(ReportePacientesDTO r) => new()
        {
            Titulo = "Reporte de pacientes",
            Indicadores = new()
            {
                ("Pacientes activos", r.PacientesActivos.ToString()),
                ("Pacientes nuevos en el periodo", r.NuevosEnPeriodo.ToString())
            },
            Tablas =
            {
                TablaConteos("Pacientes activos por género", "Género", r.PorGenero),
                TablaConteos("Pacientes activos por edad", "Rango de edad", r.PorRangoEdad),
                TablaConteos("Pacientes activos por grupo sanguíneo", "Grupo sanguíneo", r.PorGrupoSanguineo)
            }
        };

        public static DocumentoReporte De(ReporteDiagnosticosDTO r) => new()
        {
            Titulo = "Diagnósticos más frecuentes",
            Indicadores = new() { ("Evoluciones en el periodo", r.TotalEvoluciones.ToString()) },
            Tablas = { TablaConteos("Diagnósticos", "Diagnóstico", r.Diagnosticos) }
        };
    }
}
