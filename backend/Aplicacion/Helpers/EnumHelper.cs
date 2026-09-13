using System.ComponentModel.DataAnnotations;

namespace Aplicacion.Helpers
{
    public static class EnumHelper
    {
        // Obtiene todos los valores de una enumeración junto con sus nombres mostrables.
        // Respeta el atributo [Display(Name = "...")] cuando está presente.
        public static List<(T Value, string Nombre)> ObtenerOpcionesEnum<T>() where T : Enum
        {
            var type = typeof(T);
            var valores = Enum.GetValues(type).Cast<T>();
            var resultado = new List<(T, string)>();

            foreach (var valor in valores)
            {
                var nombre = ObtenerNombreEnum(valor);
                resultado.Add((valor, nombre));
            }

            return resultado;
        }

        // Obtiene el nombre mostrable de un valor enum, respetando el atributo [Display(Name = "...")].
        // Si el valor no tiene atributo Display, retorna el nombre literal del enumerado.
        public static string ObtenerNombreEnum<T>(T valor) where T : Enum
        {
            var campo = valor.GetType().GetField(valor.ToString());
            var display = campo?.GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;

            return display?.Name ?? valor.ToString();
        }
    }
}