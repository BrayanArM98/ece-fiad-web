namespace Aplicacion.DTOs.Evoluciones
{
    public class ActualizarEvolucionDTO
    {
        // El Id es necesario para identificar qué evolución se va a modificar.
        public int Id { get; set; }

        // Llaves foráneas: aunque la práctica pide mostrar paciente, doctor
        // y fecha como solo lectura en la edición (regla 20), conservamos
        // las propiedades para que el binding del formulario funcione.
        public int IdHistoriaClinica { get; set; }
        public int IdDoctor { get; set; }
        public DateTime Fecha { get; set; }

        // Datos editables de la evolución.
        public string Diagnostico { get; set; } = string.Empty;
        public string Tratamiento { get; set; } = string.Empty;
        public string Notas { get; set; } = string.Empty;

        // El estado se puede cambiar al editar (activar/desactivar la evolución).
        public bool Activo { get; set; }
    }
}