namespace Aplicacion.DTOs.Doctores
{
    public class ActualizarDoctorDTO
    {
        // El Id es necesario para identificar qué doctor se va a modificar.
        public int Id { get; set; }

        // Datos editables (mismos campos que CrearDoctorDTO).
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string HorarioAtencion { get; set; } = string.Empty;

        // Permite cambiar la especialidad asignada al doctor.
        public int IdEspecialidad { get; set; }
    }
}