namespace Aplicacion.DTOs.Reportes
{
    /// <summary>Reporte 1: indicadores generales del periodo, comparados con el periodo anterior.</summary>
    public class ResumenGeneralDTO
    {
        public PeriodoDTO Periodo { get; set; } = new();
        public PeriodoDTO PeriodoAnterior { get; set; } = new();
        public List<IndicadorDTO> Indicadores { get; set; } = new();
    }

    /// <summary>Reporte 2: citas por estado, en el tiempo, por doctor y por especialidad.</summary>
    public class ReporteCitasDTO
    {
        public PeriodoDTO Periodo { get; set; } = new();
        public int Total { get; set; }

        /// <summary>"dia" si el periodo es de hasta 31 días; "semana" si es mayor.</summary>
        public string Agrupacion { get; set; } = "dia";

        public List<ConteoDTO> PorEstado { get; set; } = new();
        public List<PuntoSerieCitasDTO> Serie { get; set; } = new();
        public List<ConteoDTO> PorDoctor { get; set; } = new();
        public List<ConteoDTO> PorEspecialidad { get; set; } = new();
    }

    /// <summary>Reporte 3: desempeño de cada doctor en el periodo.</summary>
    public class ReporteProductividadDTO
    {
        public PeriodoDTO Periodo { get; set; } = new();
        public List<ProductividadDoctorDTO> Doctores { get; set; } = new();
    }

    public class ProductividadDoctorDTO
    {
        public int IdDoctor { get; set; }
        public string NombreDoctor { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public int CitasTotales { get; set; }
        public int Completadas { get; set; }
        public int NoAsistio { get; set; }
        public int Canceladas { get; set; }
        public int PorAtender { get; set; }
        public int Evoluciones { get; set; }

        /// <summary>Completadas entre (completadas + inasistencias), de 0 a 100. Null si no hubo citas con resultado.</summary>
        public decimal? TasaAsistencia { get; set; }
    }

    /// <summary>Reporte 4: altas del periodo y composición de los pacientes activos.</summary>
    public class ReportePacientesDTO
    {
        public PeriodoDTO Periodo { get; set; } = new();
        public int PacientesActivos { get; set; }
        public int NuevosEnPeriodo { get; set; }
        public List<ConteoDTO> PorGenero { get; set; } = new();
        public List<ConteoDTO> PorRangoEdad { get; set; } = new();
        public List<ConteoDTO> PorGrupoSanguineo { get; set; } = new();
    }

    /// <summary>Reporte 5: diagnósticos más frecuentes en las evoluciones del periodo.</summary>
    public class ReporteDiagnosticosDTO
    {
        public PeriodoDTO Periodo { get; set; } = new();
        public int TotalEvoluciones { get; set; }
        public List<ConteoDTO> Diagnosticos { get; set; } = new();
    }
}
