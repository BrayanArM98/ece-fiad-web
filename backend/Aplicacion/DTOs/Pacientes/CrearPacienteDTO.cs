using Dominio.Enumeraciones;

namespace Aplicacion.DTOs.Pacientes
{
    public class CrearPacienteDTO
    {
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;
        public TipoDeDocumento TipoDocumento { get; set; }
        public string? Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public Genero Genero { get; set; }
        public GrupoSanguineo GrupoSanguineo { get; set; }
        public string? Direccion { get; set; } = string.Empty;
    }
}