namespace Aplicacion.DTOs.HistoriasClinicas
{
    public class CrearHistoriaDTO
    {
        // Llave foránea: el usuario selecciona desde un dropdown
        // de pacientes que aún no tienen historia clínica activa.
        public int IdPaciente { get; set; }

        // Fecha de apertura de la historia clínica.
        // Por defecto se muestra la fecha actual en el formulario.
        public DateTime FechaApertura { get; set; } = DateTime.Now;

        // Alergias del paciente (opcional, máximo 500 caracteres).
        public string Alergias { get; set; } = string.Empty;

        // Antecedentes familiares (opcional, máximo 500 caracteres).
        public string AntecedentesFamiliares { get; set; } = string.Empty;

        // Antecedentes personales (opcional, máximo 500 caracteres).
        public string AntecedentesPersonales { get; set; } = string.Empty;

        // Estado de la historia clínica al crearla.
        // Por defecto activa, el usuario puede cambiarlo con un checkbox.
        public bool Activo { get; set; } = true;
    }
}