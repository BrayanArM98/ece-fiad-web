namespace Aplicacion.DTOs.Especialidades
{
    public class EspecialidadDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; } = string.Empty;
        public bool Activo { get; set; }

        // Cantidad de doctores asociados a esta especialidad.
        // Se usa en el listado para mostrar la información y para validar al eliminar.
        public int CantidadDoctores { get; set; }
    }
}