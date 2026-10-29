using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Reportes;

namespace Aplicacion.Features.Reportes.Comun
{
    /// <summary>
    /// Periodo y doctor que realmente cubre un reporte.
    /// Si quien lo pide es un médico, el reporte siempre es de él, sin importar el filtro enviado.
    /// </summary>
    internal record AlcanceReporte(PeriodoDTO Periodo, int? IdDoctor, int? IdEspecialidad)
    {
        // Un médico sin doctor vinculado no ve datos de nadie
        private const int SinDoctor = -1;

        public bool EsDeUnDoctor => IdDoctor.HasValue;

        public static AlcanceReporte Crear(FiltroReporteDTO filtro, IUsuarioActual usuario, DateTime hoy)
        {
            var idDoctor = usuario.EsMedico ? usuario.IdDoctor ?? SinDoctor : filtro.IdDoctor;
            return new AlcanceReporte(Periodos.Resolver(filtro, hoy), idDoctor, filtro.IdEspecialidad);
        }
    }
}
