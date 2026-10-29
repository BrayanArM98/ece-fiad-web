namespace Aplicacion.Helpers
{
    // Versión genérica: para operaciones que retornan datos (Crear, Obtener, Actualizar)
    public class ResultadoAccion<T>
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public T? Datos { get; set; }
        public List<string> Errores { get; set; } = new();

        public static ResultadoAccion<T> Exito(T datos, string mensaje = "Operación exitosa")
        {
            return new ResultadoAccion<T> { Exitoso = true, Mensaje = mensaje, Datos = datos };
        }

        public static ResultadoAccion<T> Falla(string mensaje, List<string>? errores = null)
        {
            return new ResultadoAccion<T> { Exitoso = false, Mensaje = mensaje, Errores = errores ?? new() };
        }

        /// <summary>
        /// Falla por datos que no pasan las validaciones (FluentValidation).
        /// El mensaje resume todos los errores para clientes que solo muestran el mensaje
        /// y la lista Errores los entrega por separado para mostrarlos uno por uno.
        /// </summary>
        public static ResultadoAccion<T> FallaValidacion(IEnumerable<string> errores)
        {
            var lista = errores.ToList();
            return new ResultadoAccion<T>
            {
                Exitoso = false,
                Mensaje = $"Datos inválidos: {string.Join(" | ", lista)}",
                Errores = lista
            };
        }
    }

    // Versión sin datos: para operaciones que no retornan datos (Eliminar)
    public class ResultadoAccion
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<string> Errores { get; set; } = new();

        public static ResultadoAccion Exito(string mensaje = "Operación exitosa")
        {
            return new ResultadoAccion { Exitoso = true, Mensaje = mensaje };
        }

        public static ResultadoAccion Falla(string mensaje, List<string>? errores = null)
        {
            return new ResultadoAccion { Exitoso = false, Mensaje = mensaje, Errores = errores ?? new() };
        }
    }
}
