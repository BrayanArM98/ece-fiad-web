using Dominio.Enumeraciones;

namespace Aplicacion.DTOs.Reportes
{
    // Filas planas que devuelve la base de datos para calcular los reportes.
    // Solo traen las columnas necesarias; los totales se calculan en la capa de aplicación.

    public record FilaCitaReporte(
        int Id,
        DateTime FechaHora,
        EstadoCita Estado,
        int IdDoctor,
        string NombreDoctor,
        int IdEspecialidad,
        string NombreEspecialidad,
        int IdPaciente);

    public record FilaEvolucionReporte(DateTime Fecha, int IdDoctor, string Diagnostico);

    public record FilaPacienteReporte(DateTime FechaNacimiento, Genero Genero, GrupoSanguineo GrupoSanguineo);

    public record FilaDoctorReporte(int Id, string NombreCompleto, string Especialidad);
}
