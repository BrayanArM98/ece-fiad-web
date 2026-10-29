using Aplicacion.DTOs.Reportes;
using Dominio.Enumeraciones;

namespace Aplicacion.Features.Reportes.Comun
{
    /// <summary>
    /// Cálculos de los reportes a partir de las filas que devuelve la base de datos.
    /// No accede a datos: recibe listas y devuelve totales, así que se puede probar por separado.
    /// </summary>
    internal static class CalculadoraReportes
    {
        public static decimal Porcentaje(int parte, int total) =>
            total == 0 ? 0 : Math.Round(parte * 100m / total, 1);

        /// <summary>Completadas entre citas con resultado (completadas + inasistencias).</summary>
        public static decimal? TasaAsistencia(int completadas, int noAsistio) =>
            completadas + noAsistio == 0 ? null : Porcentaje(completadas, completadas + noAsistio);

        public static string TextoEstado(EstadoCita estado) => estado switch
        {
            EstadoCita.NoAsistio => "No asistió",
            _ => estado.ToString()
        };

        public static string TextoGenero(Genero genero) => genero switch
        {
            Genero.PrefieroNoDecir => "Prefiero no decir",
            _ => genero.ToString()
        };

        public static string TextoGrupoSanguineo(GrupoSanguineo grupo) => grupo switch
        {
            GrupoSanguineo.APositivo => "A+",
            GrupoSanguineo.ANegativo => "A-",
            GrupoSanguineo.BPositivo => "B+",
            GrupoSanguineo.BNegativo => "B-",
            GrupoSanguineo.ABPositivo => "AB+",
            GrupoSanguineo.ABNegativo => "AB-",
            GrupoSanguineo.OPositivo => "O+",
            GrupoSanguineo.ONegativo => "O-",
            _ => grupo.ToString()
        };

        public static readonly string[] RangosEdad = { "0 a 11", "12 a 17", "18 a 29", "30 a 44", "45 a 59", "60 o más" };

        public static string RangoEdad(DateTime fechaNacimiento, DateTime hoy)
        {
            var edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > hoy.Date.AddYears(-edad)) edad--;

            return edad switch
            {
                < 12 => RangosEdad[0],
                < 18 => RangosEdad[1],
                < 30 => RangosEdad[2],
                < 45 => RangosEdad[3],
                < 60 => RangosEdad[4],
                _ => RangosEdad[5]
            };
        }

        /// <summary>
        /// Cuenta cuántas veces aparece cada etiqueta, de la más frecuente a la menos frecuente.
        /// Si se pasa un orden, se respeta ese orden e incluye las categorías en cero.
        /// </summary>
        public static List<ConteoDTO> Contar(IEnumerable<string> etiquetas, IEnumerable<string>? orden = null)
        {
            var lista = etiquetas.ToList();
            var total = lista.Count;
            var grupos = lista.GroupBy(e => e).ToDictionary(g => g.Key, g => g.Count());

            var claves = orden?.ToList()
                ?? grupos.OrderByDescending(g => g.Value).ThenBy(g => g.Key).Select(g => g.Key).ToList();

            return claves.Select(clave =>
            {
                var cantidad = grupos.GetValueOrDefault(clave);
                return new ConteoDTO { Etiqueta = clave, Cantidad = cantidad, Porcentaje = Porcentaje(cantidad, total) };
            }).ToList();
        }

        /// <summary>
        /// Serie de citas por día (periodos de hasta 31 días) o por semana (periodos mayores).
        /// Incluye los días o semanas sin citas, para que la gráfica no tenga huecos.
        /// </summary>
        public static (string Agrupacion, List<PuntoSerieCitasDTO> Serie) Serie(
            IReadOnlyCollection<FilaCitaReporte> citas, PeriodoDTO periodo)
        {
            var dias = periodo.Dias <= 31 ? 1 : 7;
            var serie = new List<PuntoSerieCitasDTO>();

            for (var inicio = periodo.Desde.Date; inicio <= periodo.Hasta.Date; inicio = inicio.AddDays(dias))
            {
                var fin = inicio.AddDays(dias);
                var enRango = citas.Where(c => c.FechaHora >= inicio && c.FechaHora < fin).ToList();

                serie.Add(new PuntoSerieCitasDTO
                {
                    Fecha = inicio,
                    Total = enRango.Count,
                    Completadas = enRango.Count(c => c.Estado == EstadoCita.Completada),
                    NoAsistio = enRango.Count(c => c.Estado == EstadoCita.NoAsistio),
                    Canceladas = enRango.Count(c => c.Estado == EstadoCita.Cancelada),
                    PorAtender = enRango.Count(c => c.Estado is EstadoCita.Pendiente or EstadoCita.Confirmada)
                });
            }

            return (dias == 1 ? "dia" : "semana", serie);
        }

        /// <summary>
        /// Agrupa diagnósticos escritos a mano: ignora mayúsculas, espacios repetidos y el punto final.
        /// Muestra la forma en que se escribió con más frecuencia.
        /// </summary>
        public static List<ConteoDTO> DiagnosticosFrecuentes(IReadOnlyCollection<FilaEvolucionReporte> evoluciones, int limite)
        {
            static string Normalizar(string texto) =>
                string.Join(' ', texto.Trim().TrimEnd('.').ToLowerInvariant()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries));

            var total = evoluciones.Count;

            return evoluciones
                .Where(e => !string.IsNullOrWhiteSpace(e.Diagnostico))
                .GroupBy(e => Normalizar(e.Diagnostico))
                .Select(g => new
                {
                    Etiqueta = g.GroupBy(e => e.Diagnostico.Trim().TrimEnd('.'))
                        .OrderByDescending(v => v.Count()).First().Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(d => d.Cantidad)
                .ThenBy(d => d.Etiqueta)
                .Take(limite)
                .Select(d => new ConteoDTO
                {
                    Etiqueta = d.Etiqueta,
                    Cantidad = d.Cantidad,
                    Porcentaje = Porcentaje(d.Cantidad, total)
                })
                .ToList();
        }

        public static IndicadorDTO Indicador(string clave, string titulo, decimal valor, decimal? anterior, string formato = "numero")
        {
            decimal? variacion = null;
            if (anterior.HasValue)
            {
                if (formato == "porcentaje")
                    variacion = Math.Round(valor - anterior.Value, 1);
                else if (anterior.Value != 0)
                    variacion = Math.Round((valor - anterior.Value) * 100m / anterior.Value, 1);
            }

            return new IndicadorDTO
            {
                Clave = clave,
                Titulo = titulo,
                Valor = valor,
                ValorAnterior = anterior,
                Variacion = variacion,
                Formato = formato
            };
        }
    }
}
