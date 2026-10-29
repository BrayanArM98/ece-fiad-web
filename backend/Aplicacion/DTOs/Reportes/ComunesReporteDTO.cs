namespace Aplicacion.DTOs.Reportes
{
    /// <summary>Rango de fechas que cubre un reporte (ambos extremos inclusivos).</summary>
    public class PeriodoDTO
    {
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public int Dias => (Hasta.Date - Desde.Date).Days + 1;
    }

    /// <summary>Cantidad por categoría, lista para una gráfica de barras o de pastel.</summary>
    public class ConteoDTO
    {
        public string Etiqueta { get; set; } = string.Empty;
        public int Cantidad { get; set; }

        /// <summary>Porcentaje del total, de 0 a 100 con un decimal.</summary>
        public decimal Porcentaje { get; set; }
    }

    /// <summary>
    /// Indicador de la pantalla de inicio. Formato: "numero" o "porcentaje".
    /// Variacion compara contra el periodo anterior: en indicadores numéricos es el cambio porcentual
    /// y en indicadores de porcentaje es la diferencia en puntos. Es null si no hay con qué comparar.
    /// </summary>
    public class IndicadorDTO
    {
        public string Clave { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal? ValorAnterior { get; set; }
        public decimal? Variacion { get; set; }
        public string Formato { get; set; } = "numero";
    }

    /// <summary>Citas de un día o de una semana, desglosadas por resultado.</summary>
    public class PuntoSerieCitasDTO
    {
        public DateTime Fecha { get; set; }
        public int Total { get; set; }
        public int Completadas { get; set; }
        public int NoAsistio { get; set; }
        public int Canceladas { get; set; }

        /// <summary>Pendientes y confirmadas: citas que aún no tienen resultado.</summary>
        public int PorAtender { get; set; }
    }
}
