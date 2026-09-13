namespace Aplicacion.DTOs.Pacientes
{
    public class ActualizarPacienteDTO
    {
        public int Id { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        // No se permite actualizar número de documento, tipo documento, fecha nacimiento, etc.
    }
}