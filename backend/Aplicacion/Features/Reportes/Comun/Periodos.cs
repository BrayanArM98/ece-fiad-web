using Aplicacion.DTOs.Reportes;

namespace Aplicacion.Features.Reportes.Comun
{
    /// <summary>
    /// Reglas de fechas de los reportes.
    /// Sin fechas: el mes actual completo. Solo con "desde": hasta hoy (o ese mismo día si es futuro).
    /// Solo con "hasta": desde el primer día de ese mes.
    /// </summary>
    internal static class Periodos
    {
        public static PeriodoDTO Resolver(FiltroReporteDTO filtro, DateTime hoy)
        {
            DateTime desde, hasta;

            if (filtro.Desde.HasValue && filtro.Hasta.HasValue)
            {
                desde = filtro.Desde.Value.Date;
                hasta = filtro.Hasta.Value.Date;
            }
            else if (filtro.Desde.HasValue)
            {
                desde = filtro.Desde.Value.Date;
                hasta = desde > hoy.Date ? desde : hoy.Date;
            }
            else if (filtro.Hasta.HasValue)
            {
                hasta = filtro.Hasta.Value.Date;
                desde = new DateTime(hasta.Year, hasta.Month, 1);
            }
            else
            {
                desde = new DateTime(hoy.Year, hoy.Month, 1);
                hasta = desde.AddMonths(1).AddDays(-1);
            }

            return new PeriodoDTO { Desde = desde, Hasta = hasta };
        }

        /// <summary>El periodo de la misma duración inmediatamente anterior.</summary>
        public static PeriodoDTO Anterior(PeriodoDTO periodo)
        {
            var hasta = periodo.Desde.AddDays(-1);
            return new PeriodoDTO { Desde = hasta.AddDays(-(periodo.Dias - 1)), Hasta = hasta };
        }

        /// <summary>Límite superior para las consultas: el inicio del día siguiente a "hasta".</summary>
        public static DateTime HastaExclusivo(PeriodoDTO periodo) => periodo.Hasta.Date.AddDays(1);

        public static string Describir(PeriodoDTO periodo) =>
            $"Periodo: {periodo.Desde:dd/MM/yyyy} al {periodo.Hasta:dd/MM/yyyy}";
    }
}
