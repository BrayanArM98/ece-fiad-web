using Dominio.Entidades.Bases;
using Dominio.Entidades.Doctores;

namespace Dominio.Entidades.Especialidades
{
    public class Especialidad : EntidadBase
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        public virtual ICollection<Doctor> Doctores { get; set; } = new List<Doctor>();
    }
}