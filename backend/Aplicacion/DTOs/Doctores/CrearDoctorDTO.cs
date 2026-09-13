namespace Aplicacion.DTOs.Doctores
{
    public class CrearDoctorDTO
    {
        // Datos personales que el usuario captura en el formulario.
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string HorarioAtencion { get; set; } = string.Empty;

        // Llave foránea: el usuario selecciona la especialidad desde un dropdown.
        public int IdEspecialidad { get; set; }
    }
}