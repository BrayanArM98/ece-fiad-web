using System.ComponentModel.DataAnnotations;

namespace Dominio.Enumeraciones
{
    public enum TipoDeDocumento
    {
        [Display(Name = "Cédula")]
        Cedula = 1,
        Pasaporte = 2,
        Otro = 3
    }
}