namespace Aplicacion.DTOs.Evoluciones
{
    public class CrearEvolucionDTO
    {
        // Llaves foráneas: el usuario selecciona desde dos dropdowns.
        public int IdHistoriaClinica { get; set; }
        public int IdDoctor { get; set; }

        // Fecha de la evolución (regla 22: no puede ser futura).
        // Por defecto se muestra la fecha actual en el formulario.
        public DateTime Fecha { get; set; } = DateTime.Now;

        // Diagnóstico médico (obligatorio, máximo 500 caracteres).
        public string Diagnostico { get; set; } = string.Empty;

        // Tratamiento indicado (obligatorio, máximo 500 caracteres).
        public string Tratamiento { get; set; } = string.Empty;

        // Notas adicionales (opcional, máximo 1000 caracteres).
        public string Notas { get; set; } = string.Empty;

        // Estado de la evolución al crearla.
        // Por defecto activa, el usuario puede cambiarlo con un checkbox.
        public bool Activo { get; set; } = true;
    }
}