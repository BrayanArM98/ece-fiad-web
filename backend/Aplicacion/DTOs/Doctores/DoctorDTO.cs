namespace Aplicacion.DTOs.Doctores
{
    public class DoctorDTO
    {
        public int Id { get; set; }

        // Datos personales del doctor
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;

        // Propiedad calculada para mostrar en listados
        public string NombreCompleto => $"{Nombres} {Apellidos}";

        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string HorarioAtencion { get; set; } = string.Empty;

        // Datos de la especialidad asociada
        public int IdEspecialidad { get; set; }
        public string NombreEspecialidad { get; set; } = string.Empty;

        // Estado del registro
        public bool Activo { get; set; }

        // Cantidad de citas asociadas (se usa en el listado y para validar al eliminar).
        public int CantidadCitas { get; set; }
    }
}