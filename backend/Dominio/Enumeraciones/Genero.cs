using System.ComponentModel.DataAnnotations;

namespace Dominio.Enumeraciones
{
    public enum Genero
    {
        Masculino = 1,
        Femenino = 2,
        Otro = 3,
        [Display(Name = "Prefiero no decir")]
        PrefieroNoDecir = 4
    }
}